using System;
using System.Collections.Generic;
using System.IO;
using EPloy.Game.Obj;
using EPloy.Game.ObjectPool;
using EPloy.Game.TaskPool;
using UnityEngine;

namespace EPloy.Game.Res
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public sealed class ResLoader : IResLoader
    {
        private static ResLoader instance = null;
        public static ResLoader CreateResLoader()
        {
            if (instance == null) instance = new ResLoader();
            return instance;
        }
        internal static ResLoader Instance
        {
            get
            {
                return CreateResLoader();
            }
        }

        public ObjectPoolBase AssetPool { get; private set; }
        public ObjectPoolBase ResourcePool { get; private set; }

        internal TaskPool<LoadResTaskBase> TaskPool { get; private set; }
        public Dictionary<object, int> AssetDependCount { get; private set; }
        public Dictionary<object, int> ResourceDependCount { get; private set; }
        public Dictionary<object, object> AssetToResourceMap { get; private set; }
        public Dictionary<string, object> SceneToAssetMap { get; private set; }
        internal ResHelper ResHelper
        {
            get
            {
                return GameModule.Res.ResHelper;
            }
        }
        /// <summary>
        /// 资产路径
        /// </summary>
        internal string ResPath = null;

        private ResLoader()
        {
            SetObjectPoolManager();
            TaskPool = new TaskPool<LoadResTaskBase>();
            AssetDependCount = new Dictionary<object, int>();
            ResourceDependCount = new Dictionary<object, int>();
            AssetToResourceMap = new Dictionary<object, object>();
            SceneToAssetMap = new Dictionary<string, object>(StringComparer.Ordinal);
            for (int i = 0; i < 5; i++)
            {
                TaskPool.AddAgent(new LoadResAgent(), Application.streamingAssetsPath);
            }
        }

        /// <summary>
        /// 关闭并清理加载资源器。
        /// </summary>
        public void OnDestroy()
        {
            TaskPool.OnDestroy();
            AssetDependCount.Clear();
            ResourceDependCount.Clear();
            AssetToResourceMap.Clear();
            SceneToAssetMap.Clear();
        }

        /// <summary>
        /// 加载资源器轮询。
        /// </summary>
        public void Update()
        {
            TaskPool.Update();
        }

        /// <summary>
        /// 设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        private void SetObjectPoolManager()
        {
            AssetPool = GameModule.ObjectPool.CreateObjectPool(typeof(ObjectAsset), "AssetPool");
            ResourcePool = GameModule.ObjectPool.CreateObjectPool(typeof(ObjectAsset), "ResPool");
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasResult HasAsset(string assetName)
        {
            ResInfo resInfo = GameModule.ResUpdater.GetResInfo(assetName);
            if (resInfo == null) return HasResult.NotExist;
            if (!resInfo.Ready) return HasResult.NotReady;
            return resInfo.IsLoadFromBinary ? HasResult.BinaryOnDisk : HasResult.AssetOnDisk;
        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        public string GetBinaryPath(string binaryAssetName)
        {
            ResInfo resInfo = GameModule.ResUpdater.GetResInfo(binaryAssetName);
            if (resInfo == null) return null;
            if (!resInfo.Ready) return null;
            if (!resInfo.IsLoadFromBinary) return null;
            return UtilPath.GetRegularPath(Path.Combine(ResPath, resInfo.ResName.Name));
        }

        /// <summary>
        /// 获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        public int GetBinaryLength(string binaryAssetName)
        {
            ResInfo resInfo = GameModule.ResUpdater.GetResInfo(binaryAssetName);
            if (resInfo == null) return -1;
            if (!resInfo.Ready) return -1;
            if (!resInfo.IsLoadFromBinary) return -1;
            return resInfo.Length;
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
            ResInfo resInfo = null;
            string[] dependAssetNames = null;
            if (!CheckAsset(assetName, out resInfo, out dependAssetNames))
            {
                string errorMessage = UtilText.Format("Can not load asset '{0}'.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, resInfo != null && !resInfo.Ready ? LoadResStatus.NotReady : LoadResStatus.NotExist, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            if (resInfo.IsLoadFromBinary)
            {
                string errorMessage = UtilText.Format("Can not load asset '{0}' which is a binary asset.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.TypeError, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            LoadAssetTask mainTask = LoadAssetTask.Create(assetName, assetType, resInfo, dependAssetNames, loadAssetCallbacks, userData);
            foreach (string dependencyAssetName in dependAssetNames)
            {
                if (!LoadDependencyAsset(dependencyAssetName, mainTask))
                {
                    string errorMessage = UtilText.Format("Can not load dependency asset '{0}' when load asset '{1}'.", dependencyAssetName, assetName);
                    if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    {
                        loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.DependencyError, errorMessage);
                        return;
                    }

                    Log.Fatal(errorMessage);
                    return;
                }
            }

            TaskPool.AddTask(mainTask);
            if (!resInfo.Ready)
            {
                //m_ResourceManager.UpdateResource(resInfo.ResourceName);
            }
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
            ResInfo resInfo = null;
            string[] dependAssetNames = null;
            if (!CheckAsset(sceneAssetName, out resInfo, out dependAssetNames))
            {
                string errorMessage = UtilText.Format("Can not load scene '{0}'.", sceneAssetName);
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, resInfo != null && !resInfo.Ready ? LoadResStatus.NotReady : LoadResStatus.NotExist, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            if (resInfo.IsLoadFromBinary)
            {
                string errorMessage = UtilText.Format("Can not load scene asset '{0}' which is a binary asset.", sceneAssetName);
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.TypeError, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            LoadSceneTask mainTask = LoadSceneTask.Create(sceneAssetName, resInfo, dependAssetNames, loadSceneCallbacks);
            foreach (string dependencyAssetName in dependAssetNames)
            {
                if (!LoadDependencyAsset(dependencyAssetName, mainTask))
                {
                    string errorMessage = UtilText.Format("Can not load dependency asset '{0}' when load scene '{1}'.", dependencyAssetName, sceneAssetName);
                    if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                    {
                        loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.DependencyError, errorMessage);
                        return;
                    }

                    Log.Fatal(errorMessage);
                    return;
                }
            }

            TaskPool.AddTask(mainTask);
            if (!resInfo.Ready)
            {
                //m_ResourceManager.UpdateResource(resInfo.ResourceName);
            }
        }

        public void UnloadScene(string sceneName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            ResHelper.UnloadScene(sceneName, unloadSceneCallbacks);
        }

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            ResInfo resInfo = GameModule.ResUpdater.GetResInfo(binaryAssetName);
            if (resInfo == null)
            {
                string errorMessage = UtilText.Format("Can not load binary '{0}' which is not exist.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotExist, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            if (!resInfo.Ready)
            {
                string errorMessage = UtilText.Format("Can not load binary '{0}' which is not ready.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotReady, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            if (!resInfo.IsLoadFromBinary)
            {
                string errorMessage = UtilText.Format("Can not load binary '{0}' which is not a binary asset.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.TypeError, errorMessage);
                    return;
                }

                Log.Fatal(errorMessage);
                return;
            }

            string path = UtilPath.GetRemotePath(Path.Combine(Application.streamingAssetsPath, resInfo.ResName.Name));
            ResHelper.LoadBinary(path, resInfo, loadBinaryCallbacks);
        }

        /// <summary>
        /// 获取所有加载资源任务的信息。
        /// </summary>
        /// <returns>所有加载资源任务的信息。</returns>
        public TaskInfo[] GetAllLoadAssetInfos()
        {
            return TaskPool.GetAllTaskInfos();
        }

        private bool LoadDependencyAsset(string assetName, LoadResTaskBase mainTask)
        {
            if (mainTask == null)
            {
                Log.Fatal("Main task is invalid.");
                return false;
            }

            ResInfo resInfo = null;
            string[] dependencyAssetNames = null;
            if (!CheckAsset(assetName, out resInfo, out dependencyAssetNames))
            {
                return false;
            }

            if (resInfo.IsLoadFromBinary)
            {
                return false;
            }

            LoadDependAssetTask dependencyTask = LoadDependAssetTask.Create(assetName, resInfo, dependencyAssetNames, mainTask);
            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (!LoadDependencyAsset(dependencyAssetName, dependencyTask))
                {
                    return false;
                }
            }

            TaskPool.AddTask(dependencyTask);
            if (!resInfo.Ready)
            {
                //m_ResourceManager.UpdateResource(resInfo.ResourceName);
            }

            return true;
        }

        private bool CheckAsset(string assetName, out ResInfo resInfo, out string[] dependencyAssetNames)
        {
            resInfo = null;
            dependencyAssetNames = null;

            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            AssetInfo assetInfo = GameModule.ResUpdater.GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return false;
            }

            resInfo = GameModule.ResUpdater.GetResInfo(assetInfo.ResName);
            if (resInfo == null)
            {
                return false;
            }
            dependencyAssetNames = assetInfo.GetDependencyAssetNames();
            return resInfo.Ready;
        }
    }
}
