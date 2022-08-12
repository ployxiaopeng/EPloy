using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using EPloy.Game.Res;
using EPloy.Game.Download;

namespace EPloy.Game
{
    /// <summary>
    /// 版本校验。
    /// </summary>
    public class VersionCheckerModule : IGameModule
    {
        private UpdatableVersionListSerializer UpdatableVersionListSerializer;
        private VersionInfo VersionInfo;
        private DownloadCallBack DownloadCallBack;

        private Action<bool, VersionInfo> VersionCheckerCallback;
        private Action<bool, string> VersionUpdateCallback;

        public void Awake()
        {
            VersionInfo = null;
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            DownloadCallBack = new DownloadCallBack(OnDownloadSuccess, OnDownloadFailure);
            VersionCheckerCallback = null;
            VersionUpdateCallback = null;
        }

        public void Update()
        {

        }

        public void OnDestroy()
        {
            VersionInfo = null;
            UpdatableVersionListSerializer = null;
            DownloadCallBack = null;
            VersionCheckerCallback = null;
            VersionUpdateCallback = null;
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
            GameStart.Instance.StartCoroutine(
                CheckVersionRequest(UtilText.Format(ConfigVersion.CheckVersionUrl, GetPlatformPath())));
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
                Log.Fatal(UtilText.Format("Check version failure, error message is '{0}'.", errorMessage));
                VersionCheckerCallback(false, null);
            }
        }

        private void OnWebRequestSuccess(byte[] versionInfoBytes)
        {

            string versionInfoString = UtilConverter.GetString(versionInfoBytes);
            VersionInfo = JsonMapper.ToObject<VersionInfo>(versionInfoString);
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
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
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

                internalResourceVersion = (int) internalResourceVersionObject;
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


        public void UpdateVersionList(Action<bool, string> versionUpdateCallback)
        {
            this.VersionUpdateCallback = versionUpdateCallback;
            string localVersionListFilePath = UtilPath.GetRegularPath(Path.Combine(Application.persistentDataPath,
                ConfigVersion.RemoteVersionListFileName));
            int dotPosition = ConfigVersion.RemoteVersionListFileName.LastIndexOf('.');

            string latestVersionListFullNameWithCrc32 = UtilText.Format("{0}.{2:x8}.{1}",
                ConfigVersion.RemoteVersionListFileName.Substring(0, dotPosition),
                ConfigVersion.RemoteVersionListFileName.Substring(dotPosition + 1), VersionInfo.VersionListHashCode);

            string downloadUri =
                UtilPath.GetRemotePath(
                    Path.Combine(VersionInfo.UpdatePrefixUri, latestVersionListFullNameWithCrc32));
            GameModule.DownLoad.AddDownload(localVersionListFilePath, downloadUri, DownloadCallBack);
        }

        private void OnDownloadSuccess(DownloadInfo info)
        {
            using (FileStream fileStream = new FileStream(info.DownloadPath, FileMode.Open, FileAccess.ReadWrite))
            {
                int length = (int) fileStream.Length;
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

                if (GameModule.ResUpdater.DecompressCachedStream == null)
                {
                    GameModule.ResUpdater.DecompressCachedStream = new MemoryStream();
                }

                try
                {
                    fileStream.Position = 0L;
                    GameModule.ResUpdater.DecompressCachedStream.Position = 0L;
                    GameModule.ResUpdater.DecompressCachedStream.SetLength(0L);
                    if (!UtilZip.Decompress(fileStream, GameModule.ResUpdater.DecompressCachedStream))
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format("Unable to decompress latest version list '{0}'.",
                            info.DownloadPath);
                        OnDownloadFailure(info);
                        return;
                    }

                    if (GameModule.ResUpdater.DecompressCachedStream.Length != VersionInfo.VersionListLength)
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format(
                            "Latest version list length error, need '{0}', downloaded '{1}'.",
                            VersionInfo.VersionListLength, GameModule.ResUpdater.DecompressCachedStream.Length);
                        OnDownloadFailure(info);
                        return;
                    }

                    fileStream.Position = 0L;
                    fileStream.SetLength(0L);
                    fileStream.Write(GameModule.ResUpdater.DecompressCachedStream.GetBuffer(),
                        0, (int) GameModule.ResUpdater.DecompressCachedStream.Length);
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
                    GameModule.ResUpdater.DecompressCachedStream.Position = 0L;
                    GameModule.ResUpdater.DecompressCachedStream.SetLength(0L);
                }
            }

            this.VersionUpdateCallback(true, null);
        }

        private void OnDownloadFailure(DownloadInfo info)
        {
            if (File.Exists(info.DownloadPath))
            {
                File.Delete(info.DownloadPath);
            }

            this.VersionUpdateCallback(false, info.ErrMsg);
        }

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
                    throw new NotSupportedException(UtilText.Format("Platform '{0}' is not supported.",
                        Application.platform.ToString()));
            }
        }
    }
}