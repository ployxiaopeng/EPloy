
using System;
using System.IO;
using UnityEngine;

namespace EPloy
{
    public class DownloadCallBack : IDisposable
    {

        public Action<DownloadAgent> DownloadStart { get; private set; }
        public Action<DownloadAgent, int> DownloadUpdate { get; private set; }
        public Action<DownloadAgent, long> DownloadSuccess { get; private set; }
        public Action<DownloadAgent, string> DownloadFailure { get; private set; }

        public DownloadCallBack(Action<DownloadAgent, long> downloadSuccess, Action<DownloadAgent, string> downloadFailure)
        {
            DownloadStart = null; DownloadUpdate = null;
            DownloadSuccess = downloadSuccess; DownloadFailure = downloadFailure;
        }

        public DownloadCallBack(Action<DownloadAgent, long> downloadSuccess, Action<DownloadAgent, string> downloadFailure,
       Action<DownloadAgent, int> downloadUpdate)
        {
            DownloadStart = null; DownloadUpdate = downloadUpdate;
            DownloadSuccess = downloadSuccess; DownloadFailure = downloadFailure;
        }

        public DownloadCallBack(Action<DownloadAgent, long> downloadSuccess, Action<DownloadAgent, string> downloadFailure,
         Action<DownloadAgent> downloadStart, Action<DownloadAgent, int> downloadUpdate)
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