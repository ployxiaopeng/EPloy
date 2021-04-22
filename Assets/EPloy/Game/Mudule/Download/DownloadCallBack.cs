
using System;
using System.IO;
using UnityEngine;

namespace EPloy
{
    public class DownloadCallBack : IDisposable
    {
        public EPloyAction<DownloadTask> DownloadStart { get; private set; }
        public EPloyAction<DownloadTask, int> DownloadUpdate { get; private set; }
        public EPloyAction<DownloadTask, long> DownloadSuccess { get; private set; }
        public EPloyAction<DownloadTask, string> DownloadFailure { get; private set; }

        public DownloadCallBack() : this(null, null, null, null)
        {
        }

        public DownloadCallBack(EPloyAction<DownloadTask, long> downloadSuccess, EPloyAction<DownloadTask, string> downloadFailure) :
         this(downloadSuccess, downloadFailure, null, null)
        {

        }

        public DownloadCallBack(EPloyAction<DownloadTask, long> downloadSuccess, EPloyAction<DownloadTask, string> downloadFailure,
       EPloyAction<DownloadTask, int> downloadUpdate) : this(downloadSuccess, downloadFailure, null, downloadUpdate)
        {

        }

        public DownloadCallBack(EPloyAction<DownloadTask, long> downloadSuccess, EPloyAction<DownloadTask, string> downloadFailure,
         EPloyAction<DownloadTask> downloadStart, EPloyAction<DownloadTask, int> downloadUpdate)
        {
            DownloadStart = downloadStart; DownloadUpdate = downloadUpdate;
            DownloadSuccess = downloadSuccess; DownloadFailure = downloadFailure;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}