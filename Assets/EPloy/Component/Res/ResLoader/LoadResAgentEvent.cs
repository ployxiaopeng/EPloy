
using EPloy;

namespace EPloy.Res
{
    /// <summary>
    /// 代理加载资源事件。
    /// </summary>
    public sealed class LoadResAgentEvent : EventArg
    {
        public override int id
        {
            get
            {
                return 101;
            }
        }

        /// <summary>
        /// 加载类型
        /// </summary>
        public LoadType LoadType { get; private set; }

        /// <summary>
        /// 获取加载资源状态。
        /// </summary>
        public LoadResStatus Status { get; private set; }

        /// <summary>
        /// 获取加载资源进度。
        /// </summary>
        public LoadResProgress LoadProgress { get; private set; }

        /// <summary>
        /// 获取进度。
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// 获取加载的资源。
        /// </summary>
        public object Asset { get; private set; }

        /// <summary>
        /// 二进制 数据
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        ///  辅助器更新事件。
        /// </summary>
        public void SetProgress(LoadType loadType, LoadResStatus status, LoadResProgress loadProgress, float progress)
        {
            LoadType = loadType;
            Status = status;
            LoadProgress = loadProgress;
            Progress = progress;
        }

        /// <summary>
        /// 辅助器资源完成事件。
        /// </summary>
        public void SetAsset(LoadType loadType, LoadResStatus status, object asset)
        {
            LoadType = loadType;
            Status = status;
            Asset = asset;
        }

        /// <summary>
        /// 辅助器二进制流完成事件。
        /// </summary>
        public void Setbytes(LoadType loadType, LoadResStatus status, byte[] bytes)
        {
            LoadType = loadType;
            Status = status;
            Bytes = bytes;
        }

        /// <summary>
        ///  错误数据
        /// </summary>
        public void SetErr(LoadType loadType, LoadResStatus status, string errorMsg)
        {
            LoadType = loadType;
            Status = status;
            ErrorMsg = errorMsg;

        }

        /// <summary>
        /// 清理加载资源代理辅助器错误事件。
        /// </summary>
        public override void Clear()
        {
            Status = LoadResStatus.Success;
            ErrorMsg = null;
        }
    }
}
