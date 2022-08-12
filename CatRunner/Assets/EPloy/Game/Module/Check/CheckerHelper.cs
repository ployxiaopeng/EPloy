using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using EPloy.Res;
using EPloy.Download;

namespace EPloy.Checker
{
    /// <summary>
    /// 版本校验辅助
    /// </summary>
    public static class CheckerHelper
    {
        private static UpdatableVersionListSerializer UpdatableVersionListSerializer;
        private static VersionInfo VersionInfo;
        private static DownloadCallBack DownloadCallBack;

        private static Action<bool, VersionInfo> VersionCheckerCallback;
        private static Action<bool, string> VersionUpdateCallback;

        public static void Awake()
        {
            VersionInfo = null;
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            DownloadCallBack = new DownloadCallBack(OnDownloadSuccess, OnDownloadFailure);
            VersionCheckerCallback = null;
            VersionUpdateCallback = null;
        }

        public static void OnDestroy()
        {
            VersionInfo = null;
            UpdatableVersionListSerializer = null;
            DownloadCallBack = null;
            VersionCheckerCallback = null;
            VersionUpdateCallback = null;
        }

        public static void VersionCheck(Action<bool, VersionInfo> versionCheckerCallback)
        {
            if (versionCheckerCallback == null)
            {
                Log.Fatal("versionCheckerCallback is null");
                return;
            }

            VersionCheckerCallback = versionCheckerCallback;

            // 向服务器请求版本信息
            GameStart.Instance.StartCoroutine(
                CheckVersionRequest(UtilText.Format(ConfigVersion.CheckVersionUrl, GetPlatformPath())));
        }

        private static IEnumerator CheckVersionRequest(string fileUri)
        {
            byte[] bytes = null;
            string errorMessage = null;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                errorMessage = unityWebRequest.error;
                Log.Fatal(UtilText.Format("Check version failure, error message is '{0}'.", errorMessage));
                VersionCheckerCallback(false, null);
            }
            else
            {
                bytes = unityWebRequest.downloadHandler.data;
                OnWebRequestSuccess(bytes);
                VersionCheckerCallback(true, VersionInfo);
            }
            unityWebRequest.Dispose();
        }

        private static void OnWebRequestSuccess(byte[] versionInfoBytes)
        {
            string versionInfoString = UtilConverter.GetString(versionInfoBytes);
            VersionInfo = JsonUtility.FromJson<VersionInfo>(versionInfoString);
            if (VersionInfo == null)
            {
                Log.Error("Parse VersionInfo failure.");
                return;
            }

            string str = UtilText.Format("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.",
                VersionInfo.LatestGameVersion, VersionInfo.InternalGameVersion, Application.version, 0);
            Log.Info(str);

            if (VersionInfo.UpdateGame)
            {
                return;
            }

            VersionInfo.UpdateVersion = CheckVersionList(VersionInfo.InternalResourceVersion) ==
                                        CheckVersionListResult.NeedUpdate;
        }

