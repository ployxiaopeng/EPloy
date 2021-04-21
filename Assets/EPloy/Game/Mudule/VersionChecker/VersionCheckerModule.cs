using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using EPloy.Res;

namespace EPloy
{
    /// <summary>
    /// 版本校验。
    /// </summary>
    public class VersionCheckerModule : EPloyModule
    {
        private UpdatableVersionListSerializer UpdatableVersionListSerializer;
        private VersionInfo VersionInfo = null;
        private Action<bool, VersionInfo> VersionCheckerCallback;

        public override void Awake()
        {
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
        }

        public override void Update()
        {

        }

        public override void OnDestroy()
        {

        }


        public void VersionChecker(Action<bool, VersionInfo> versionCheckerCallback)
        {
            if (versionCheckerCallback == null)
            {
                Log.Fatal("versionCheckerCallback is null");
                return;
            }
            VersionCheckerCallback = versionCheckerCallback;

            // 向服务器请求版本信息
            Game.Instance.StartCoroutine(CheckVersionRequest(Utility.Text.Format(MuduleConfig.CheckVersionUrl, GetPlatformPath())));
        }

        private IEnumerator CheckVersionRequest(string fileUri)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
            if (!isError)
            {
                OnWebRequestSuccess(bytes);
                VersionCheckerCallback(true, VersionInfo);
            }
            else
            {
                Log.Fatal(Utility.Text.Format("Check version failure, error message is '{0}'.", errorMessage));
                VersionCheckerCallback(false, null);
            }
        }

        private void OnWebRequestSuccess(byte[] versionInfoBytes)
        {

            string versionInfoString = Utility.Converter.GetString(versionInfoBytes);
            VersionInfo = JsonMapper.ToObject<VersionInfo>(versionInfoString);
            if (VersionInfo == null)
            {
                Log.Error("Parse VersionInfo failure.");
                return;
            }
            string str = Utility.Text.Format("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.",
            VersionInfo.LatestGameVersion, VersionInfo.InternalGameVersion, Application.version, 0);
            Log.Info(str);

            if (VersionInfo.UpdateGame)
            {
                // 需要强制更新游戏应用

                return;
            }
            VersionInfo.UpdateGame = CheckVersionList(VersionInfo.InternalResourceVersion) == CheckVersionListResult.NeedUpdate;
        }

