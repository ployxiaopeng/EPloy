
namespace EPloy.Res
{
    /// <summary>
    /// 加载资源成功回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="asset">已加载的资源。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetSuccessCallback(string assetName, object asset, float duration);

    /// <summary>
    /// 加载资源失败回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="status">加载资源状态。</param>
    /// <param name="errorMessage">错误信息。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetFailureCallback(string assetName, LoadResStatus status, string errorMessage);

    /// <summary>
    /// 加载依赖资源回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="dependAssetName">被加载的依赖资源名称。</param>
    /// <param name="loadedCount">当前已加载依赖资源数量。</param>
    /// <param name="totalCount">总共加载依赖资源数量。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadDependAssetCallback(string assetName, string dependAssetName, int loadedCount, int totalCount);

    /// <summary>
    /// 加载资源回调函数集。
    /// </summary>
    public sealed class LoadAssetCallbacks
    {
        /// <summary>
        /// 获取加载资源成功回调函数。
        /// </summary>
        public LoadAssetSuccessCallback LoadAssetSuccessCallback { get; private set; }
        /// <summary>
        /// 获取加载资源失败回调函数。
        /// </summary>
        public LoadAssetFailureCallback LoadAssetFailureCallback { get; private set; }
        /// <summary>
        /// 获取加载资源时加载依赖资源回调函数。
        /// </summary>
        public LoadDependAssetCallback LoadDependAssetCallback { get; private set; }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback)
            : this(loadAssetSuccessCallback, null, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadDependAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback, LoadDependAssetCallback loadDependAssetCallback)
        {
            this.LoadAssetSuccessCallback = loadAssetSuccessCallback ?? throw new EPloyException("Load asset success callback is invalid.");
            this.LoadAssetFailureCallback = loadAssetFailureCallback;
            this.LoadDependAssetCallback = loadDependAssetCallback;
        }
    }
}