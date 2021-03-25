using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;

namespace EPloy.Res
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public sealed class ResLoader
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

        internal TaskPool<LoadResTaskBase> TaskPool { get; private set; }
        public Dictionary<object, int> AssetDependCount { get; private set; }
        public Dictionary<object, int> ResourceDependCount { get; private set; }
        public Dictionary<object, object> AssetToResourceMap { get; private set; }
        public Dictionary<string, object> SceneToAssetMap { get; private set; }
        public byte[] CachedHashBytes { get; private set; }
        private const int CachedHashBytesLength = 4;

        /// <summary>
        /// 资产路径
        /// </summary>
        internal string ResPath = null;

        private ResLoader()
        {
            TaskPool = new TaskPool<LoadResTaskBase>();
            AssetDependCount = new Dictionary<object, int>();
            ResourceDependCount = new Dictionary<object, int>();
            AssetToResourceMap = new Dictionary<object, object>();
            SceneToAssetMap = new Dictionary<string, object>(StringComparer.Ordinal);
            CachedHashBytes = new byte[CachedHashBytesLength];
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
            //LoadResourceAgent.Clear();
        }

        /// <summary>
        /// 加载资源器轮询。
        /// </summary>
        public void Update()
        {
            TaskPool.Update();
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasResult HasAsset(string assetName)
        {
            ResInfo resInfo =  ResStore.Instance.GetResInfo(assetName);
            if (resInfo == null)
            {
                return HasResult.NotExist;
            }

            if (!resInfo.Ready)
            {
                return HasResult.NotReady;
            }
            return resInfo.IsLoadFromBinary ? HasResult.BinaryOnDisk : HasResult.AssetOnDisk;
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
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, resInfo != null && !resInfo.Ready ? LoadResStatus.NotReady : LoadResStatus.NotExist, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (resInfo.IsLoadFromBinary)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}' which is a binary asset.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.TypeError, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            LoadAssetTask mainTask = LoadAssetTask.Create(assetType, resInfo, dependAssetNames, loadAssetCallbacks, userData);
            foreach (string dependencyAssetName in dependAssetNames)
            {
                if (!LoadDependencyAsset(dependencyAssetName, mainTask, userData))
                {
                    string errorMessage = Utility.Text.Format("Can not load dependency asset '{0}' when load asset '{1}'.", dependencyAssetName, assetName);
                    if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    {
                        loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResStatus.DependencyError, errorMessage, userData);
                        return;
                    }

                    throw new EPloyException(errorMessage);
                }
            }

            TaskPool.AddTask(mainTask);
            if (!resInfo.Ready)
            {
                //m_ResourceManager.UpdateResource(resourceInfo.ResourceName);
            }
        }

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, object userData)
        {
            ResInfo resInfo = null;
            string[] dependAssetNames = null;
            if (!CheckAsset(sceneAssetName, out resInfo, out dependAssetNames))
            {
                string errorMessage = Utility.Text.Format("Can not load scene '{0}'.", sceneAssetName);
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, resInfo != null && !resInfo.Ready ? LoadResStatus.NotReady : LoadResStatus.NotExist, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (resInfo.IsLoadFromBinary)
            {
                string errorMessage = Utility.Text.Format("Can not load scene asset '{0}' which is a binary asset.", sceneAssetName);
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.TypeError, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            LoadSceneTask mainTask = LoadSceneTask.Create(resInfo, dependAssetNames, loadSceneCallbacks, userData);
            foreach (string dependencyAssetName in dependAssetNames)
            {
                if (!LoadDependencyAsset(dependencyAssetName, mainTask, userData))
                {
                    string errorMessage = Utility.Text.Format("Can not load dependency asset '{0}' when load scene '{1}'.", dependencyAssetName, sceneAssetName);
                    if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                    {
                        loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResStatus.DependencyError, errorMessage, userData);
                        return;
                    }

                    throw new EPloyException(errorMessage);
                }
            }

            TaskPool.AddTask(mainTask);
            if (!resInfo.Ready)
            {
                //m_ResourceManager.UpdateResource(resourceInfo.ResourceName);
            }
        }

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks, object userData)
        {
            ResInfo resourceInfo = ResStore.Instance.GetResInfo(binaryAssetName);
            if (resourceInfo == null)
            {
                string errorMessage = Utility.Text.Format("Can not load binary '{0}' which is not exist.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotExist, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (!resourceInfo.Ready)
            {
                string errorMessage = Utility.Text.Format("Can not load binary '{0}' which is not ready.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotReady, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (!resourceInfo.IsLoadFromBinary)
            {
                string errorMessage = Utility.Text.Format("Can not load binary '{0}' which is not a binary asset.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.TypeError, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }


            //string path = Utility.Path.GetRemotePath(Path.Combine(resourceInfo.StorageInReadOnly ? m_ResourceManager.m_ReadOnlyPath : m_ResourceManager.m_ReadWritePath, resourceInfo.ResourceName.FullName));
            //m_ResourceManager.m_ResourceHelper.LoadBytes(path, m_LoadBytesCallbacks, LoadBinaryInfo.Create(binaryAssetName, resourceInfo, loadBinaryCallbacks, userData));

        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        public string GetBinaryPath(string binaryAssetName)
        {
            ResInfo resInfo =  ResStore.Instance.GetResInfo(binaryAssetName);
            if (resInfo == null)
            {
                return null;
            }

            if (!resInfo.Ready)
            {
                return null;
            }

            if (!resInfo.IsLoadFromBinary)
            {
                return null;
            }

            return Utility.Path.GetRegularPath(Path.Combine(ResPath, resInfo.ResName.FullName));
        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <param name="relativePath">二进制资源或存储二进制资源的文件系统，相对于只读区或者读写区的相对路径。</param>
        /// <param name="fileName">若二进制资源存储在文件系统中，则指示二进制资源在文件系统中的名称，否则此参数返回空。</param>
        /// <returns>是否获取二进制资源的实际路径成功。</returns>
        public bool GetBinaryPath(string binaryAssetName, out string relativePath, out string fileName)
        {
            relativePath = null;
            fileName = null;

            ResInfo resInfo =  ResStore.Instance.GetResInfo(binaryAssetName);
            if (resInfo == null)
            {
                return false;
            }

            if (!resInfo.Ready)
            {
                return false;
            }

            if (!resInfo.IsLoadFromBinary)
            {
                return false;
            }
            relativePath = resInfo.ResName.FullName;
            return true;
        }

        /// <summary>
        /// 获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        public int GetBinaryLength(string binaryAssetName)
        {
            ResInfo resInfo =  ResStore.Instance.GetResInfo(binaryAssetName);
            if (resInfo == null)
            {
                return -1;
            }

            if (!resInfo.Ready)
            {
                return -1;
            }

            if (!resInfo.IsLoadFromBinary)
            {
                return -1;
            }

            return resInfo.Length;
        }

        /// <summary>
        /// 获取所有加载资源任务的信息。
        /// </summary>
        /// <returns>所有加载资源任务的信息。</returns>
        public TaskInfo[] GetAllLoadAssetInfos()
        {
            return TaskPool.GetAllTaskInfos();
        }

        private bool LoadDependencyAsset(string assetName, LoadResTaskBase mainTask, object userData)
        {
            if (mainTask == null)
            {
                throw new EPloyException("Main task is invalid.");
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

            LoadDependAssetTask dependencyTask = LoadDependAssetTask.Create(resInfo, dependencyAssetNames, mainTask, userData);
            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (!LoadDependencyAsset(dependencyAssetName, dependencyTask, userData))
                {
                    return false;
                }
            }

            TaskPool.AddTask(dependencyTask);
            if (!resInfo.Ready)
            {
                //m_ResourceManager.UpdateResource(resourceInfo.ResourceName);
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

            AssetInfo assetInfo =  ResStore.Instance.GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return false;
            }

            resInfo =  ResStore.Instance.GetResInfo(assetInfo.ResName);
            if (resInfo == null)
            {
                return false;
            }
            dependencyAssetNames = assetInfo.GetDependencyAssetNames();
            return resInfo.Ready;
        }
    }
}
