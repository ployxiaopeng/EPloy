
using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class BaseComponentAwakeSystem : AwakeSystem<BaseComponent, bool>
    {
        public override void Awake(BaseComponent self, bool a)
        {
            self.Awake(a);
        }
    }
    [ObjectSystem]
    public class BaseComponentDestroySystem : DestroySystem<BaseComponent>
    {
        public override void Destroy(BaseComponent self)
        {
            //self.OnDestroy();
        }
    }
    [ObjectSystem]
    public class BaseComponentUpdateSystem : UpdateSystem<BaseComponent>
    {
        public override void Update(BaseComponent self)
        {
            self.Update();
        }
    }

    public static class BaseComponentSystem
    {
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void Awake(this BaseComponent self, bool isEditorResource)
        {
            self.InitVersionHelper(); self.InitLogHelper();
            self.InitZipHelper(); self.InitJsonHelper();
            self.InitProfilerHelper();
            Log.Info("Game Framework Version: {0}", GameFramework.Version.GameFrameworkVersion);
            Log.Info("Game Version: {0} ({1})", GameFramework.Version.GameVersion, GameFramework.Version.InternalGameVersion.ToString());
            Log.Info("Unity Version: {0}", Application.unityVersion);

            Utility.Converter.ScreenDpi = Screen.dpi;
            if (Utility.Converter.ScreenDpi <= 0) Utility.Converter.ScreenDpi = BaseComponent.DefaultDpi;

            self.EditorResourceMode = isEditorResource;
            self.EditorResourceMode &= Application.isEditor;
            if (self.EditorResourceMode) Log.Info("当前为编辑器模式资源加载");
            else Log.Info("当前为正常模式资源加载");

            Application.targetFrameRate = self.FrameRate;
            Time.timeScale = self.GameSpeed;
            Application.runInBackground = self.RunInBackground;
            Screen.sleepTimeout = self.NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

            Application.lowMemory += self.OnLowMemory;
        }

        public static void Update(this BaseComponent self)
        {
            try
            {
                GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public static void OnDestroy(this BaseComponent self)
        {
#if UNITY_5_6_OR_NEWER
            Application.lowMemory -= self.OnLowMemory;
#endif
            GameFrameworkEntry.Shutdown();
        }

        public static void OnLowMemory(this BaseComponent self)
        {
            Log.Info("Low memory reported...");
            Init.ObjectPool.ReleaseAllUnused();
            Init.Resource.ForceUnloadUnusedAssets(true);
        }

        #region 辅助类
        public static string m_VersionHelperTypeName = "ETModel.DefaultVersionHelper";
        public static string m_LogHelperTypeName = "ETModel.DefaultLogHelper";
        public static string m_ZipHelperTypeName = "ETModel.DefaultZipHelper";
        public static string m_JsonHelperTypeName = "ETModel.DefaultJsonHelper";
        public static string m_ProfilerHelperTypeName = "ETModel.DefaultProfilerHelper";
        private static void InitVersionHelper(this BaseComponent self)
        {
            Type versionHelperType = Utility.Assembly.GetType(m_VersionHelperTypeName);
            if (versionHelperType == null)
            {
                Log.Error(Utility.Text.Format("Can not find version helper type '{0}'.", m_VersionHelperTypeName));
                return;
            }
            GameFramework.Version.IVersionHelper versionHelper = (GameFramework.Version.IVersionHelper)Activator.CreateInstance(versionHelperType);
            if (versionHelper == null)
            {
                Log.Error(Utility.Text.Format("Can not create version helper instance '{0}'.", m_VersionHelperTypeName));
                return;
            }
            GameFramework.Version.SetVersionHelper(versionHelper);
        }
        private static void InitLogHelper(this BaseComponent self)
        {
            Type logHelperType = Utility.Assembly.GetType(m_LogHelperTypeName);
            if (logHelperType == null)
            {
                Log.Error(Utility.Text.Format("Can not find log helper type '{0}'.", m_LogHelperTypeName));
                return;
            }
            GameFrameworkLog.ILogHelper logHelper = (GameFrameworkLog.ILogHelper)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
            {
                Log.Error(Utility.Text.Format("Can not create log helper instance '{0}'.", m_LogHelperTypeName));
                return;
            }
            GameFrameworkLog.SetLogHelper(logHelper);
        }
        private static void InitZipHelper(this BaseComponent self)
        {

            Type zipHelperType = Utility.Assembly.GetType(m_ZipHelperTypeName);
            if (zipHelperType == null)
            {
                Log.Error("Can not find Zip helper type '{0}'.", m_ZipHelperTypeName);
                return;
            }
            Utility.Zip.IZipHelper zipHelper = (Utility.Zip.IZipHelper)Activator.CreateInstance(zipHelperType);
            if (zipHelper == null)
            {
                Log.Error("Can not create Zip helper instance '{0}'.", m_ZipHelperTypeName);
                return;
            }
            Utility.Zip.SetZipHelper(zipHelper);
        }
        private static void InitJsonHelper(this BaseComponent self)
        {

            Type jsonHelperType = Utility.Assembly.GetType(m_JsonHelperTypeName);
            if (jsonHelperType == null)
            {
                Log.Error("Can not find JSON helper type '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.IJsonHelper jsonHelper = (Utility.Json.IJsonHelper)Activator.CreateInstance(jsonHelperType);
            if (jsonHelper == null)
            {
                Log.Error("Can not create JSON helper instance '{0}'.", m_JsonHelperTypeName);
                return;
            }
            Utility.Json.SetJsonHelper(jsonHelper);
        }
        private static void InitProfilerHelper(this BaseComponent self)
        {

            Type profilerHelperType = Utility.Assembly.GetType(m_ProfilerHelperTypeName);
            if (profilerHelperType == null)
            {
                Log.Error("Can not find profiler helper type '{0}'.", m_ProfilerHelperTypeName);
                return;
            }
            Utility.Profiler.IProfilerHelper profilerHelper = (Utility.Profiler.IProfilerHelper)Activator.CreateInstance(profilerHelperType, Thread.CurrentThread);
            if (profilerHelper == null)
            {
                Log.Error("Can not create profiler helper instance '{0}'.", m_ProfilerHelperTypeName);
                return;
            }
            Utility.Profiler.SetProfilerHelper(profilerHelper);
        }
        #endregion
    }
}
