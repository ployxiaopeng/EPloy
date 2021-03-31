
using GameFramework;
using GameFramework.Event;
using GameFramework.Procedure;
using GameFramework.Resource;
using GameFramework.WebRequest;
using LitJson;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace ETModel
{
    public class ProcedureCheckVersion : ProcedureBase
    {
        private bool InitResourcesComplete = false;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            m_UpdateVersionListCallbacks = new UpdateVersionListCallbacks(OnUpdateVersionListSuccess, OnUpdateVersionListFailure);
        }
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            InitResourcesComplete = false;
            Log.Info("正在检擦更新");
            Init.WebRequest.WebRequestManager.WebRequestSuccess += OnWebRequestSuccess;
            Init.WebRequest.WebRequestManager.WebRequestFailure += OnWebRequestFailure;

            if (Init.Resource.ResourceMode == ResourceMode.Updatable)
            {
                RequestVersion();
                return;
            }

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //可更新模式检查版本信息，否则直接初始化资源
                if (Init.Resource.ResourceMode == ResourceMode.Updatable)
                    RequestVersion();
                else Init.Resource.InitResources(OnInitResourcesComplete);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (Init.Base.EditorResourceMode) OnInitResourcesComplete();
                else Init.Resource.InitResources(OnInitResourcesComplete);
            }
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (InitResourcesComplete)
            {
                asyncLoad();
                InitResourcesComplete = false;
            }
            if (LatestVersionComplete)
            {
                ChangeState<ProcedureUpdateResource>(procedureOwner);
            }
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            Init.WebRequest.WebRequestManager.WebRequestSuccess -= OnWebRequestSuccess;
            Init.WebRequest.WebRequestManager.WebRequestFailure -= OnWebRequestFailure;

            base.OnLeave(procedureOwner, isShutdown);
        }
        private async void asyncLoad()
        {
            Log.Info("当前不需要更新");
            await Init.Sound.StartAsync();
            Init.ILRuntime.StartAsync();
        }

        private void OnInitResourcesComplete()
        {
            Init.Event.Subscribe(ShowUIEvent<string>.EventId, OpenUIWnd);
            ShowUIEvent<string> uIEvent = ReferencePool.Acquire<ShowUIEvent<string>>();
            uIEvent.SetEventData("CheckVersion");
            Init.Event.Fire(ShowUIEvent<string>.EventId, uIEvent);
        }
        private void OpenUIWnd(object sender, GameEventArgs e)
        {
            if ((int)sender != ShowUIEvent<string>.EventId) return;
            ShowUIEvent<string> ne = (ShowUIEvent<string>)e;
            if (ne.uiWnd != "CheckVersion") return;
            InitResourcesComplete = true;
            Init.Event.Unsubscribe(ShowUIEvent<string>.EventId, OpenUIWnd);
        }

        #region 资源更新
        private bool LatestVersionComplete = false;
        private VersionInfo m_VersionInfo = null;
        private UpdateVersionListCallbacks m_UpdateVersionListCallbacks = null;
        private void RequestVersion()
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            string deviceName = SystemInfo.deviceName;
            string deviceModel = SystemInfo.deviceModel;
            string processorType = SystemInfo.processorType;
            string processorCount = SystemInfo.processorCount.ToString();
            string memorySize = SystemInfo.systemMemorySize.ToString();
            string operatingSystem = SystemInfo.operatingSystem;
            string iOSGeneration = string.Empty;
            string iOSSystemVersion = string.Empty;
            string iOSVendorIdentifier = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
            iOSGeneration = UnityEngine.iOS.Device.generation.ToString();
            iOSSystemVersion = UnityEngine.iOS.Device.systemVersion;
            iOSVendorIdentifier = UnityEngine.iOS.Device.vendorIdentifier ?? string.Empty;
