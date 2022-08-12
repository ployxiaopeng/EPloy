using System;
namespace EPloy.Res
{
    /// <summary>
    /// 加载二进制资源回调函数集。
    /// </summary>
    public sealed class LoadBinaryCallbacks
    {
        /// <summary>
        /// 获取加载二进制资源成功回调函数。
        /// </summary>
        public Action<string, byte[], float> LoadBinarySuccessCallback { get; private set; }
        /// <summary>
        /// 获取加载二进制资源失败回调函数。
        /// </summary>
        public Action<string, LoadResStatus, string> LoadBinaryFailureCallback { get; private set; }

        /// <summary>
        /// 初始化加载二进制资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadBinarySuccessCallback">加载二进制资源成功回调函数。</param>
        public LoadBinaryCallbacks(Action<string, byte[], float> loadBinarySuccessCallback)
            : this(loadBinarySuccessCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载二进制资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadBinarySuccessCallback">加载二进制资源成功回调函数。</param>
        /// <param name="loadBinaryFailureCallback">加载二进制资源失败回调函数。</param>
        public LoadBinaryCallbacks(Action<string, byte[], float> loadBinarySuccessCallback, Action<string, LoadResStatus, string> loadBinaryFailureCallback)
        {
            if (loadBinarySuccessCallback == null)
            {
                Log.Fatal("Load binary success callback is invalid.");
                return;
            }

            LoadBinarySuccessCallback = loadBinarySuccessCallback;
            LoadBinaryFailureCallback = loadBinaryFailureCallback;
        }
    }
}
