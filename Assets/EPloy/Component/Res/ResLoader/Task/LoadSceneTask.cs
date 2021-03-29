
namespace EPloy.Res
{
    internal sealed class LoadSceneTask : LoadResTaskBase
    {
        private LoadSceneCallbacks m_LoadSceneCallbacks;

        public override bool IsScene
        {
            get
            {
                return true;
            }
        }

        public static LoadSceneTask Create(ResInfo resInfo, string[] dependAssetNames, LoadSceneCallbacks loadSceneCallbacks)
        {
            LoadSceneTask loadSceneTask = ReferencePool.Acquire<LoadSceneTask>();
            loadSceneTask.Initialize(null, resInfo, dependAssetNames);
            loadSceneTask.m_LoadSceneCallbacks = loadSceneCallbacks;
            return loadSceneTask;
        }

        public override void Clear()
        {
            base.Clear();
            m_LoadSceneCallbacks = null;
        }

        public override void OnLoadAssetSuccess(LoadResAgent agent, object asset, float duration)
        {
            base.OnLoadAssetSuccess(agent, asset, duration);
            if (m_LoadSceneCallbacks.LoadSceneSuccessCallback != null)
            {
                m_LoadSceneCallbacks.LoadSceneSuccessCallback(AssetName, duration);
            }
        }

        public override void OnLoadAssetFailure(LoadResAgent agent, LoadResStatus status, string errorMessage)
        {
            base.OnLoadAssetFailure(agent, status, errorMessage);
            if (m_LoadSceneCallbacks.LoadSceneFailureCallback != null)
            {
                m_LoadSceneCallbacks.LoadSceneFailureCallback(AssetName, status, errorMessage);
            }
        }

        public override void OnLoadDependAsset(LoadResAgent agent, string dependAssetName, object dependAsset)
        {
            base.OnLoadDependAsset(agent, dependAssetName, dependAsset);
            if (m_LoadSceneCallbacks.LoadSceneDepenAssetCallback != null)
            {
                m_LoadSceneCallbacks.LoadSceneDepenAssetCallback(AssetName, dependAssetName, DependAssets.Count, TotalDependAssetCount);
            }
        }
    }
}
