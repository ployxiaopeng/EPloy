
using System;
using System.IO;
using UnityEngine;

namespace EPloy.Download
{

    /// <summary>
    /// 下载代理。
    /// </summary>
    internal partial class DownloadAgent : IDisposable
    {
        private FileStream FileStream;
        private int WaitFlushSize;
        private bool Disposed;

        /// <summary>
        /// 获取下载任务。
        /// </summary>
        public DownloadTask Task
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取已经等待时间。
        /// </summary>
        public float WaitTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取开始下载时已经存在的大小。
        /// </summary>
        public int StartLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取本次已经下载的大小。
        /// </summary>
        public long DownloadedLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取当前的大小。
        /// </summary>
        public int CurrentLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取已经存盘的大小。
        /// </summary>
        public int SavedLength
        {
            get;
            private set;
        }

        private DownloadCallBack DownloadCallBack
        {
            get
            {
                if (Task == null)
                {
                    return null;
                }
                return Task.DownloadCallBack;
            }
        }

        public DownloadAgent()
        {
            Task = null;
            FileStream = null;
            WaitFlushSize = 0;
            WaitTime = 0f;
            StartLength = 0;
            DownloadedLength = 0;
            SavedLength = 0;
            Disposed = false;
        }

        /// <summary>
        /// 下载代理轮询。
        /// </summary>
        public void Update()
        {
            if (Task.TaskStatus == DownloadTaskStatus.Doing)
            {
                WaitTime += Time.deltaTime;
                if (WaitTime >= Task.Timeout)
                {
                    OnDownloadError(false, "Timeout");
                }
            }
            DownloadUpdate();
        }

        /// <summary>
        /// 开始处理下载任务。
        /// </summary>
        /// <param name="task">要处理的下载任务。</param>
        /// <returns>开始处理任务的状态。</returns>
        public DownloadTaskStatus Start(DownloadTask task)
        {
            if (task == null)
            {
                Log.Fatal("Task is invalid.");
            }

            Task = task;

            Task.TaskStatus = DownloadTaskStatus.Doing;
            string downloadFile = Utility.Text.Format("{0}.download", Task.DownloadPath);

            try
            {
                if (File.Exists(downloadFile))
                {
                    FileStream = File.OpenWrite(downloadFile);
                    FileStream.Seek(0, SeekOrigin.End);
                    StartLength = SavedLength = (int)FileStream.Length;
                    DownloadedLength = 0;
                }
                else
                {
                    string directory = Path.GetDirectoryName(Task.DownloadPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    FileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    StartLength = SavedLength = 0;
                    DownloadedLength = 0;
                }

                if (DownloadCallBack.DownloadStart != null)
                {
                    DownloadInfo info = DownloadInfo.Create(Task.Serial, Task.DownloadPath, Task.DownloadUri, CurrentLength, Task.UserData);
                    DownloadCallBack.DownloadStart(info);
                    info.Dispose();
                }

                if (StartLength > 0)
                {
                    Download(Task.DownloadUri, StartLength);
                }
                else
                {
                    Download(Task.DownloadUri);
                }

                return DownloadTaskStatus.Doing;
            }
            catch (Exception exception)
            {
                OnDownloadError(false, exception.ToString());
                return DownloadTaskStatus.Error;
            }
        }

        /// <summary>
        /// 重置下载代理。
        /// </summary>
        public void Reset()
        {
            DownloadReset();
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }

            Task = null;
            WaitFlushSize = 0;
            WaitTime = 0f;
            StartLength = 0;
            DownloadedLength = 0;
            SavedLength = 0;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        private void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (FileStream != null)
                {
                    FileStream.Dispose();
                    FileStream = null;
                }
                if (UnityWebRequest != null)
                {
                    UnityWebRequest.Dispose();
                    UnityWebRequest = null;
                }
                DownloadCallBack.Dispose();
            }

            Disposed = true;
        }

        private void OnDownloadUpdateBytes(int offset, int length, byte[] bytes)
        {
            WaitTime = 0f;
            try
            {
                FileStream.Write(bytes, offset, length);
                WaitFlushSize += length;
                SavedLength += length;

                if (WaitFlushSize >= Task.FlushSize)
                {
                    FileStream.Flush();
                    WaitFlushSize = 0;
                }
            }
            catch (Exception exception)
            {
                OnDownloadError(false, exception.ToString());
            }
        }

        private void OnDownloadUpdateLength(int deltaLength)
        {
            WaitTime = 0f;
            DownloadedLength += deltaLength;
            if (DownloadCallBack.DownloadUpdate != null)
            {
                DownloadInfo info = DownloadInfo.Create(Task.Serial, Task.DownloadPath, Task.DownloadUri, CurrentLength, Task.UserData);
                // todo: 本次以下载大小待定
                DownloadCallBack.DownloadUpdate(info);
                info.Dispose();
            }
        }

        private void OnDownloadComplete(long length)
        {
            WaitTime = 0f;
            DownloadedLength = length;
            if (SavedLength != CurrentLength)
            {
                Log.Fatal("Internal download error");
                return;
            }

            DownloadReset();
            FileStream.Close();
            FileStream = null;

            if (File.Exists(Task.DownloadPath))
            {
                File.Delete(Task.DownloadPath);
            }

            File.Move(Utility.Text.Format("{0}.download", Task.DownloadPath), Task.DownloadPath);

            Task.TaskStatus = DownloadTaskStatus.Done;

            if (DownloadCallBack.DownloadSuccess != null)
            {
                // todo: 本次以下载大小待定
                DownloadInfo info = DownloadInfo.Create(Task.Serial, Task.DownloadPath, Task.DownloadUri, CurrentLength, Task.UserData);
                DownloadCallBack.DownloadSuccess(info);
                info.Dispose();
            }

            Task.TaskStatus = DownloadTaskStatus.Done;
        }

        private void OnDownloadError(bool isDelete, string errorMessage)
        {
            DownloadReset();
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }

            if (isDelete)
            {
                File.Delete(Utility.Text.Format("{0}.download", Task.DownloadPath));
            }

            Task.TaskStatus = DownloadTaskStatus.Error;

            if (DownloadCallBack.DownloadFailure != null)
            {
                DownloadInfo info = DownloadInfo.Create(Task.Serial, Task.DownloadPath, Task.DownloadUri, CurrentLength, Task.UserData);
                info.ErrMsg = errorMessage;
                DownloadCallBack.DownloadFailure(info);
                info.Dispose();
            }

            Task.TaskStatus = DownloadTaskStatus.Error;
        }
    }
}