#endif
            string gameVersion = Version.GameVersion;
            string platform = Application.platform.ToString();
            string unityVersion = Application.unityVersion;
            string installMode = Application.installMode.ToString();
            string sandboxType = Application.sandboxType.ToString();
            string screenWidth = Screen.width.ToString();
            string screenHeight = Screen.height.ToString();
            string screenDpi = Screen.dpi.ToString();
            string screenOrientation = Screen.orientation.ToString();
            string screenResolution = Utility.Text.Format("{0} x {1} @ {2}Hz", Screen.currentResolution.width.ToString(), Screen.currentResolution.height.ToString(), Screen.currentResolution.refreshRate.ToString());
            string useWifi = (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork).ToString();

            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("DeviceId", WebUtility.EscapeString(deviceId));
            wwwForm.AddField("DeviceName", WebUtility.EscapeString(deviceName));
            wwwForm.AddField("DeviceModel", WebUtility.EscapeString(deviceModel));
            wwwForm.AddField("ProcessorType", WebUtility.EscapeString(processorType));
            wwwForm.AddField("ProcessorCount", WebUtility.EscapeString(processorCount));
            wwwForm.AddField("MemorySize", WebUtility.EscapeString(memorySize));
            wwwForm.AddField("OperatingSystem", WebUtility.EscapeString(operatingSystem));
            wwwForm.AddField("IOSGeneration", WebUtility.EscapeString(iOSGeneration));
            wwwForm.AddField("IOSSystemVersion", WebUtility.EscapeString(iOSSystemVersion));
            wwwForm.AddField("IOSVendorIdentifier", WebUtility.EscapeString(iOSVendorIdentifier));
            wwwForm.AddField("GameVersion", WebUtility.EscapeString(gameVersion));
            wwwForm.AddField("Platform", WebUtility.EscapeString(platform));
            wwwForm.AddField("UnityVersion", WebUtility.EscapeString(unityVersion));
            wwwForm.AddField("InstallMode", WebUtility.EscapeString(installMode));
            wwwForm.AddField("SandboxType", WebUtility.EscapeString(sandboxType));
            wwwForm.AddField("ScreenWidth", WebUtility.EscapeString(screenWidth));
            wwwForm.AddField("ScreenHeight", WebUtility.EscapeString(screenHeight));
            wwwForm.AddField("ScreenDPI", WebUtility.EscapeString(screenDpi));
            wwwForm.AddField("ScreenOrientation", WebUtility.EscapeString(screenOrientation));
            wwwForm.AddField("ScreenResolution", WebUtility.EscapeString(screenResolution));
            wwwForm.AddField("UseWifi", WebUtility.EscapeString(useWifi));

            //发送web请求，获取服务器上的版本信息（version.txt）
            Init.WebRequest.AddWebRequest("http://192.168.1.107/Res/version_Win64.txt", this);
        }

        /// <summary>
        /// 跳转到整包更新的网址
        /// </summary>
        private void GotoUpdateApp(object userData)
        {
            string url = null;
            if (url != null) ;
            Application.OpenURL(url);
        }

        /// <summary>
        /// 获取资源版本名称
        /// </summary>
        private string GetResourceVersionName()
        {

            string[] splitApplicableGameVersion = Version.GameVersion.Split('.');
            if (splitApplicableGameVersion.Length != 3)
            {
                return string.Empty;
            }

            return Utility.Text.Format("{0}_{1}_{2}_{3}", splitApplicableGameVersion[0], splitApplicableGameVersion[1], splitApplicableGameVersion[2], m_VersionInfo.InternalResourceVersion.ToString());
        }

        /// <summary>
        /// 获取平台路径
        /// </summary>
        private string GetPlatformPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "windows";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "osx";
                case RuntimePlatform.IPhonePlayer:
                    return "ios";
                case RuntimePlatform.Android:
                    return "android";
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
                    return "winstore";
                default:
                    return string.Empty;
            }
        }

        private void UpdateVersion()
        {
            if (Init.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) == CheckVersionListResult.Updated)
            {
                LatestVersionComplete = true;
            }
            else
            {
                Init.Resource.UpdateVersionList(m_VersionInfo.VersionListLength, m_VersionInfo.VersionListHashCode, m_VersionInfo.VersionListZipLength, m_VersionInfo.VersionListZipHashCode, m_UpdateVersionListCallbacks);
            }
        }

        private void OnWebRequestSuccess(object sender, WebRequestSuccessEventArgs e)
        {
            //解析服务器发来的版本信息
            string strResult = Utility.Converter.GetString(e.GetWebResponseBytes());
            m_VersionInfo = JsonMapper.ToObject<VersionInfo>(strResult);
            if (m_VersionInfo == null)
            {
                Log.Error("Parse VersionInfo failure.");
                return;
            }
            Log.Info("Latest game version is '{0}', local game version is '{1}'.", m_VersionInfo.LatestGameVersion, Version.GameVersion);
            //是否需要整包更新
            if (m_VersionInfo.ForceGameUpdate)
            {
                //TODO:进行整包更新的相关操作
                //GotoUpdateApp(null);
                return;
            }
            //设置资源更新URL
            Init.Resource.UpdatePrefixUri = Utility.Path.GetCombinePath(m_VersionInfo.GameUpdateUrl, GetPlatformPath());
            Log.Error(Init.Resource.UpdatePrefixUri);
            //更新版本资源列表
            UpdateVersion();
        }
        private void OnWebRequestFailure(object sender, WebRequestFailureEventArgs e)
        {
            Log.Error(e.ErrorMessage);
        }

        private void OnUpdateVersionListSuccess(string downloadPath, string downloadUri)
        {
            LatestVersionComplete = true;
            Log.Info("Update latest version list from '{0}' success.", downloadUri);
        }
        private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
        {
            Log.Warning("Update latest version list from '{0}' failure, error message '{1}'.", downloadUri, errorMessage);
        }
        #endregion
    }
}