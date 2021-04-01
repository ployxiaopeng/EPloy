using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;
using UnityEngine.Networking;

namespace EPloy.Res
{
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
    public delegate void DecryptResCallback(byte[] bytes, int startIndex, int count, string name, string variant, string extension, byte loadType, int length, int hashCode);

    /// <summary>
    /// 加载资源代理 信息存储的基类是LoadResTaskBase
    /// </summary>
    internal sealed partial class LoadResAgent : ITaskAgent<LoadResTaskBase>
    {
        private static readonly Dictionary<string, string> s_CachedResNames = new Dictionary<string, string>(StringComparer.Ordinal);
        private static readonly HashSet<string> s_LoadingAssetNames = new HashSet<string>(StringComparer.Ordinal);
        private static readonly HashSet<string> s_LoadingResNames = new HashSet<string>(StringComparer.Ordinal);

        public LoadResTaskBase Task { get; private set; }
        private string ResPath;

        /// <summary>
        /// 初始化加载资源代理
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        /// <param name="decryptResourceCallback">解密资源回调函数。</param>
        public void Initialize(string resPath)
        {
            ResPath = resPath;
            Task = null;
        }

        /// <summary>
        /// 开始处理加载资源任务。
        /// </summary>
        /// <param name="task">要处理的加载资源任务。</param>
        /// <returns>开始处理任务的状态。</returns>
        public StartTaskStatus Start(LoadResTaskBase task)
        {
            Task = task ?? throw new Exception("Task is invalid.");
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
                ObjectBase assetObject = ResStore.Instance.AssetPool.Spawn(Task.AssetName);
                if (assetObject != null)
                {
                    OnAssetReady(assetObject);
                    return StartTaskStatus.Done;
                }
            }

            foreach (string dependencyAssetName in Task.DependAssetsName)
            {
                if (!ResStore.Instance.AssetPool.CanSpawn(dependencyAssetName))
                {
                    Task.StartTime = default(DateTime);
                    return StartTaskStatus.HasToWait;
                }
            }

            string resName = resInfo.ResName.Name;
            if (IsResLoading(resName))
            {
                Task.StartTime = default(DateTime);
                return StartTaskStatus.HasToWait;
            }

            s_LoadingAssetNames.Add(Task.AssetName);

            ObjectBase resObject = ResStore.Instance.ResourcePool.Spawn(resName);
            if (resObject != null)
            {
                return StartTaskStatus.CanResume;
            }

            s_LoadingResNames.Add(resName);

            string fullPath = null;
            if (!s_CachedResNames.TryGetValue(resName, out fullPath))
            {
                fullPath = Utility.Path.GetRegularPath(Path.Combine(ResPath, resInfo.ResName.FullName));
                s_CachedResNames.Add(resName, fullPath);
            }

            switch (resInfo.LoadType)
            {
                case LoadType.LoadFromFile:
                    ReadFile(fullPath);
                    break;
                case LoadType.LoadFromMemory:
                    ReadBytes(fullPath);
                    break;
                case LoadType.LoadFromBinary:
                    ReadBytes(fullPath);
                    break;
            }

            return StartTaskStatus.CanResume;
        }

        private static bool IsAssetLoading(string assetName)
        {
            return s_LoadingAssetNames.Contains(assetName);
        }

        private static bool IsResLoading(string resName)
        {
            return s_LoadingResNames.Contains(resName);
        }

        private void OnAssetReady(ObjectBase assetObject)
        {
            object asset = assetObject.Target;
            if (Task.IsScene)
            {
                ResLoader.Instance.SceneToAssetMap.Add(Task.AssetName, asset);
            }

            Task.OnLoadAssetSuccess(this, asset, (float)(DateTime.Now - Task.StartTime).TotalSeconds);
            Task.Done = true;
        }

        // 所有依赖资源加载完毕  加载主资源
        private void OnResObjectReady(ResObject resObject)
        {
            Task.SetResObject(resObject);
            LoadAsset(resObject.Target, Task.AssetName, Task.AssetType, Task.IsScene);
        }

        private void OnComplete(object asset)
        {
            ObjectBase assetObject = null;
            if (Task.IsScene)
            {
                assetObject = ResStore.Instance.AssetPool.Spawn(Task.AssetName);
            }

            if (assetObject == null)
            {
                List<object> dependencyAssets = Task.DependAssets;
                assetObject = AssetObject.Create(Task.AssetName, asset, dependencyAssets, Task.ResObject.Target);
                ResStore.Instance.AssetPool.Register(assetObject, true);
                ResLoader.Instance.AssetToResourceMap.Add(asset, Task.ResObject.Target);
                foreach (object dependencyAsset in dependencyAssets)
                {
                    object dependencyResource = null;
                    if (ResLoader.Instance.AssetToResourceMap.TryGetValue(dependencyAsset, out dependencyResource))
                    {
                        Task.ResObject.AddDependencyResource(dependencyResource);
                    }
                    else
                    {
                        Log.Fatal("Can not find dependency resource.");
                    }
                }
            }

            s_LoadingAssetNames.Remove(Task.AssetName);
            OnAssetReady(assetObject);
        }

        private void OnReadFileComplete(object res)
        {
            ResObject resObject = ResObject.Create(Task.AssetName, res);
            ResStore.Instance.ResourcePool.Register(resObject, true);
            s_LoadingAssetNames.Remove(Task.AssetName);
            OnResObjectReady(resObject);
        }

        private void OnError(LoadResStatus status, string errorMessage)
        {
            Task.OnLoadAssetFailure(this, status, errorMessage);
            s_LoadingAssetNames.Remove(Task.AssetName);
            s_LoadingResNames.Remove(Task.ResInfo.ResName.Name);
            Task.Done = true;
        }

        private void OnReadBytesComplete(byte[] bytes)
        {
            ResInfo resInfo = Task.ResInfo;
            ResLoader.Instance.DecryptResCallback(bytes, 0, bytes.Length, resInfo.ResName.Name, resInfo.ResName.Variant,
            resInfo.ResName.Extension, resInfo.FileSystemName, (byte)resInfo.LoadType, resInfo.Length, resInfo.HashCode);
            ParseBytes(bytes);
        }

        private void OnParseBytesComplete(object asset)
        {
            ResObject resObject = ResObject.Create(Task.AssetName, asset);
            ResStore.Instance.ResourcePool.Register(resObject, true);
            s_LoadingResNames.Remove(Task.AssetName);
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
    }
}