        /// <summary>
        /// 检查版本资源列表。
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号。</param>
        /// <returns>检查版本资源列表结果。</returns>
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {

            string versionListFileName = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, MuduleConfig.RemoteVersionListFileName));
            if (!File.Exists(versionListFileName))
            {
                return CheckVersionListResult.NeedUpdate;
            }

            int internalResourceVersion = 0;
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(versionListFileName, FileMode.Open, FileAccess.Read);
                object internalResourceVersionObject = null;
                if (!UpdatableVersionListSerializer.TryGetValue(fileStream, "InternalResourceVersion", out internalResourceVersionObject))
                {
                    return CheckVersionListResult.NeedUpdate;
                }

                internalResourceVersion = (int)internalResourceVersionObject;
            }
            catch
            {
                return CheckVersionListResult.NeedUpdate;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }
            }

            if (internalResourceVersion != latestInternalResourceVersion)
            {
                return CheckVersionListResult.NeedUpdate;
            }

            return CheckVersionListResult.Updated;
        }


        public void UpdateVersionList()
        {
            string localVersionListFilePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, MuduleConfig.RemoteVersionListFileName));
            int dotPosition = MuduleConfig.RemoteVersionListFileName.LastIndexOf('.');
            string latestVersionListFullNameWithCrc32 = Utility.Text.Format("{0}.{2:x8}.{1}", MuduleConfig.RemoteVersionListFileName.Substring(0, dotPosition),
             MuduleConfig.RemoteVersionListFileName.Substring(dotPosition + 1), VersionInfo.VersionListHashCode);
            //  m_DownloadManager.AddDownload(localVersionListFilePath, Utility.Path.GetRemotePath(Path.Combine(VersionInfo.UpdatePrefixUri, latestVersionListFullNameWithCrc32)), this);
        }

        // private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
        // {
        //     VersionListProcessor versionListProcessor = e.UserData as VersionListProcessor;
        //     if (versionListProcessor == null || versionListProcessor != this)
        //     {
        //         return;
        //     }

        //     using (FileStream fileStream = new FileStream(e.DownloadPath, FileMode.Open, FileAccess.ReadWrite))
        //     {
        //         int length = (int)fileStream.Length;
        //         if (length != m_VersionListZipLength)
        //         {
        //             fileStream.Close();
        //             string errorMessage = Utility.Text.Format("Latest version list zip length error, need '{0}', downloaded '{1}'.", m_VersionListZipLength.ToString(), length.ToString());
        //             DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
        //             OnDownloadFailure(this, downloadFailureEventArgs);
        //             ReferencePool.Release(downloadFailureEventArgs);
        //             return;
        //         }

        //         fileStream.Position = 0L;
        //         int hashCode = Utility.Verifier.GetCrc32(fileStream);
        //         if (hashCode != m_VersionListZipHashCode)
        //         {
        //             fileStream.Close();
        //             string errorMessage = Utility.Text.Format("Latest version list zip hash code error, need '{0}', downloaded '{1}'.", m_VersionListZipHashCode.ToString(), hashCode.ToString());
        //             DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
        //             OnDownloadFailure(this, downloadFailureEventArgs);
        //             ReferencePool.Release(downloadFailureEventArgs);
        //             return;
        //         }

        //         if (m_ResourceManager.m_DecompressCachedStream == null)
        //         {
        //             m_ResourceManager.m_DecompressCachedStream = new MemoryStream();
        //         }

        //         try
        //         {
        //             fileStream.Position = 0L;
        //             m_ResourceManager.m_DecompressCachedStream.Position = 0L;
        //             m_ResourceManager.m_DecompressCachedStream.SetLength(0L);
        //             if (!Utility.Zip.Decompress(fileStream, m_ResourceManager.m_DecompressCachedStream))
        //             {
        //                 fileStream.Close();
        //                 string errorMessage = Utility.Text.Format("Unable to decompress latest version list '{0}'.", e.DownloadPath);
        //                 DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
        //                 OnDownloadFailure(this, downloadFailureEventArgs);
        //                 ReferencePool.Release(downloadFailureEventArgs);
        //                 return;
        //             }

        //             if (m_ResourceManager.m_DecompressCachedStream.Length != m_VersionListLength)
        //             {
        //                 fileStream.Close();
        //                 string errorMessage = Utility.Text.Format("Latest version list length error, need '{0}', downloaded '{1}'.", m_VersionListLength.ToString(), m_ResourceManager.m_DecompressCachedStream.Length.ToString());
        //                 DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
        //                 OnDownloadFailure(this, downloadFailureEventArgs);
        //                 ReferencePool.Release(downloadFailureEventArgs);
        //                 return;
        //             }

        //             fileStream.Position = 0L;
        //             fileStream.SetLength(0L);
        //             fileStream.Write(m_ResourceManager.m_DecompressCachedStream.GetBuffer(), 0, (int)m_ResourceManager.m_DecompressCachedStream.Length);
        //         }
        //         catch (Exception exception)
        //         {
        //             fileStream.Close();
        //             string errorMessage = Utility.Text.Format("Unable to decompress latest version list '{0}' with error message '{1}'.", e.DownloadPath, exception.ToString());
        //             DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
        //             OnDownloadFailure(this, downloadFailureEventArgs);
        //             ReferencePool.Release(downloadFailureEventArgs);
        //             return;
        //         }
        //         finally
        //         {
        //             m_ResourceManager.m_DecompressCachedStream.Position = 0L;
        //             m_ResourceManager.m_DecompressCachedStream.SetLength(0L);
        //         }
        //     }

        //     if (VersionListUpdateSuccess != null)
        //     {
        //         VersionListUpdateSuccess(e.DownloadPath, e.DownloadUri);
        //     }
        // }

        // private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
        // {
        //     VersionListProcessor versionListProcessor = e.UserData as VersionListProcessor;
        //     if (versionListProcessor == null || versionListProcessor != this)
        //     {
        //         return;
        //     }

        //     if (File.Exists(e.DownloadPath))
        //     {
        //         File.Delete(e.DownloadPath);
        //     }

        //     if (VersionListUpdateFailure != null)
        //     {
        //         VersionListUpdateFailure(e.DownloadUri, e.ErrorMessage);
        //     }
        // }

        private string GetPlatformPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "MacOS";

                case RuntimePlatform.IPhonePlayer:
                    return "IOS";

                case RuntimePlatform.Android:
                    return "Android";

                default:
                    throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.", Application.platform.ToString()));
            }
        }
    }
}