        /// <summary>
        /// 检查版本资源列表。
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号。</param>
        /// <returns>检查版本资源列表结果。</returns>
        public static CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {

            string versionListFileName = UtilPath.GetRegularPath(Path.Combine(Application.persistentDataPath,
                ConfigVersion.RemoteVersionListFileName));
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
                if (!UpdatableVersionListSerializer.TryGetValue(fileStream, "InternalResourceVersion",
                    out internalResourceVersionObject))
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

        public static void UpdateVersionList(Action<bool, string> versionUpdateCallback)
        {
            VersionUpdateCallback = versionUpdateCallback;
            string localVersionListFilePath = UtilPath.GetRegularPath(Path.Combine(Application.persistentDataPath,
                ConfigVersion.RemoteVersionListFileName));
            int dotPosition = ConfigVersion.RemoteVersionListFileName.LastIndexOf('.');

            string latestVersionListFullNameWithCrc32 = UtilText.Format("{0}.{2:x8}.{1}",
                ConfigVersion.RemoteVersionListFileName.Substring(0, dotPosition),
                ConfigVersion.RemoteVersionListFileName.Substring(dotPosition + 1), VersionInfo.VersionListHashCode);

            string downloadUri =
                UtilPath.GetRemotePath(
                    Path.Combine(VersionInfo.UpdatePrefixUri, latestVersionListFullNameWithCrc32));
            GameEntry.DownLoad.AddDownload(localVersionListFilePath, downloadUri, DownloadCallBack);
        }

        private static void OnDownloadSuccess(DownloadInfo info)
        {
            using (FileStream fileStream = new FileStream(info.DownloadPath, FileMode.Open, FileAccess.ReadWrite))
            {
                int length = (int)fileStream.Length;
                if (length != VersionInfo.VersionListZipLength)
                {
                    fileStream.Close();
                    info.ErrMsg =
                        UtilText.Format("Latest version list zip length error, need '{0}', downloaded '{1}'.",
                            VersionInfo.VersionListZipLength, length);
                    OnDownloadFailure(info);

                    return;
                }

                fileStream.Position = 0L;
                int hashCode = UtilVerifier.GetCrc32(fileStream);
                if (hashCode != VersionInfo.VersionListZipHashCode)
                {
                    fileStream.Close();
                    info.ErrMsg =
                        UtilText.Format("Latest version list zip hash code error, need '{0}', downloaded '{1}'.",
                            VersionInfo.VersionListZipHashCode, hashCode);
                    OnDownloadFailure(info);
                    return;
                }

                if (GameEntry.ResUpdater.DecompressCachedStream == null)
                {
                    GameEntry.ResUpdater.DecompressCachedStream = new MemoryStream();
                }

                try
                {
                    fileStream.Position = 0L;
                    GameEntry.ResUpdater.DecompressCachedStream.Position = 0L;
                    GameEntry.ResUpdater.DecompressCachedStream.SetLength(0L);
                    if (!UtilZip.Decompress(fileStream, GameEntry.ResUpdater.DecompressCachedStream))
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format("Unable to decompress latest version list '{0}'.",
                            info.DownloadPath);
                        OnDownloadFailure(info);
                        return;
                    }

                    if (GameEntry.ResUpdater.DecompressCachedStream.Length != VersionInfo.VersionListLength)
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format(
                            "Latest version list length error, need '{0}', downloaded '{1}'.",
                            VersionInfo.VersionListLength, GameEntry.ResUpdater.DecompressCachedStream.Length);
                        OnDownloadFailure(info);
                        return;
                    }

                    fileStream.Position = 0L;
                    fileStream.SetLength(0L);
                    fileStream.Write(GameEntry.ResUpdater.DecompressCachedStream.GetBuffer(),
                        0, (int)GameEntry.ResUpdater.DecompressCachedStream.Length);
                }
                catch (Exception exception)
                {
                    fileStream.Close();
                    info.ErrMsg =
                        UtilText.Format("Unable to decompress latest version list '{0}' with error message '{1}'.",
                            info.DownloadPath, exception);
                    OnDownloadFailure(info);
                    return;
                }
                finally
                {
                    GameEntry.ResUpdater.DecompressCachedStream.Position = 0L;
                    GameEntry.ResUpdater.DecompressCachedStream.SetLength(0L);
                }
            }

            VersionUpdateCallback(true, null);
        }

        private static void OnDownloadFailure(DownloadInfo info)
        {
            if (File.Exists(info.DownloadPath))
            {
                File.Delete(info.DownloadPath);
            }

            VersionUpdateCallback(false, info.ErrMsg);
        }

        private static string GetPlatformPath()
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
                    throw new NotSupportedException(UtilText.Format("Platform '{0}' is not supported.",
                        Application.platform.ToString()));
            }
        }
    }
}