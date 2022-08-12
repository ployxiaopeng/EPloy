using System;
using System.IO;

namespace EPloy.Game.Download
{
    /// <summary>
    /// 下载任务。
    /// </summary>
    public sealed class DownloadTask : IDisposable
    {
        private static int s_Serial = 0;

        /// <summary>
        /// 是否下载完成
        /// </summary>
        /// <value></value>
        public int Serial
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置下载任务的状态。
        /// </summary>
        public DownloadTaskStatus TaskStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        /// <value></value>
        public bool Done
        {
            get
            {
                return TaskStatus == DownloadTaskStatus.Done;
            }
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
        /// 获取原始下载地址。
        /// </summary>
        public string DownloadUri
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取将缓冲区写入磁盘的临界大小。
        /// </summary>
        public int FlushSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载超时时长，以秒为单位。
        /// </summary>
        public float Timeout
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载任务的描述。
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        public DownloadCallBack DownloadCallBack
        {
            get;
            private set;
        }

        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="flushSize">将缓冲区写入磁盘的临界大小。</param>
        /// <param name="timeout">下载超时时长，以秒为单位。</param>
        /// <returns>创建的下载任务。</returns>
        public static DownloadTask Create(string downloadPath, string downloadUri, int flushSize, float timeout, DownloadCallBack downloadCallBack, object userData)
        {
            DownloadTask downloadTask = new DownloadTask();
            downloadTask.Dispose();
            downloadTask.Serial += DownloadTask.s_Serial;
            downloadTask.DownloadPath = downloadPath;
            downloadTask.DownloadUri = downloadUri;
            downloadTask.FlushSize = flushSize;
            downloadTask.Timeout = timeout;
            downloadTask.DownloadCallBack = downloadCallBack;
            downloadTask.UserData = userData;
            return downloadTask;
        }

        /// <summary>
        /// 清理下载任务。
        /// </summary>
        public void Dispose()
        {
            TaskStatus = DownloadTaskStatus.Todo;
            DownloadPath = null;
            DownloadUri = null;
            FlushSize = 0;
            Timeout = 0f;
            UserData = null;
        }
    }

    /// <summary>
    /// 下载任务的状态。
    /// </summary>
    public enum DownloadTaskStatus : byte
    {
        /// <summary>
        /// 准备下载。
        /// </summary>
        Todo = 0,

        /// <summary>
        /// 下载中。
        /// </summary>
        Doing,

        /// <summary>
        /// 下载完成。
        /// </summary>
        Done,

        /// <summary>
        /// 下载错误。
        /// </summary>
        Error
    }
}
