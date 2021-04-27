using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;
using EPloy.Res;
using UnityEngine;

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
                OnCheckResComplete(0, 0, 0, 0, 0);
            }
            else
            {
                Game.ResUpdater.CheckRes(OnCheckResComplete);
            }
            return;
#endif
            // 1. 校验版本
            Game.VersionChecker.VersionChecker(VersionCheckerCallback);
        }

        private void VersionCheckerCallback(bool result, VersionInfo versionInfo)
        {
            if (!result)
            {
                return;
            }
            // 2.1  强制更新游戏
            if (versionInfo.UpdateGame)
            {

                return;
            }
            // 2.2 更新游戏资源列表  或者 跳过到底3步
            if (versionInfo.UpdateVersion)
            {
                Game.VersionChecker.UpdateVersionList(VersionUpdateCallback);
            }
            else
            {
                Game.ResUpdater.CheckRes(OnCheckResComplete);
            }
        }

        private void VersionUpdateCallback(bool result, string msg)
        {
            if (result)
            {
                // 3. 校验资源 准备更新列表
                Game.ResUpdater.CheckRes(OnCheckResComplete);
                return;
            }
            Log.Fatal(msg);
        }

        private void OnCheckResComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            Log.Info(Utility.Text.Format("Check resources complete, '{0}' resources need to update, compressed length is '{1}', uncompressed length is '{2}'.",
            updateCount, updateTotalZipLength, updateTotalLength));

            if (updateCount == 0)
            {
                Game.ILRuntime.StartILRuntime(IsILRuntime);
                return;
            }
            // 4. 更新资源
            UpdateRes(updateCount, updateTotalZipLength);
        }

        private void UpdateRes(long UpdateResCount, long UpdateReTotalZipLength)
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                // 网络环境判断 是否更新  否则关掉游戏

                return;
            }

            // 开ui

            Log.Info("Start update resources...");
            Game.ResUpdater.UpdateRes(OnUpdateResComplete);
        }

        private void OnUpdateResComplete(ResGroup resourceGroup, bool result)
        {
            if (result)
            {
                Game.ILRuntime.StartILRuntime(IsILRuntime);
                Log.Info("Update resources complete with no errors.");
            }
            else
            {
                Log.Error("Update resources complete with errors.");
            }
        }

        private string GetByteLengthString(long byteLength)
        {
            if (byteLength < 1024L) // 2 ^ 10
            {
                return Utility.Text.Format("{0} Bytes", byteLength.ToString());
            }

            if (byteLength < 1048576L) // 2 ^ 20
            {
                return Utility.Text.Format("{0} KB", (byteLength / 1024f).ToString("F2"));
            }

            if (byteLength < 1073741824L) // 2 ^ 30
            {
                return Utility.Text.Format("{0} MB", (byteLength / 1048576f).ToString("F2"));
            }

            if (byteLength < 1099511627776L) // 2 ^ 40
            {
                return Utility.Text.Format("{0} GB", (byteLength / 1073741824f).ToString("F2"));
            }

            if (byteLength < 1125899906842624L) // 2 ^ 50
            {
                return Utility.Text.Format("{0} TB", (byteLength / 1099511627776f).ToString("F2"));
            }

            if (byteLength < 1152921504606846976L) // 2 ^ 60
            {
                return Utility.Text.Format("{0} PB", (byteLength / 1125899906842624f).ToString("F2"));
            }

            return Utility.Text.Format("{0} EB", (byteLength / 1152921504606846976f).ToString("F2"));
        }

        private class UpdateLengthData
        {
            private readonly string m_Name;

            public UpdateLengthData(string name)
            {
                m_Name = name;
            }

            public string Name
            {
                get
                {
                    return m_Name;
                }
            }

            public int Length
            {
                get;
                set;
            }
        }
    }
}