
using System;
using System.IO;
using UnityEngine;

namespace EPloy.Download
{
    public class DownloadCallBack : IDisposable
    {
        public EPloyAction<DownloadInfo> DownloadStart { get; private set; }
        public EPloyAction<DownloadInfo> DownloadUpdate { get; private set; }
        public EPloyAction<DownloadInfo> DownloadSuccess { get; private set; }
        public EPloyAction<DownloadInfo> DownloadFailure { get; private set; }

        public DownloadCallBack() : this(null, null, null, null)
        {
        }

        public DownloadCallBack(EPloyAction<DownloadInfo> downloadSuccess, EPloyAction<DownloadInfo> downloadFailure) :
         this(downloadSuccess, downloadFailure, null, null)
        {

        }

        public DownloadCallBack(EPloyAction<DownloadInfo> downloadSuccess, EPloyAction<DownloadInfo> downloadFailure,
       EPloyAction<DownloadInfo> downloadUpdate) : this(downloadSuccess, downloadFailure, null, downloadUpdate)
        {

        }

        public DownloadCallBack(EPloyAction<DownloadInfo> downloadSuccess, EPloyAction<DownloadInfo> downloadFailure,
         EPloyAction<DownloadInfo> downloadStart, EPloyAction<DownloadInfo> downloadUpdate)
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