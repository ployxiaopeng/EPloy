using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;

namespace EPloy
{
    public partial class ResComponent : Component
    {
        private  TaskPool<LoadResTaskBase> TaskPool;
        private  Dictionary<object, int> AssetDependCount;
        private  Dictionary<object, int> ResourceDependCount;
        private  Dictionary<object, object> AssetToResourceMap;
        private  Dictionary<string, object> SceneToAssetMap;
        private  LoadBytesCallbacks LoadBytesCallbacks;
        private  byte[] CachedHashBytes;
        private ObjectPoolBase AssetPool;
        private ObjectPoolBase ResourcePool;

        /// <summary>
        /// 设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        private void SetObjectPoolManager()
        {
            AssetPool = GameEntry.ObjectPool.CreateObjectPool(typeof(AssetObject), "AssetPool");
            ResourcePool = GameEntry.ObjectPool.CreateObjectPool(typeof(AssetObject), "ResPool");
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            ResInfo resInfo = null;
            string[] dependAssetNames = null;
            if (!CheckAsset(assetName, out resInfo, out dependAssetNames))
            {
                string errorMessage = string.Format("Can not load asset '{0}'.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, resInfo != null && !resInfo.Ready ? LoadResStatus.NotReady : LoadResStatus.NotExist, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (resInfo.IsLoadFromBinary)
            {
                string errorMessage = string.Format("Can not load asset '{0}' which is a binary asset.", assetName);
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
                if (!LoadDependencyAsset(dependencyAssetName, priority, mainTask, userData))
                {
                    string errorMessage = string.Format("Can not load dependency asset '{0}' when load asset '{1}'.", dependencyAssetName, assetName);
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
        public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData)
        {
            ResInfo resInfo = null;
            string[] dependAssetNames = null;
            if (!CheckAsset(sceneAssetName, out resInfo, out dependAssetNames))
            {
                string errorMessage = string.Format("Can not load scene '{0}'.", sceneAssetName);
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, resInfo != null && !resInfo.Ready ? LoadResStatus.NotReady : LoadResStatus.NotExist, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (resInfo.IsLoadFromBinary)
            {
                string errorMessage = string.Format("Can not load scene asset '{0}' which is a binary asset.", sceneAssetName);
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
                if (!LoadDependencyAsset(dependencyAssetName, priority, mainTask, userData))
                {
                    string errorMessage = string.Format("Can not load dependency asset '{0}' when load scene '{1}'.", dependencyAssetName, sceneAssetName);
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
            ResInfo resourceInfo = GetResInfo(binaryAssetName);
            if (resourceInfo == null)
            {
                string errorMessage = string.Format("Can not load binary '{0}' which is not exist.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotExist, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (!resourceInfo.Ready)
            {
                string errorMessage = string.Format("Can not load binary '{0}' which is not ready.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.NotReady, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (!resourceInfo.IsLoadFromBinary)
            {
                string errorMessage = string.Format("Can not load binary '{0}' which is not a binary asset.", binaryAssetName);
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                {
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResStatus.TypeError, errorMessage, userData);
                    return;
                }

                throw new EPloyException(errorMessage);
            }

            if (resourceInfo.UseFileSystem)
            {
                //loadBinaryCallbacks.LoadBinarySuccessCallback(binaryAssetName, LoadBinaryFromFileSystem(binaryAssetName), 0f, userData);
            }
            else
            {
                //string path = Utility.Path.GetRemotePath(Path.Combine(resourceInfo.StorageInReadOnly ? m_ResourceManager.m_ReadOnlyPath : m_ResourceManager.m_ReadWritePath, resourceInfo.ResourceName.FullName));
                //m_ResourceManager.m_ResourceHelper.LoadBytes(path, m_LoadBytesCallbacks, LoadBinaryInfo.Create(binaryAssetName, resourceInfo, loadBinaryCallbacks, userData));
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
            ResInfo resInfo = GetResInfo(binaryAssetName);
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

            if (resInfo.UseFileSystem)
            {
                return null;
            }

            return Utility.Path.GetRegularPath(Path.Combine(ReadWritePath, resInfo.ResName.FullName));
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

            ResInfo resInfo = GetResInfo(binaryAssetName);
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
            ResInfo resInfo = GetResInfo(binaryAssetName);
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

        private bool LoadDependencyAsset(string assetName, int priority, LoadResTaskBase mainTask, object userData)
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
                if (!LoadDependencyAsset(dependencyAssetName, priority, dependencyTask, userData))
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

            AssetInfo assetInfo = GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return false;
            }

            resInfo = GetResInfo(assetInfo.ResName);
            if (resInfo == null)
            {
                return false;
            }
            dependencyAssetNames = assetInfo.GetDependencyAssetNames();
            return resInfo.Ready;
        }

        private AssetInfo GetAssetInfo(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new EPloyException("Asset name is invalid.");
            }

            if (m_AssetInfos == null)
            {
                return null;
            }

            AssetInfo assetInfo = null;
            if (m_AssetInfos.TryGetValue(assetName, out assetInfo))
            {
                return assetInfo;
            }

            return null;
        }
        private ResInfo GetResInfo(ResName resName)
        {
            if (m_ResInfos == null)
            {
                return null;
            }

            ResInfo resInfo = null;
            if (m_ResInfos.TryGetValue(resName, out resInfo))
            {
                return resInfo;
            }

            return null;
        }
        private ResInfo GetResInfo(string assetName)
        {
            if (m_ResInfos == null)
            {
                return null;
            }

            AssetInfo assetInfo = GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return null;
            }

            ResInfo resInfo = null;
            if (m_ResInfos.TryGetValue(resInfo.ResName, out resInfo))
            {
                return resInfo;
            }

            return null;
        }
    }
}
