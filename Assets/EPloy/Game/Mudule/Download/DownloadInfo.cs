using System;

namespace EPloy.Download
{
    /// <summary>
    /// 下载开始事件。
    /// </summary>
    public sealed class DownloadInfo : IDisposable
    {
        /// <summary>
        /// 初始化下载开始事件的新实例。
        /// </summary>
        public DownloadInfo()
        {
            SerialId = 0;
            DownloadPath = null;
            DownloadUri = null;
            CurrentLength = 0;
            UserData = null;
        }

        /// <summary>
        /// 获取下载任务的序列编号。
        /// </summary>
        public int SerialId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载后存放路径。
        /// </summary>
        public string DownloadPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载地址。
        /// </summary>
        public string DownloadUri
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取当前大小。
        /// </summary>
        public int CurrentLength
        {
            get;
            private set;
        }

        /// <summary>
        ///  设置当前错误信息
        /// </summary>
        public string ErrMsg
        {
            get;
            set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建下载开始事件。
        /// </summary>
        /// <param name="serialId">下载任务的序列编号。</param>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="currentLength">当前大小。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的下载开始事件。</returns>
        public static DownloadInfo Create(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            DownloadInfo downloadInfo = new DownloadInfo();
            downloadInfo.SerialId = serialId;
            downloadInfo.DownloadPath = downloadPath;
            downloadInfo.DownloadUri = downloadUri;
            downloadInfo.CurrentLength = currentLength;
            downloadInfo.UserData = userData;
            return downloadInfo;
        }

        /// <summary>
        /// 重置下载信息类
        /// </summary>
        public void Dispose()
        {
            SerialId = 0;
            DownloadPath = null;
            DownloadUri = null;
            CurrentLength = 0;
            UserData = null;
        }
    }
}
