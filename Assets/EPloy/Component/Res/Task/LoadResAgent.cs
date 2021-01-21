using System;
using System.Collections.Generic;
using System.IO;
using EPloy.TaskPool;

namespace EPloy
{
    /// <summary>
    /// 加载资源状态。
    /// </summary>
    public enum LoadResStatus : byte
    {
        /// <summary>
        /// 加载资源完成。
        /// </summary>
        Success = 0,

        /// <summary>
        /// 资源不存在。
        /// </summary>
        NotExist,

        /// <summary>
        /// 资源尚未准备完毕。
        /// </summary>
        NotReady,

        /// <summary>
        /// 依赖资源错误。
        /// </summary>
        DependencyError,

        /// <summary>
        /// 资源类型错误。
        /// </summary>
        TypeError,

        /// <summary>
        /// 加载资源错误。
        /// </summary>
        AssetError
    }

    /// <summary>
    /// 加载资源进度类型。
    /// </summary>
    public enum LoadResProgress : byte
    {
        /// <summary>
        /// 未知类型。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 读取资源包。
        /// </summary>
        ReadRes,

        /// <summary>
        /// 加载资源包。
        /// </summary>
        LoadRes,

        /// <summary>
        /// 加载资源。
        /// </summary>
        LoadAsset,

        /// <summary>
        /// 加载场景。
        /// </summary>
        LoadScene
    }


    /// <summary>
    /// 解密资源回调函数。
    /// </summary>
    /// <param name="bytes">要解密的资源二进制流。</param>
    /// <param name="startIndex">解密二进制流的起始位置。</param>
    /// <param name="count">解密二进制流的长度。</param>
    /// <param name="name">资源名称。</param>
    /// <param name="variant">变体名称。</param>
    /// <param name="extension">扩展名称。</param>
    /// <param name="fileSystem">文件系统名称。</param>
    /// <param name="loadType">资源加载方式。</param>
    /// <param name="length">资源大小。</param>
    /// <param name="hashCode">资源哈希值。</param>
    public delegate void DecryptResCallback(byte[] bytes, int startIndex, int count, string name, string variant, string extension,string fileSystem, byte loadType, int length, int hashCode);

    /// <summary>
    /// 加载资源代理。
    /// </summary>
    internal sealed partial class LoadResAgent : ITaskAgent<LoadResTaskBase>
    {
        private static readonly Dictionary<string, string> s_CachedResNames = new Dictionary<string, string>(StringComparer.Ordinal);
        private static readonly HashSet<string> s_LoadingAssetNames = new HashSet<string>(StringComparer.Ordinal);
        private static readonly HashSet<string> s_LoadingResNames = new HashSet<string>(StringComparer.Ordinal);

        public string ReadWritePath { get; private set; }
        public DecryptResCallback DecryptResCallback { get; private set; }
        /// <summary>
        /// 获取加载资源任务。
        /// </summary>
        public LoadResTaskBase Task { get; private set; }

        /// <summary>
        /// 初始化加载资源代理
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        /// <param name="decryptResourceCallback">解密资源回调函数。</param>
        public void Initialize( string readWritePath, DecryptResCallback decryptResCallback)
        {
            ReadWritePath = readWritePath;
            DecryptResCallback = decryptResCallback ?? throw new EPloyException("Decrypt resource callback is invalid.");
            Task = null;
        }

        /// <summary>
        /// 加载资源代理轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update()
        {
        }

        /// <summary>
        /// 关闭并清理加载资源代理。
        /// </summary>
        public void OnDestroy()
        {
            Reset();
        }

        public static void Clear()
        {
            s_CachedResNames.Clear();
            s_LoadingAssetNames.Clear();
            s_LoadingResNames.Clear();
        }

