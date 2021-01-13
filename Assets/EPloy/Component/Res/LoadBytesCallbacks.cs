namespace EPloy
{

    /// <summary>
    /// 加载数据流失败回调函数。
    /// </summary>
    /// <param name="fileUri">文件路径。</param>
    /// <param name="errorMessage">错误信息。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadBytesFailureCallback(string fileUri, string errorMessage, object userData);

    /// <summary>
    /// 加载数据流成功回调函数。
    /// </summary>
    /// <param name="fileUri">文件路径。</param>
    /// <param name="bytes">数据流。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadBytesSuccessCallback(string fileUri, byte[] bytes, float duration, object userData);

    /// <summary>
    /// 加载数据流回调函数集。
    /// </summary>
    public sealed class LoadBytesCallbacks
    {
        private readonly LoadBytesSuccessCallback m_LoadBytesSuccessCallback;
        private readonly LoadBytesFailureCallback m_LoadBytesFailureCallback;

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
            if (loadBytesSuccessCallback == null)
            {
                throw new EPloyException("Load bytes success callback is invalid.");
            }

            m_LoadBytesSuccessCallback = loadBytesSuccessCallback;
            m_LoadBytesFailureCallback = loadBytesFailureCallback;
        }

        /// <summary>
        /// 获取加载数据流成功回调函数。
        /// </summary>
        public LoadBytesSuccessCallback LoadBytesSuccessCallback
        {
            get
            {
                return m_LoadBytesSuccessCallback;
            }
        }

        /// <summary>
        /// 获取加载数据流失败回调函数。
        /// </summary>
        public LoadBytesFailureCallback LoadBytesFailureCallback
        {
            get
            {
                return m_LoadBytesFailureCallback;
            }
        }
    }
}
