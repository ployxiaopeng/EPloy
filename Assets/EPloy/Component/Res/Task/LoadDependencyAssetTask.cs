namespace EPloy
{
    internal sealed class LoadDependencyAssetTask : LoadResTaskbase
    {
        private LoadResTaskbase m_MainTask;

        public LoadDependencyAssetTask()
        {
            m_MainTask = null;
        }

        public override bool IsScene
        {
            get
            {
                return false;
            }
        }

        public static LoadDependencyAssetTask Create(string assetName, ResInfo resInfo, string[] dependencyAssetNames, LoadResTaskbase mainTask, object userData)
        {
            LoadDependencyAssetTask loadDependencyAssetTask = ReferencePool.Acquire<LoadDependencyAssetTask>();
            loadDependencyAssetTask.Initialize(assetName, null, resInfo, dependencyAssetNames, userData);
            loadDependencyAssetTask.m_MainTask = mainTask;
            loadDependencyAssetTask.m_MainTask.TotalDependencyAssetCount++;
            return loadDependencyAssetTask;
        }

        public override void Clear()
        {
            base.Clear();
            m_MainTask = null;
        }

        public override void OnLoadAssetSuccess(LoadResAgent agent, object asset, float duration)
        {
            base.OnLoadAssetSuccess(agent, asset, duration);
            m_MainTask.OnLoadDependencyAsset(agent, AssetName, asset);
        }

        public override void OnLoadAssetFailure(LoadResAgent agent, LoadResourceStatus status, string errorMessage)
        {
            base.OnLoadAssetFailure(agent, status, errorMessage);
            m_MainTask.OnLoadAssetFailure(agent, LoadResourceStatus.DependencyError, Utility.Text.Format("Can not load dependency asset '{0}', internal status '{1}', internal error message '{2}'.", AssetName, status.ToString(), errorMessage));
        }
    }
}

