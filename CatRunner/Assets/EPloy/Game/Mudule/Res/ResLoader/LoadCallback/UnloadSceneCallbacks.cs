using System;

namespace EPloy.Game.Res
{

    /// <summary>
    /// 卸载场景失败回调函数。
    /// </summary>
    /// <param name="sceneAssetName">要卸载的场景资源名称。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void UnloadSceneFailureCallback(string sceneAssetName);

    /// <summary>
    /// 卸载场景回调函数集。
    /// </summary>
    public sealed class UnloadSceneCallbacks
    {
        /// <summary>
        /// 获取卸载场景成功回调函数。
        /// </summary>
        public Action<string> UnloadSceneSuccessCallback
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取卸载场景失败回调函数。
        /// </summary>
        public Action<string> UnloadSceneFailureCallback
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化卸载场景回调函数集的新实例。
        /// </summary>
        /// <param name="unloadSceneSuccessCallback">卸载场景成功回调函数。</param>
        public UnloadSceneCallbacks(Action<string> unloadSceneSuccessCallback)
            : this(unloadSceneSuccessCallback, null)
        {
        }

        /// <summary>
        /// 初始化卸载场景回调函数集的新实例。
        /// </summary>
        /// <param name="unloadSceneSuccessCallback">卸载场景成功回调函数。</param>
        /// <param name="unloadSceneFailureCallback">卸载场景失败回调函数。</param>
        public UnloadSceneCallbacks(Action<string> unloadSceneSuccessCallback, Action<string> unloadSceneFailureCallback)
        {
            if (unloadSceneSuccessCallback == null)
            {
                Log.Fatal("Unload scene success callback is invalid.");
                return;
            }

            UnloadSceneSuccessCallback = unloadSceneSuccessCallback;
            UnloadSceneFailureCallback = unloadSceneFailureCallback;
        }
    }
}