        /// <summary>
        /// 开始处理加载资源任务。
        /// </summary>
        /// <param name="task">要处理的加载资源任务。</param>
        /// <returns>开始处理任务的状态。</returns>
        public StartTaskStatus Start(LoadResTaskBase task)
        {
            Task = task ?? throw new EPloyException("Task is invalid.");
            Task.StartTime = DateTime.Now;
            ResInfo resInfo = Task.ResInfo;

            if (!resInfo.Ready)
            {
                Task.StartTime = default(DateTime);
                return StartTaskStatus.HasToWait;
            }

            if (IsAssetLoading(Task.AssetName))
            {
                Task.StartTime = default(DateTime);
                return StartTaskStatus.HasToWait;
            }

            if (!Task.IsScene)
            {
                AssetObject assetObject = m_ResourceLoader.m_AssetPool.Spawn(Task.AssetName);
                if (assetObject != null)
                {
                    OnAssetObjectReady(assetObject);
                    return StartTaskStatus.Done;
                }
            }

            foreach (string dependencyAssetName in Task.GetDependencyAssetNames())
            {
                if (!m_ResourceLoader.m_AssetPool.CanSpawn(dependencyAssetName))
                {
                    Task.StartTime = default(DateTime);
                    return StartTaskStatus.HasToWait;
                }
            }

            string resName = resInfo.ResName.Name;
            if (IsResourceLoading(resName))
            {
                m_Task.StartTime = default(DateTime);
                return StartTaskStatus.HasToWait;
            }

            s_LoadingAssetNames.Add(m_Task.AssetName);

            ResObject resourceObject = m_ResourceLoader.m_ResourcePool.Spawn(resourceName);
            if (resourceObject != null)
            {
                OnResourceObjectReady(resourceObject);
                return StartTaskStatus.CanResume;
            }

            s_LoadingResNames.Add(resourceName);

            string fullPath = null;
            if (!s_CachedResNames.TryGetValue(resourceName, out fullPath))
            {
                fullPath = Utility.Path.GetRegularPath(Path.Combine(resourceInfo.StorageInReadOnly ? m_ReadOnlyPath : m_ReadWritePath, resourceInfo.UseFileSystem ? resourceInfo.FileSystemName : resourceInfo.ResourceName.FullName));
                s_CachedResNames.Add(resourceName, fullPath);
            }

            if (resourceInfo.LoadType == LoadType.LoadFromFile)
            {
                if (resourceInfo.UseFileSystem)
                {
                    IFileSystem fileSystem = m_ResourceLoader.m_ResourceManager.GetFileSystem(resourceInfo.FileSystemName, resourceInfo.StorageInReadOnly);
                    m_Helper.ReadFile(fileSystem, resourceInfo.ResourceName.FullName);
                }
                else
                {
                    m_Helper.ReadFile(fullPath);
                }
            }
            else if (resourceInfo.LoadType == LoadType.LoadFromMemory || resourceInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || resourceInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt)
            {
                if (resourceInfo.UseFileSystem)
                {
                    IFileSystem fileSystem = m_ResourceLoader.m_ResourceManager.GetFileSystem(resourceInfo.FileSystemName, resourceInfo.StorageInReadOnly);
                    m_Helper.ReadBytes(fileSystem, resourceInfo.ResourceName.FullName);
                }
                else
                {
                    m_Helper.ReadBytes(fullPath);
                }
            }
            else
            {
                throw new EPloyException(string.Format("Resource load type '{0}' is not supported.", resourceInfo.LoadType.ToString()));
            }

            return StartTaskStatus.CanResume;
        }

        /// <summary>
        /// 重置加载资源代理。
        /// </summary>
        public void Reset()
        {
            Helper.Reset();
            Task = null;
        }

        private static bool IsAssetLoading(string assetName)
        {
            return s_LoadingAssetNames.Contains(assetName);
        }

        private static bool IsResLoading(string resName)
        {
            return s_LoadingResNames.Contains(resName);
        }

        private void OnAssetObjectReady(AssetObject assetObject)
        {
            object asset = assetObject.Target;
            if (Task.IsScene)
            {
                m_ResourceLoader.m_SceneToAssetMap.Add(m_Task.AssetName, asset);
            }

            Task.OnLoadAssetSuccess(this, asset, (float)(DateTime.Now - m_Task.StartTime).TotalSeconds);
            Task.Done = true;
        }

