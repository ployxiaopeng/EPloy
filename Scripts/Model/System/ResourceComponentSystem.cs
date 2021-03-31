using GameFramework;
using GameFramework.Download;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class ResourceComponentAwakeSystem : AwakeSystem<ResourceComponent>
    {
        public override void Awake(ResourceComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class ResourceComponentStartSystem : StartSystem<ResourceComponent>
    {
        public override void Start(ResourceComponent self)
        {
            self.Start();
        }
    }
    [ObjectSystem]
    public class ResourceComponentUpdateSystem : UpdateSystem<ResourceComponent>
    {
        public override void Update(ResourceComponent self)
        {
            self.Update();
        }
    }

    public static class ResourceComponentSystem
    {
        #region 资源更新
        //资源热更URL
        private static string UpdatePrefixUri;
        //更新文件缓存大小
        private static int UpdateFileCacheLength;
        //每更新多少缓存刷新一次
        private static int GenerateReadWriteListLength;
        //资源更新失败重试次数
        private static int UpdateRetryCount;
        #endregion

        #region 对象池设置
        //对象池容量
        private static int AssetCapacity;
        //无用资源释放间隔时间。
        private static float UnloadUnusedAssetsInterval;
        //资源过期事件
        private static float AssetExpireTime;
        //资源对象池优先级
        private static int AssetPriority;
        //资源对象池检擦是否可释放间隔
        private static int AssetAutoReleaseInterval;
        //资源对象池容量
        private static int ResourceCapacity;
        //资源对象池优先级
        private static int ResourcePriority;
        //资源过期事件
        private static float ResourceExpireTime;
        //资源对象池检擦是否可释放间隔
        private static float ResourceAutoReleaseInterval;
        #endregion
        public static void Awake(this ResourceComponent self)
        {
            UpdatePrefixUri = null; UpdateFileCacheLength = 1024 * 1024;
            GenerateReadWriteListLength = 1024 * 1024;
            UpdateRetryCount = 3;

            ResourceCapacity = AssetCapacity = 64;
            ResourceAutoReleaseInterval = UnloadUnusedAssetsInterval = 600f;
            AssetExpireTime = ResourceExpireTime = 600f;
            AssetPriority = ResourcePriority = 1000;
        }
        public static void Start(this ResourceComponent self)
        {
            self.ResourceManager = Init.Base.EditorResourceMode ? new EditorResourceComponent()
                : GameFrameworkEntry.GetModule<IResourceManager>();
            if (self.ResourceManager == null)
            {
                Log.Fatal("Resource manager is invalid.");
                return;
            }
            self.ResourceManager.SetReadOnlyPath(Application.streamingAssetsPath);
            if (self.ReadWritePathType == ReadWritePathType.TemporaryCache)
            {
                self.ResourceManager.SetReadWritePath(Application.temporaryCachePath);
            }
            else self.ResourceManager.SetReadWritePath(Application.persistentDataPath);

            if (Init.Base.EditorResourceMode) return;

            self.SetResourceMode(self.m_ResourceMode);
            self.ResourceManager.SetDownloadManager(Init.Download.DownloadManager);
            self.ResourceManager.SetObjectPoolManager(Init.ObjectPool.ObjectPoolManager);
            self.ResourceManager.AssetAutoReleaseInterval = AssetAutoReleaseInterval;
            self.ResourceManager.AssetCapacity = AssetCapacity;
            self.ResourceManager.AssetExpireTime = AssetExpireTime;
            self.ResourceManager.AssetPriority = AssetPriority;

            self.ResourceManager.ResourceAutoReleaseInterval = ResourceAutoReleaseInterval;
            self.ResourceManager.ResourceCapacity = ResourceCapacity;
            self.ResourceManager.ResourceExpireTime = ResourceExpireTime;
            self.ResourceManager.ResourcePriority = ResourcePriority;
            if (self.m_ResourceMode == ResourceMode.Updatable)
            {
                self.ResourceManager.UpdatePrefixUri = UpdatePrefixUri;
                self.ResourceManager.UpdateFileCacheLength = UpdateFileCacheLength;
                self.ResourceManager.GenerateReadWriteListLength = GenerateReadWriteListLength;
                self.ResourceManager.UpdateRetryCount = UpdateRetryCount;
            }
            self.ResourceManager.SetResourceHelper(new ResourceHelper());
            self.loadResourceAgents = new LoadResourceAgentHelper[self.m_LoadResourceAgentHelperCount];
            for (int i = 0; i < self.m_LoadResourceAgentHelperCount; i++)
                self.AddLoadResourceAgentHelper(i);
        }

        public static float LastOperationElapse = 0f;
        public static void Update(this ResourceComponent self)
        {
            if (Init.Base.EditorResourceMode)
            {
                EditorResourceComponent editorResource = (EditorResourceComponent)self.ResourceManager;
                editorResource.Update();
                return;
            }

            LastOperationElapse += Time.unscaledDeltaTime;
            if (self.m_AsyncOperation == null && (self.m_ForceUnloadUnusedAssets || self.m_PreorderUnloadUnusedAssets &&
                LastOperationElapse >= UnloadUnusedAssetsInterval))
            {
                Log.Info("Unload unused assets...");
                self.m_ForceUnloadUnusedAssets = false;
                self.m_PreorderUnloadUnusedAssets = false;
                LastOperationElapse = 0f;
                self.m_AsyncOperation = Resources.UnloadUnusedAssets();
            }

            if (self.m_AsyncOperation != null && self.m_AsyncOperation.isDone)
            {
                self.m_AsyncOperation = null;
                if (self.m_PerformGCCollect)
                {
                    Log.Info("GC.Collect...");
                    self.m_PerformGCCollect = false;
                    GC.Collect();
                }
            }
            foreach (var resourceAgent in self.loadResourceAgents)
                resourceAgent.Update();
        }

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static async Task<T> AwaitLoadAsset<T>(this ResourceComponent self, string assetName)
        {
            object asset = await self.AwaitInternalLoadAsset<T>(assetName);
            return (T)asset;
        }
        private static TaskCompletionSource<object> s_LoadAssetTcs;
        private static LoadAssetCallbacks s_LoadAssetCallbacks;
        private static Task<object> AwaitInternalLoadAsset<T>(this ResourceComponent self, string assetName)
        {
            if (s_LoadAssetCallbacks == null) s_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
            s_LoadAssetTcs = new TaskCompletionSource<object>();
            self.LoadAsset(assetName, typeof(T), s_LoadAssetCallbacks);
            return s_LoadAssetTcs.Task;
        }

        private static void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            s_LoadAssetTcs.SetResult(asset);
        }
        private static void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            s_LoadAssetTcs.SetException(new GameFrameworkException(errorMessage));
        }
    }
}