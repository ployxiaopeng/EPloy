
namespace EPloy.Res
{
    /// <summary>
    /// 加载二进制资源成功回调函数。
    /// </summary>
    /// <param name="binaryAssetName">要加载的二进制资源名称。</param>
    /// <param name="binaryBytes">已加载的二进制资源。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadBinarySuccessCallback(string binaryAssetName, byte[] binaryBytes, float duration);

    /// <summary>
    /// 加载二进制资源失败回调函数。
    /// </summary>
    /// <param name="binaryAssetName">要加载的二进制资源名称。</param>
    /// <param name="status">加载二进制资源状态。</param>
    /// <param name="errorMessage">错误信息。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadBinaryFailureCallback(string binaryAssetName, LoadResStatus status, string errorMessage);

    /// <summary>
    /// 加载二进制资源回调函数集。
    /// </summary>
    public sealed class LoadBinaryCallbacks
    {
        /// <summary>
        /// 获取加载二进制资源成功回调函数。
        /// </summary>
        public LoadBinarySuccessCallback LoadBinarySuccessCallback { get; private set; }
        /// <summary>
        /// 获取加载二进制资源失败回调函数。
        /// </summary>
        public LoadBinaryFailureCallback LoadBinaryFailureCallback { get; private set; }

        /// <summary>
        /// 初始化加载二进制资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadBinarySuccessCallback">加载二进制资源成功回调函数。</param>
        public LoadBinaryCallbacks(LoadBinarySuccessCallback loadBinarySuccessCallback)
            : this(loadBinarySuccessCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载二进制资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadBinarySuccessCallback">加载二进制资源成功回调函数。</param>
        /// <param name="loadBinaryFailureCallback">加载二进制资源失败回调函数。</param>
        public LoadBinaryCallbacks(LoadBinarySuccessCallback loadBinarySuccessCallback, LoadBinaryFailureCallback loadBinaryFailureCallback)
        {
            if (loadBinarySuccessCallback == null)
            {
                throw new EPloyException("Load binary success callback is invalid.");
            }

           LoadBinarySuccessCallback = loadBinarySuccessCallback;
            LoadBinaryFailureCallback = loadBinaryFailureCallback;
        }
    }
}
