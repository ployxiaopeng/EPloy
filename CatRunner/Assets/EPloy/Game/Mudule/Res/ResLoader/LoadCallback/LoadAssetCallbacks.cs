
using System;

namespace EPloy.Game.Res
{
    /// <summary>
    /// 加载资源回调函数集。
    /// </summary>
    public sealed class LoadAssetCallbacks
    {
        /// <summary>
        /// 获取加载资源成功回调函数。
        /// </summary>
        public Action<string, object, float, object> LoadAssetSuccessCallback { get; private set; }
        /// <summary>
        /// 获取加载资源失败回调函数。
        /// </summary>
        public Action<string, LoadResStatus, string> LoadAssetFailureCallback { get; private set; }
        /// <summary>
        /// 获取加载资源时加载依赖资源回调函数。
        /// </summary>
        public Action<string, string, int, int> LoadDependAssetCallback { get; private set; }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        public LoadAssetCallbacks(Action<string, object, float, object> loadAssetSuccessCallback)
            : this(loadAssetSuccessCallback, null, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        public LoadAssetCallbacks(Action<string, object, float, object> loadAssetSuccessCallback, Action<string, LoadResStatus, string> loadAssetFailureCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadDependAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetCallbacks(Action<string, object, float, object> loadAssetSuccessCallback, Action<string, LoadResStatus, string> loadAssetFailureCallback, Action<string, string, int, int> loadDependAssetCallback)
        {
            this.LoadAssetSuccessCallback = loadAssetSuccessCallback ?? throw new Exception("Load asset success callback is invalid.");
            this.LoadAssetFailureCallback = loadAssetFailureCallback;
            this.LoadDependAssetCallback = loadDependAssetCallback;
        }
    }
}