        private void OnResourceObjectReady(ResObject resourceObject)
        {
            Task.LoadAsset(this, resourceObject);
        }

        private void OnError(LoadResStatus status, string errorMessage)
        {
            m_Helper.Reset();
            Task.OnLoadAssetFailure(this, status, errorMessage);
            s_LoadingAssetNames.Remove(Task.AssetName);
            s_LoadingResNames.Remove(Task.ResInfo.ResName.Name);
            Task.Done = true;
        }

        private void OnLoadResourceAgentHelperUpdate(object sender, LoadResourceAgentHelperUpdateEventArgs e)
        {
            Task.OnLoadAssetUpdate(this, e.Type, e.Progress);
        }

        private void OnLoadResourceAgentHelperReadFileComplete(object sender, LoadResourceAgentHelperReadFileCompleteEventArgs e)
        {
            ResObject resourceObject = ResObject.Create(m_Task.ResourceInfo.ResourceName.Name, e.Resource, m_ResourceHelper, m_ResourceLoader);
            m_ResourceLoader.m_ResourcePool.Register(resourceObject, true);
            s_LoadingResNames.Remove(m_Task.ResourceInfo.ResourceName.Name);
            OnResourceObjectReady(resourceObject);
        }

        private void OnLoadResourceAgentHelperReadBytesComplete(object sender, LoadResourceAgentHelperReadBytesCompleteEventArgs e)
        {
            byte[] bytes = e.GetBytes();
            ResourceInfo resourceInfo = m_Task.ResourceInfo;
            if (resourceInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || resourceInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt)
            {
                m_DecryptResourceCallback(bytes, 0, bytes.Length, resourceInfo.ResourceName.Name, resourceInfo.ResourceName.Variant, resourceInfo.ResourceName.Extension, resourceInfo.StorageInReadOnly, resourceInfo.FileSystemName, (byte)resourceInfo.LoadType, resourceInfo.Length, resourceInfo.HashCode);
            }

            m_Helper.ParseBytes(bytes);
        }

        private void OnLoadResourceAgentHelperParseBytesComplete(object sender, LoadResourceAgentHelperParseBytesCompleteEventArgs e)
        {
            ResObject resourceObject = ResObject.Create(m_Task.ResourceInfo.ResourceName.Name, e.Resource, m_ResourceHelper, m_ResourceLoader);
            m_ResourceLoader.m_ResourcePool.Register(resourceObject, true);
            s_LoadingResNames.Remove(m_Task.ResourceInfo.ResourceName.Name);
            OnResourceObjectReady(resourceObject);
        }

        private void OnLoadResourceAgentHelperLoadComplete(object sender, LoadResourceAgentHelperLoadCompleteEventArgs e)
        {
            AssetObject assetObject = null;
            if (m_Task.IsScene)
            {
                assetObject = m_ResourceLoader.m_AssetPool.Spawn(m_Task.AssetName);
            }

            if (assetObject == null)
            {
                List<object> dependencyAssets = m_Task.GetDependencyAssets();
                assetObject = AssetObject.Create(m_Task.AssetName, e.Asset, dependencyAssets, m_Task.ResourceObject.Target, m_ResourceHelper, m_ResourceLoader);
                m_ResourceLoader.m_AssetPool.Register(assetObject, true);
                m_ResourceLoader.m_AssetToResourceMap.Add(e.Asset, m_Task.ResourceObject.Target);
                foreach (object dependencyAsset in dependencyAssets)
                {
                    object dependencyResource = null;
                    if (m_ResourceLoader.m_AssetToResourceMap.TryGetValue(dependencyAsset, out dependencyResource))
                    {
                        m_Task.ResourceObject.AddDependencyResource(dependencyResource);
                    }
                    else
                    {
                        throw new GameFrameworkException("Can not find dependency resource.");
                    }
                }
            }

            s_LoadingAssetNames.Remove(m_Task.AssetName);
            OnAssetObjectReady(assetObject);
        }

        private void OnLoadResourceAgentHelperError(object sender, LoadResourceAgentHelperErrorEventArgs e)
        {
            OnError(e.Status, e.ErrorMessage);
        }
    }
}

