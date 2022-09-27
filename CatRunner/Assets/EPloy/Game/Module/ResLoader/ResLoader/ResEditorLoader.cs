using EPloy.Res;
using EPloy.TaskPool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器资源组件。
    /// </summary>
    public sealed class ResEditorLoader: IResLoader
    {
        private static ResEditorLoader instance = null;
        public static ResEditorLoader CreateResEditorLoader()
        {
            if (instance == null) instance = new ResEditorLoader();
            return instance;
        }
        internal static ResEditorLoader Instance
        {
            get
            {
                return CreateResEditorLoader();
            }
        }

        private static readonly int AssetsStringLength = "Assets".Length;
        private bool EnableCachedAssets = true;
        private int LoadAssetCountPerFrame = 5;
        private Dictionary<string, UnityEngine.Object> CachedAssets = null;
        private TypeLinkedList<LoadAssetInfo> LoadAssetInfos = null;
        private TypeLinkedList<LoadSceneInfo> LoadSceneInfos = null;
        private TypeLinkedList<UnloadSceneInfo> UnloadSceneInfos = null;

        /// <summary>
        /// 获取等待编辑器加载的资源数量。
        /// </summary>
        public int LoadWaitingAssetCount
        {
            get
            {
                return LoadAssetInfos.Count;
            }
        }


        private ResEditorLoader()
        {
            CachedAssets = new Dictionary<string, UnityEngine.Object>(StringComparer.Ordinal);
            LoadAssetInfos = new TypeLinkedList<LoadAssetInfo>();
            LoadSceneInfos = new TypeLinkedList<LoadSceneInfo>();
            UnloadSceneInfos = new TypeLinkedList<UnloadSceneInfo>();
        }

        public void OnDestroy()
        {
            CachedAssets.Clear();
            LoadAssetInfos.Clear();
            LoadSceneInfos.Clear();
            UnloadSceneInfos.Clear();
        }

        public void Update()
        {
            if (LoadAssetInfos.Count > 0)
            {
                int count = 0;
                LinkedListNode<LoadAssetInfo> current = LoadAssetInfos.First;
                while (current != null && count < LoadAssetCountPerFrame)
                {
                    LoadAssetInfo loadAssetInfo = current.Value;
                    float elapseSeconds = (float)(DateTime.UtcNow - loadAssetInfo.StartTime).TotalSeconds;
                    if (elapseSeconds >= loadAssetInfo.DelaySeconds)
                    {
                        UnityEngine.Object asset = GetCachedAsset(loadAssetInfo.AssetName);
                        if (asset == null)
                        {
#if UNITY_EDITOR
                            if (loadAssetInfo.AssetType != null)
                            {
                                asset = ResEditor.LoadAssetAtPath(loadAssetInfo.AssetName, loadAssetInfo.AssetType);
                            }
                            else
                            {
                                asset = ResEditor.LoadMainAssetAtPath(loadAssetInfo.AssetName);
                            }
                            if (EnableCachedAssets && asset != null)
                            {
                                CachedAssets.Add(loadAssetInfo.AssetName, asset);
                            }
#endif
                        }

                        if (asset)
                        {
                            if (loadAssetInfo.LoadAssetCallbacks.LoadAssetSuccessCallback != null)
                            {
                                try
                                {
                                    loadAssetInfo.LoadAssetCallbacks.LoadAssetSuccessCallback(loadAssetInfo.AssetName, asset, elapseSeconds, loadAssetInfo.UserData);
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e.ToString());
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            if (loadAssetInfo.LoadAssetCallbacks.LoadAssetFailureCallback != null)
                            {
                                loadAssetInfo.LoadAssetCallbacks.LoadAssetFailureCallback(loadAssetInfo.AssetName, LoadResStatus.AssetError, "Can not load this asset from asset database.");
                            }
                        }

                        LoadAssetInfos.RemoveFirst();
                        if (LoadAssetInfos.Count > 1)
                        {
                            current = LoadAssetInfos.First;
                            count++;
                        }
                        else
                        {
                            current = null;
                        }
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
            }

            if (LoadSceneInfos.Count > 0)
            {
                LinkedListNode<LoadSceneInfo> current = LoadSceneInfos.First;
                while (current != null)
                {
                    LoadSceneInfo loadSceneInfo = current.Value;
                    if (loadSceneInfo.AsyncOperation.isDone)
                    {
                        if (loadSceneInfo.AsyncOperation.allowSceneActivation)
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadSceneSuccessCallback != null)
                            {
                                loadSceneInfo.LoadSceneCallbacks.LoadSceneSuccessCallback(loadSceneInfo.SceneAssetName, (float)(DateTime.UtcNow - loadSceneInfo.StartTime).TotalSeconds);
                            }
                        }
                        else
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadSceneFailureCallback != null)
                            {
                                loadSceneInfo.LoadSceneCallbacks.LoadSceneFailureCallback(loadSceneInfo.SceneAssetName, LoadResStatus.AssetError, "Can not load this scene from asset database.");
                            }
                        }

                        LinkedListNode<LoadSceneInfo> next = current.Next;
                        LoadSceneInfos.Remove(current);
                        current = next;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
            }

            if (UnloadSceneInfos.Count > 0)
            {
                LinkedListNode<UnloadSceneInfo> current = UnloadSceneInfos.First;
                while (current != null)
                {
                    UnloadSceneInfo unloadSceneInfo = current.Value;
                    if (unloadSceneInfo.AsyncOperation.isDone)
                    {
                        if (unloadSceneInfo.AsyncOperation.allowSceneActivation)
                        {
                            if (unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                            {
                                unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneSuccessCallback(unloadSceneInfo.SceneAssetName);
                            }
                        }
                        else
                        {
                            if (unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneFailureCallback != null)
                            {
                                unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneFailureCallback(unloadSceneInfo.SceneAssetName);
                            }
                        }

                        LinkedListNode<UnloadSceneInfo> next = current.Next;
                        UnloadSceneInfos.Remove(unloadSceneInfo);
                        current = next;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
            }
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasResult HasAsset(string assetName)
        {
#if UNITY_EDITOR
            return (HasResult)ResEditor.HasAsset(assetName);
#else
            return HasResult.NotExist;
#endif
        }


        /// <summary>
        /// 获取所有加载资源任务的信息。
        /// </summary>
        /// <returns>所有加载资源任务的信息。</returns>
        public TaskInfo[] GetAllLoadAssetInfos()
        {
            Log.Fatal("Task no use in Editor");
            return null;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (loadAssetCallbacks == null)
            {
                Log.Error("Load asset callbacks is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(assetName))
            {
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.NotExist, "Asset name is invalid.");
                }

                return;
            }

            if (!assetName.StartsWith("Assets/", StringComparison.Ordinal))
            {
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.NotExist, UtilText.Format("Asset name '{0}' is invalid.", assetName));
                }

                return;
            }

            if (!HasFile(assetName))
            {
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.NotExist, UtilText.Format("Asset '{0}' is not exist.", assetName));
                }

                return;
            }

            LoadAssetInfos.AddLast(new LoadAssetInfo(assetName, assetType, DateTime.UtcNow, 0.5f, loadAssetCallbacks, userData));
        }

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks)
        {
            if (loadSceneCallbacks == null)
            {
                Log.Error("Load scene callbacks is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(sceneAssetName))
            {
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.NotExist, "Scene asset name is invalid.");
                }

                return;
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) || !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.NotExist, UtilText.Format("Scene asset name '{0}' is invalid.", sceneAssetName));
                }

                return;
            }

            if (!HasFile(sceneAssetName))
            {
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.NotExist, UtilText.Format("Scene '{0}' is not exist.", sceneAssetName));
                }

                return;
            }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneAssetName, LoadSceneMode.Single);
            if (asyncOperation == null)
            {
                return;
            }

            LoadSceneInfos.AddLast(new LoadSceneInfo(asyncOperation, sceneAssetName, DateTime.UtcNow, loadSceneCallbacks));
        }

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) || !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                Log.Error(UtilText.Format("Scene asset name '{0}' is invalid.", sceneAssetName));
                return;
            }

            if (unloadSceneCallbacks == null)
            {
                Log.Error("Unload scene callbacks is invalid.");
                return;
            }

            if (!HasFile(sceneAssetName))
            {
                Log.Error(UtilText.Format("Scene '{0}' is not exist.", sceneAssetName));
                return;
            }

            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneAssetName);
            if (asyncOperation == null)
            {
                unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName);
            return;
            }
            UnloadSceneInfos.AddLast(new UnloadSceneInfo(asyncOperation, sceneAssetName, unloadSceneCallbacks));
        }

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            if (loadBinaryCallbacks == null)
            {
                Log.Error("Load binary callbacks is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(binaryAssetName))
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotExist, "Binary asset name is invalid.");
                }

                return;
            }

            if (!binaryAssetName.StartsWith("Assets/", StringComparison.Ordinal))
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotExist, UtilText.Format("Binary asset name '{0}' is invalid.", binaryAssetName));
                }

                return;
            }

            string binaryPath = GetBinaryPath(binaryAssetName);
            if (string.IsNullOrEmpty(binaryPath))
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotExist, UtilText.Format("Binary asset '{0}' is not exist.", binaryAssetName));
                }

                return;
            }

            try
            {
                byte[] binaryBytes = File.ReadAllBytes(binaryPath);
                loadBinaryCallbacks.LoadBinarySuccessCallback(binaryAssetName, binaryBytes, 0f);
            }
            catch (Exception exception)
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.AssetError, exception.ToString());
                }
            }
        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        public string GetBinaryPath(string binaryAssetName)
        {
            if (!HasFile(binaryAssetName))
            {
                return null;
            }
            return Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + binaryAssetName;
        }

        /// <summary>
        /// 获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        public int GetBinaryLength(string binaryAssetName)
        {
            string binaryPath = GetBinaryPath(binaryAssetName);
            if (string.IsNullOrEmpty(binaryPath))
            {
                return -1;
            }

            return (int)new System.IO.FileInfo(binaryPath).Length;
        }

        private bool HasFile(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            if (HasCachedAsset(assetName))
            {
                return true;
            }

            string assetFullName = Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + assetName;
            if (string.IsNullOrEmpty(assetFullName))
            {
                return false;
            }

            string[] splitedAssetFullName = assetFullName.Split('/');
            string currentPath = Path.GetPathRoot(assetFullName);
            for (int i = 1; i < splitedAssetFullName.Length - 1; i++)
            {
                string[] directoryNames = Directory.GetDirectories(currentPath, splitedAssetFullName[i]);
                if (directoryNames.Length != 1)
                {
                    return false;
                }

                currentPath = directoryNames[0];
            }

            string[] fileNames = Directory.GetFiles(currentPath, splitedAssetFullName[splitedAssetFullName.Length - 1]);
            if (fileNames.Length != 1)
            {
                return false;
            }

            string fileFullName = UtilPath.GetRegularPath(fileNames[0]);
            if (fileFullName == null)
            {
                return false;
            }

            if (assetFullName != fileFullName)
            {
                if (assetFullName.ToLowerInvariant() == fileFullName.ToLowerInvariant())
                {
                    string msg = UtilText.Format("The real path of the specific asset '{0}' is '{1}'. Check the case of letters in the path.", assetName, "Assets" + fileFullName.Substring(Application.dataPath.Length));
                    Log.Warning(msg);
                }

                return false;
            }

            return true;
        }

        private bool HasCachedAsset(string assetName)
        {
            if (!EnableCachedAssets)
            {
                return false;
            }

            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            return CachedAssets.ContainsKey(assetName);
        }

        private UnityEngine.Object GetCachedAsset(string assetName)
        {
            if (!EnableCachedAssets)
            {
                return null;
            }

            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            UnityEngine.Object asset = null;
            if (CachedAssets.TryGetValue(assetName, out asset))
            {
                return asset;
            }

            return null;
        }

        [StructLayout(LayoutKind.Auto)]
        private struct LoadAssetInfo
        {
            public string AssetName
            {
                get;
                private set;
            }
            public Type AssetType
            {
                get;
                private set;
            }
            public DateTime StartTime
            {
                get;
                private set;
            }
            public float DelaySeconds
            {
                get;
                private set;
            }
            public LoadAssetCallbacks LoadAssetCallbacks
            {
                get;
                private set;
            }
            public object UserData
            {
                get;
                private set;
            }
            public LoadAssetInfo(string assetName, Type assetType, DateTime startTime, float delaySeconds, LoadAssetCallbacks loadAssetCallbacks, object userData)
            {
                AssetName = assetName;
                AssetType = assetType;
                StartTime = startTime;
                DelaySeconds = delaySeconds;
                LoadAssetCallbacks = loadAssetCallbacks;
                UserData = userData;
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private struct LoadSceneInfo
        {
            public AsyncOperation AsyncOperation
            {
                get;
                private set;
            }
            public string SceneAssetName
            {
                get;
                private set;
            }
            public DateTime StartTime
            {
                get;
                private set;
            }
            public LoadSceneCallbacks LoadSceneCallbacks
            {
                get;
                private set;
            }

            public LoadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName, DateTime startTime, LoadSceneCallbacks loadSceneCallbacks)
            {
                AsyncOperation = asyncOperation;
                SceneAssetName = sceneAssetName;
                StartTime = startTime;
                LoadSceneCallbacks = loadSceneCallbacks;
            }

        }

        [StructLayout(LayoutKind.Auto)]
        private struct UnloadSceneInfo
        {
            public AsyncOperation AsyncOperation
            {
                get;
                private set;
            }

            public string SceneAssetName
            {
                get;
                private set;
            }

            public UnloadSceneCallbacks UnloadSceneCallbacks
            {
                get;
                private set;
            }

            public UnloadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
            {
                AsyncOperation = asyncOperation;
                SceneAssetName = sceneAssetName;
                UnloadSceneCallbacks = unloadSceneCallbacks;
            }
        }
    }
#endif
