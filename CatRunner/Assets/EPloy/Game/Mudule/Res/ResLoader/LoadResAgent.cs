using System;
using System.Collections.Generic;
using System.IO;
using EPloy.Game.Obj;
using EPloy.Game.ObjectPool;
using EPloy.Game.TaskPool;

namespace EPloy.Game.Res
{
   
    /// <summary>
    /// 加载资源代理 信息存储的基类是LoadResTaskBase
    /// </summary>
    internal sealed partial class LoadResAgent : ITaskAgent<LoadResTaskBase>
    {
        private static readonly Dictionary<string, string> s_CachedResNames = new Dictionary<string, string>(StringComparer.Ordinal);
        //这个是加载的资源包里面的
        private static readonly HashSet<string> s_LoadingAssetNames = new HashSet<string>(StringComparer.Ordinal);
        //这个是加载的资源包/ 资产
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
                //资源包已经加载了
                ObjectBase assetObject = ResLoader.Instance.AssetPool.Spawn(Task.AssetName);
                if (assetObject != null)
                {
                    OnAssetReady(assetObject);
                    return StartTaskStatus.Done;
                }
            }

            foreach (string dependencyAssetName in Task.DependAssetsName)
            {
                if (!ResLoader.Instance.AssetPool.CanSpawn(dependencyAssetName))
                {
                    Task.StartTime = default(DateTime);
                    //Log.Error("不纯在依赖的" + dependencyAssetName);
                    return StartTaskStatus.HasToWait;
                }
            }

            if (IsResLoading(resInfo.ResName.Name))
            {
                Task.StartTime = default(DateTime);
                return StartTaskStatus.HasToWait;
            }
            //Log.Error("开始加载: " + Task.AssetName);
            //加入资源加载列表
            s_LoadingAssetNames.Add(Task.AssetName);
            ObjectBase resObject = ResLoader.Instance.ResourcePool.Spawn(resInfo.ResName.Name);
            if (resObject != null)
            {
                OnResObjectReady((ObjectRes)resObject);
                return StartTaskStatus.CanResume;
            }
            //加入资产加载列表
            s_LoadingResNames.Add(resInfo.ResName.Name);

            string fullPath = null;
            if (!s_CachedResNames.TryGetValue(Task.AssetName, out fullPath))
            {
                fullPath = UtilPath.GetRegularPath(Path.Combine(ResPath, resInfo.ResName.Name));
                s_CachedResNames.Add(Task.AssetName, fullPath);
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
           // Log.Error("加载成功  " + assetObject.Name);
            Task.OnLoadAssetSuccess(this, asset, (float)(DateTime.Now - Task.StartTime).TotalSeconds);
            Task.Done = true;
        }

        // 所有依赖资源加载完毕  加载主资源
        private void OnResObjectReady(ObjectRes objectRes)
        {
            Task.SetResObject(objectRes);
            LoadAsset(objectRes.Target, Task.AssetName.ToLowerInvariant(), Task.AssetType, Task.IsScene);
        }

        private void OnComplete(object asset)
        {
            ObjectBase assetObject = null;
            if (Task.IsScene) assetObject = ResLoader.Instance.AssetPool.Spawn(Task.AssetName);

            if (assetObject == null)
            {
                List<object> dependencyAssets = Task.DependAssets;
                assetObject = ObjectAsset.Create(Task.AssetName, asset, dependencyAssets, Task.ObjectRes.Target);
                ResLoader.Instance.AssetPool.Register(assetObject, true);
                ResLoader.Instance.AssetToResourceMap.Add(asset, Task.ObjectRes.Target);

                foreach (object dependencyAsset in dependencyAssets)
                {
                    object dependencyResource = null;
                    if (ResLoader.Instance.AssetToResourceMap.TryGetValue(dependencyAsset, out dependencyResource))
                    {
                        Task.ObjectRes.AddDependencyResource(dependencyResource);
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
            ObjectRes objectRes = ObjectRes.Create(Task.ResInfo.ResName.Name, res);
            ResLoader.Instance.ResourcePool.Register(objectRes, true);
            s_LoadingResNames.Remove(Task.ResInfo.ResName.Name);
            OnResObjectReady(objectRes);
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
            ResLoader.Instance.ResHelper.DecryptResCallback(bytes,resInfo.LoadType,resInfo.HashCode);
            ParseBytes(bytes);
        }

        private void OnParseBytesComplete(object asset)
        {
            ObjectRes objectRes = ObjectRes.Create(Task.AssetName, asset);
            ResLoader.Instance.ResourcePool.Register(objectRes, true);
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
