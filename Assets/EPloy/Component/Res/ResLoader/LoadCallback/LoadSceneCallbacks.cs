
namespace EPloy.Res
{
    /// <summary>
    /// 加载场景成功回调函数。
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadSceneSuccessCallback(string sceneAssetName, float duration, object userData);

    /// <summary>
    /// 加载场景失败回调函数。
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称。</param>
    /// <param name="status">加载场景状态。</param>
    /// <param name="errorMessage">错误信息。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadSceneFailureCallback(string sceneAssetName, LoadResStatus status, string errorMessage, object userData);

    /// <summary>
    /// 加载场景时加载依赖资源回调函数。
    /// </summary>
    /// <param name="sceneAssetName">要加载的场景资源名称。</param>
    /// <param name="dependAssetName">被加载的依赖资源名称。</param>
    /// <param name="loadedCount">当前已加载依赖资源数量。</param>
    /// <param name="totalCount">总共加载依赖资源数量。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadSceneDepenAssetCallback(string sceneAssetName, string dependAssetName, int loadedCount, int totalCount, object userData);

    /// <summary>
    /// 加载场景回调函数集。
    /// </summary>
    public sealed class LoadSceneCallbacks
    {
        /// <summary>
        /// 获取加载场景成功回调函数。
        /// </summary>
        public LoadSceneSuccessCallback LoadSceneSuccessCallback;
        /// <summary>
        /// 获取加载场景失败回调函数。
        /// </summary>
        public LoadSceneFailureCallback LoadSceneFailureCallback;
        /// <summary>
        /// 获取加载场景时加载依赖资源回调函数。
        /// </summary>
        public LoadSceneDepenAssetCallback LoadSceneDepenAssetCallback;

        /// <summary>
        /// 初始化加载场景回调函数集的新实例。
        /// </summary>
        /// <param name="loadSceneSuccessCallback">加载场景成功回调函数。</param>
        public LoadSceneCallbacks(LoadSceneSuccessCallback loadSceneSuccessCallback)
            : this(loadSceneSuccessCallback, null, null)
        {
        }

        /// <summary>
        /// 初始化加载场景回调函数集的新实例。
        /// </summary>
        /// <param name="loadSceneSuccessCallback">加载场景成功回调函数。</param>
        /// <param name="loadSceneFailureCallback">加载场景失败回调函数。</param>
        public LoadSceneCallbacks(LoadSceneSuccessCallback loadSceneSuccessCallback, LoadSceneFailureCallback loadSceneFailureCallback)
            : this(loadSceneSuccessCallback, loadSceneFailureCallback, null)
        {
        }
        /// <summary>
        /// 初始化加载场景回调函数集的新实例。
        /// </summary>
        /// <param name="loadSceneSuccessCallback">加载场景成功回调函数。</param>
        /// <param name="loadSceneFailureCallback">加载场景失败回调函数。</param>
        /// <param name="loadSceneUpdateCallback">加载场景更新回调函数。</param>
        /// <param name="LoadSceneDepenAssetCallback">加载场景时加载依赖资源回调函数。</param>
        public LoadSceneCallbacks(LoadSceneSuccessCallback loadSceneSuccessCallback, LoadSceneFailureCallback loadSceneFailureCallback, LoadSceneDepenAssetCallback loadSceneDepenAssetCallback)
        {
            LoadSceneSuccessCallback = loadSceneSuccessCallback ?? throw new EPloyException("Load scene success callback is invalid.");
            LoadSceneFailureCallback = loadSceneFailureCallback;
            LoadSceneDepenAssetCallback = loadSceneDepenAssetCallback;
        }
    }
}
