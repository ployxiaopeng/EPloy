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
    public class ProcedureModule : IGameModule
    {
        private VersionInfo VersionInfo = null;

        private long UpdateResCount = 0;
        private long UpdateResTotalCount = 0L;
        private int UpdateResSuccessCount = 0;
        private List<UpdateResData> UpdateResDatas = new List<UpdateResData>();

        public bool IsILRuntime { get; private set; }
        public bool EditorResource { get; private set; }

        public  void Awake()
        {

        }

        public  void Update()
        {

        }

        public  void OnDestroy()
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
                GameModule.ResUpdater.CheckRes(OnCheckResComplete);
            }
            return;
#endif
            // 1. 校验版本
            GameModule.VersionChecker.VersionChecker(VersionCheckerCallback);
        }

        private void VersionCheckerCallback(bool result, VersionInfo versionInfo)
        {
            if (!result)
            {
                return;
            }
            VersionInfo = versionInfo;
            GameModule.ResUpdater.UpdatePrefixUri = VersionInfo.UpdatePrefixUri;
            // 2.1  强制更新游戏
            if (versionInfo.UpdateGame)
            {

                return;
            }
            // 2.2 更新游戏资源列表  或者 跳过到底3步
            if (versionInfo.UpdateVersion)
            {
                GameModule.VersionChecker.UpdateVersionList(VersionUpdateCallback);
            }
            else
            {
                GameModule.ResUpdater.CheckRes(OnCheckResComplete);
            }
        }

        private void VersionUpdateCallback(bool result, string msg)
        {
            if (result)
            {
                // 3. 校验资源 准备更新列表
                GameModule.ResUpdater.CheckRes(OnCheckResComplete);
                return;
            }
            Log.Fatal(msg);
        }

        private void OnCheckResComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            Log.Info(Utility.Text.Format("Check  complete, '{0}'  need to update, zip length is '{1}', unZip length is '{2}'.",
            updateCount, updateTotalZipLength, updateTotalLength));

            if (updateCount == 0)
            {
                GameModule.ILRuntime.StartILRuntime(IsILRuntime);
                return;
            }
            // 4. 更新资源
            UpdateRes(updateCount, updateTotalZipLength);
        }

        private void UpdateRes(long updateResCount, long updateTotalZipLength)
        {
            this.UpdateResTotalCount = updateTotalZipLength;
            UpdateResCount = updateResCount;
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                // 网络环境判断 是否更新  否则关掉游戏

                return;
            }

            // 开ui
            Log.Info("Start update resources...");
            UpdateResCallBack updateResCallBack = new UpdateResCallBack(OnResUpdateStart, OnResUpdateChanged, OnResUpdateSuccess
            , OnResourceUpdateFailure, OnUpdateResComplete);
            GameModule.ResUpdater.UpdateRes(updateResCallBack);
        }

        private void OnUpdateResComplete(bool result)
        {
            if (result)
            {
                GameModule.ILRuntime.StartILRuntime(IsILRuntime);
                Log.Info("Update resources complete with no errors.");
            }
            else
            {
                Log.Error("Update resources complete with errors.");
            }
        }

        private void OnResUpdateStart(string resName)
        {
            for (int i = 0; i < UpdateResDatas.Count; i++)
            {
                if (UpdateResDatas[i].Name == resName)
                {
                    Log.Warning(Utility.Text.Format("Update resource '{0}' is invalid.", resName));
                    UpdateResDatas[i].Length = 0;
                    RefreshProgress();
                    return;
                }
            }

            UpdateResDatas.Add(new UpdateResData(resName));
        }

        private void OnResUpdateChanged(string resName, int currentLength)
        {
            for (int i = 0; i < UpdateResDatas.Count; i++)
            {
                if (UpdateResDatas[i].Name == resName)
                {
                    UpdateResDatas[i].Length = currentLength;
                    RefreshProgress();
                    return;
                }
            }

            Log.Warning(Utility.Text.Format("Update resource '{0}' is invalid.", resName));
        }

        private void OnResUpdateSuccess(string resName, int zipLength)
        {
            Log.Info(Utility.Text.Format("Update resource '{0}' success.", resName));
            for (int i = 0; i < UpdateResDatas.Count; i++)
            {
                if (UpdateResDatas[i].Name == resName)
                {
                    UpdateResDatas[i].Length = zipLength;
                    UpdateResSuccessCount++;
                    RefreshProgress();
                    return;
                }
            }

            Log.Warning(Utility.Text.Format("Update resource '{0}' is invalid.", resName));
        }

        private void OnResourceUpdateFailure(string resName, string errMsg, int retryCount, int totalRetryCount)
        {
            Log.Error(Utility.Text.Format("Update resource '{0}' error message '{2}', retry count '{3}'.", resName, errMsg, retryCount));
            if (retryCount >= totalRetryCount)
            {
                return;
            }

            for (int i = 0; i < UpdateResDatas.Count; i++)
            {
                if (UpdateResDatas[i].Name == resName)
                {
                    UpdateResDatas.Remove(UpdateResDatas[i]);
                    RefreshProgress();
                    return;
                }
            }

            Log.Warning(Utility.Text.Format("Update resource '{0}' is invalid.", resName));
        }

        private void RefreshProgress()
        {
            long currentTotalUpdateLength = 0L;
            for (int i = 0; i < UpdateResDatas.Count; i++)
            {
                currentTotalUpdateLength += UpdateResDatas[i].Length;
            }

            float progressTotal = (float)currentTotalUpdateLength / UpdateResTotalCount;
            //   string descriptionText = GameEntry.Localization.GetString("UpdateResource.Tips", m_UpdateSuccessCount.ToString(), m_UpdateCount.ToString(), GetByteLengthString(currentTotalUpdateLength), GetByteLengthString(m_UpdateTotalCompressedLength), progressTotal, GetByteLengthString((int)GameEntry.Download.CurrentSpeed));
            //  m_UpdateResourceForm.SetProgress(progressTotal, descriptionText);
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

        private class UpdateResData
        {
            private readonly string UpdaterHandlerName;

            public UpdateResData(string name)
            {
                UpdaterHandlerName = name;
            }

            public string Name
            {
                get
                {
                    return UpdaterHandlerName;
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