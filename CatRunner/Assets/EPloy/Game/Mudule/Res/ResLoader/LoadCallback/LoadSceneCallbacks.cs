using System;
namespace EPloy.Game.Res
{
    /// <summary>
    /// 加载场景回调函数集。
    /// </summary>
    public sealed class LoadSceneCallbacks
    {
        /// <summary>
        /// 加载场景成功回调函数。
        /// </summary>
        /// <param name="string">要加载的场景资源名称。</param>
        /// <param name="float">加载持续时间。</param>
        public Action<string, float> LoadSceneSuccessCallback;
        /// <summary>
        /// 加载场景失败回调函数。
        /// </summary>
        /// <param name="string">要加载的场景资源名称。</param>
        /// <param name="LoadResStatus">加载场景状态。</param>
        /// <param name="string">错误信息。</param>
        public Action<string, LoadResStatus, string> LoadSceneFailureCallback;
        /// <summary>
        /// 加载场景时加载依赖资源回调函数。
        /// </summary>
        /// <param name="string">要加载的场景资源名称。</param>
        /// <param name="string">被加载的依赖资源名称。</param>
        /// <param name="int">当前已加载依赖资源数量。</param>
        /// <param name="int">总共加载依赖资源数量。</param>
        public Action<string, string, int, int> LoadSceneDepenAssetCallback;

        /// <summary>
        /// 初始化加载场景回调函数集的新实例。
        /// </summary>
        /// <param name="loadSceneSuccessCallback">加载场景成功回调函数。</param>
        public LoadSceneCallbacks(Action<string, float> loadSceneSuccessCallback)
            : this(loadSceneSuccessCallback, null, null)
        {
        }

        /// <summary>
        /// 初始化加载场景回调函数集的新实例。
        /// </summary>
        /// <param name="loadSceneSuccessCallback">加载场景成功回调函数。</param>
        /// <param name="loadSceneFailureCallback">加载场景失败回调函数。</param>
        public LoadSceneCallbacks(Action<string, float> loadSceneSuccessCallback, Action<string, LoadResStatus, string> loadSceneFailureCallback)
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
        public LoadSceneCallbacks(Action<string, float> loadSceneSuccessCallback, Action<string, LoadResStatus, string> loadSceneFailureCallback, Action<string, string, int, int> loadSceneDepenAssetCallback)
        {
            LoadSceneSuccessCallback = loadSceneSuccessCallback ?? throw new Exception("Load scene success callback is invalid.");
            LoadSceneFailureCallback = loadSceneFailureCallback;
            LoadSceneDepenAssetCallback = loadSceneDepenAssetCallback;
        }
    }
}
