using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;

namespace EPloy
{
    /// <summary>
    /// 流程模块。
    /// </summary>
    public class ProcedureModule : EPloyModule
    {
        private bool CheckVersionComplete = false;
        private bool NeedUpdateVersion = false;
        private VersionInfo VersionInfo = null;


        public bool IsILRuntime { get; private set; }

        public bool EditorResource { get; private set; }


        public override void Awake()
        {

        }

        public override void Update()
        {

        }

        public override void OnDestroy()
        {

        }


        public void StartGame(bool isILRuntime, bool editorResource)
        {
            IsILRuntime = isILRuntime; EditorResource = editorResource;
#if UNITY_EDITOR
            if (EditorResource)
            {
                CheckResCallback(true, null);
            }
            else
            {

            }
            return;
#endif
            Game.VersionChecker.VersionChecker(VersionCheckerCallback);
        }

        private void VersionCheckerCallback(bool result, VersionInfo versionInfo)
        {
            if (!result)
            {
                return;
            }
            if (versionInfo.UpdateGame)
            {
                // 资源列表更新
                Game.VersionChecker.UpdateVersionList(VersionUpdateCallback);
            }
            else
            {
                // 资源校验
                Game.ResUpdater.CheckRes(CheckResCallback);
            }
        }

        private void CheckResCallback(bool result, string msg)
        {
            Game.ResUpdater.CheckRes(CheckResCallback);
            if (result)
            {
                Game.ILRuntime.StartGame(IsILRuntime);
                return;
            }
            Log.Fatal(msg);
        }

        private void VersionUpdateCallback(bool result, string msg)
        {
            if (result)
            {
                // 准备资源更新
                return;
            }
            Log.Fatal(msg);
        }
    }
}