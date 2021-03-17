namespace EPloy.Res
{
    /// <summary>
    /// 加载依赖资源任务 信息
    /// </summary>
    internal sealed class LoadDependAssetTask : LoadResTaskBase
    {
        private LoadResTaskBase m_MainTask;

        public LoadDependAssetTask()
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

        public static LoadDependAssetTask Create(ResInfo resInfo, string[] dependencyAssetNames, LoadResTaskBase mainTask, object userData)
        {
            LoadDependAssetTask loadDependencyAssetTask = ReferencePool.Acquire<LoadDependAssetTask>();
            loadDependencyAssetTask.Initialize(null, resInfo, dependencyAssetNames, userData);
            loadDependencyAssetTask.m_MainTask = mainTask;
            loadDependencyAssetTask.m_MainTask.TotalDependAssetCount++;
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
            m_MainTask.OnLoadDependAsset(agent, AssetName, asset);
        }

        public override void OnLoadAssetFailure(LoadResAgent agent, LoadResStatus status, string errorMessage)
        {
            base.OnLoadAssetFailure(agent, status, errorMessage);
            m_MainTask.OnLoadAssetFailure(agent, LoadResStatus.DependencyError,
                Utility.Text.Format("Can not load dependency asset '{0}', internal status '{1}', internal error message '{2}'.",
                AssetName, status.ToString(), errorMessage));
        }
    }
}