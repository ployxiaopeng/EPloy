namespace EPloy.Res
{

    /// <summary>
    /// 加载数据流失败回调函数。
    /// </summary>
    /// <param name="fileUri">文件路径。</param>
    /// <param name="errorMessage">错误信息。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadBytesFailureCallback(string fileUri, string errorMessage);

    /// <summary>
    /// 加载数据流成功回调函数。
    /// </summary>
    /// <param name="fileUri">文件路径。</param>
    /// <param name="bytes">数据流。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadBytesSuccessCallback(string fileUri, byte[] bytes, float duration);

    /// <summary>
    /// 加载数据流回调函数集。
    /// </summary>
    public sealed class LoadBytesCallbacks
    {
        /// <summary>
        /// 获取加载数据流成功回调函数。
        /// </summary>
        public LoadBytesSuccessCallback LoadBytesSuccessCallback { get; private set; }

        /// <summary>
        /// 获取加载数据流失败回调函数。
        /// </summary>
        public LoadBytesFailureCallback LoadBytesFailureCallback { get; private set; }

        /// <summary>
        /// 初始化加载数据流回调函数集的新实例。
        /// </summary>
        /// <param name="loadBinarySuccessCallback">加载数据流成功回调函数。</param>
        public LoadBytesCallbacks(LoadBytesSuccessCallback loadBinarySuccessCallback)
            : this(loadBinarySuccessCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载数据流回调函数集的新实例。
        /// </summary>
        /// <param name="loadBytesSuccessCallback">加载数据流成功回调函数。</param>
        /// <param name="loadBytesFailureCallback">加载数据流失败回调函数。</param>
        public LoadBytesCallbacks(LoadBytesSuccessCallback loadBytesSuccessCallback, LoadBytesFailureCallback loadBytesFailureCallback)
        {
            LoadBytesSuccessCallback = loadBytesSuccessCallback ?? throw new EPloyException("Load bytes success callback is invalid.");
            LoadBytesFailureCallback = loadBytesFailureCallback;
        }

    }
}