
using System;
using System.IO;
using UnityEngine;

namespace EPloy.Game.Download
{
    public class DownloadCallBack : IDisposable
    {
        public Action<DownloadInfo> DownloadStart { get; private set; }
        public Action<DownloadInfo> DownloadUpdate { get; private set; }
        public Action<DownloadInfo> DownloadSuccess { get; private set; }
        public Action<DownloadInfo> DownloadFailure { get; private set; }

        public DownloadCallBack() : this(null, null, null, null)
        {
        }

        public DownloadCallBack(Action<DownloadInfo> downloadSuccess, Action<DownloadInfo> downloadFailure) :
         this(downloadSuccess, downloadFailure, null, null)
        {

        }

        public DownloadCallBack(Action<DownloadInfo> downloadSuccess, Action<DownloadInfo> downloadFailure,
       Action<DownloadInfo> downloadUpdate) : this(downloadSuccess, downloadFailure, null, downloadUpdate)
        {

        }

        public DownloadCallBack(Action<DownloadInfo> downloadSuccess, Action<DownloadInfo> downloadFailure,
         Action<DownloadInfo> downloadStart, Action<DownloadInfo> downloadUpdate)
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