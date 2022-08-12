
using System;
using UnityEngine.Networking;

namespace EPloy.Game.Download
{
    /// <summary>
    /// 使用 UnityWebRequest 实现的下载代理辅助器。
    /// </summary>
    internal partial class DownloadAgent : IDisposable
    {
        /// <summary>
        /// 范围不适用错误码。
        /// </summary>
        private const int RangeNotSatisfiableErrorCode = 416;
        private const int CachedBytesLength = 0x1000;

        private readonly byte[] CachedBytes = new byte[CachedBytesLength];
        private UnityWebRequest UnityWebRequest = null;

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据。
        /// </summary>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void Download(string downloadUri)
        {
            UnityWebRequest = new UnityWebRequest(downloadUri);
            UnityWebRequest.downloadHandler = new DownloadHandler(this);
            UnityWebRequest.SendWebRequest();

        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据。
        /// </summary>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="fromPosition">下载数据起始位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void Download(string downloadUri, long fromPosition)
        {
            UnityWebRequest = new UnityWebRequest(downloadUri);
            UnityWebRequest.SetRequestHeader("Range", UtilText.Format("bytes={0}-", fromPosition.ToString()));
            UnityWebRequest.downloadHandler = new DownloadHandler(this);
            UnityWebRequest.SendWebRequest();
        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据。
        /// </summary>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="fromPosition">下载数据起始位置。</param>
        /// <param name="toPosition">下载数据结束位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void Download(string downloadUri, long fromPosition, long toPosition)
        {
            UnityWebRequest = new UnityWebRequest(downloadUri);
            UnityWebRequest.SetRequestHeader("Range", UtilText.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()));
            UnityWebRequest.downloadHandler = new DownloadHandler(this);
            UnityWebRequest.SendWebRequest();
        }

        /// <summary>
        /// 重置下载代理辅助器。
        /// </summary>
        public void DownloadReset()
        {
            if (UnityWebRequest != null)
            {
                UnityWebRequest.Abort();
                UnityWebRequest.Dispose();
                UnityWebRequest = null;
            }

            Array.Clear(CachedBytes, 0, CachedBytesLength);
        }


        private void DownloadUpdate()
        {
            if (UnityWebRequest == null)
            {
                return;
            }

            if (!UnityWebRequest.isDone)
            {
                return;
            }

            bool isError = false;
            isError = UnityWebRequest.isNetworkError || UnityWebRequest.isHttpError;

            if (isError)
            {
                OnDownloadError(UnityWebRequest.responseCode == RangeNotSatisfiableErrorCode, UnityWebRequest.error);
            }
            else
            {
                OnDownloadComplete((long)UnityWebRequest.downloadedBytes);
            }
        }
    }
}
