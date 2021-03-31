using GameFramework;
using GameFramework.Event;
using GameFramework.Procedure;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace ETModel
{
    /// <summary>
    /// 更新资源流程
    /// </summary>
    public class ProcedureUpdateResource : ProcedureBase
    {
        private bool m_UpdateAllComplete = false;
        private int m_UpdateCount = 0;
        private long m_UpdateTotalZipLength = 0;
        private int m_UpdateSuccessCount = 0;
        private List<UpdateLengthData> m_UpdateLengthData = new List<UpdateLengthData>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_UpdateAllComplete = false;
            m_UpdateCount = 0;
            m_UpdateTotalZipLength = 0;
            m_UpdateSuccessCount = 0;
            m_UpdateLengthData.Clear();

            Init.Resource.ResourceManager.ResourceUpdateStart += OnResourceUpdateStart;
            Init.Resource.ResourceManager.ResourceUpdateChanged += OnResourceUpdateChanged;
            Init.Resource.ResourceManager.ResourceUpdateSuccess += OnResourceUpdateSuccess;
            Init.Resource.ResourceManager.ResourceUpdateFailure += OnResourceUpdateFailure;

            Init.Resource.CheckResources(OnCheckResourcesComplete);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            Init.Resource.ResourceManager.ResourceUpdateStart -= OnResourceUpdateStart;
            Init.Resource.ResourceManager.ResourceUpdateChanged -= OnResourceUpdateChanged;
            Init.Resource.ResourceManager.ResourceUpdateSuccess -= OnResourceUpdateSuccess;
            Init.Resource.ResourceManager.ResourceUpdateFailure -= OnResourceUpdateFailure;

            base.OnLeave(procedureOwner, isShutdown);
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!m_UpdateAllComplete) return;
            asyncLoad();
            m_UpdateAllComplete = false;
        }
        private async void asyncLoad()
        {
            await Init.Sound.StartAsync();
            Init.ILRuntime.StartAsync();
        }

        private void OnCheckResourcesComplete(bool needUpdateResources, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            Log.Info("Check resources complete, '{0}' resources need to update, zip length is '{1}', unzip length is '{2}'.", updateCount.ToString(), updateTotalZipLength.ToString(), updateTotalLength.ToString());

            m_UpdateCount = updateCount;
            m_UpdateTotalZipLength = updateTotalZipLength;
            if (!needUpdateResources)
            {
                //不需要更新资源
                ProcessUpdateResourcesComplete();
                return;
            }
            //开始更新资源
            StartUpdateResources(null);
        }

        private void ProcessUpdateResourcesComplete()
        {
            m_UpdateAllComplete = true;
        }

        private void StartUpdateResources(object userData)
        {
            Init.Resource.UpdateResources(OnUpdateResourcesComplete);
            Log.Info("Start update resources...");
        }

        private void OnUpdateResourcesComplete()
        {
            Log.Info("Update resources complete.");
            ProcessUpdateResourcesComplete();
        }

        private void OnResourceUpdateStart(object sender, ResourceUpdateStartEventArgs e)
        {
            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == e.Name)
                {
                    Log.Warning("Update resource '{0}' is invalid.", e.Name);
                    m_UpdateLengthData[i].Length = 0;
                    RefreshProgress();
                    return;
                }
            }
            //记录下要更新的这个资源的长度数据
            m_UpdateLengthData.Add(new UpdateLengthData(e.Name));
        }

        private void OnResourceUpdateChanged(object sender, ResourceUpdateChangedEventArgs e)
        {
            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == e.Name)
                {
                    m_UpdateLengthData[i].Length = e.CurrentLength;
                    RefreshProgress();
                    return;
                }
            }

            Log.Error("Update resource '{0}' is invalid.", e.Name);
        }

        private void OnResourceUpdateSuccess(object sender, ResourceUpdateSuccessEventArgs e)
        {
            Log.Info("Update resource '{0}' success.", e.Name);

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == e.Name)
                {
                    m_UpdateLengthData[i].Length = e.ZipLength;
                    m_UpdateSuccessCount++;
                    RefreshProgress();
                    return;
                }
            }

            Log.Error("Update resource '{0}' is invalid.", e.Name);
        }

        private void OnResourceUpdateFailure(object sender, ResourceUpdateFailureEventArgs e)
        {
            if (e.RetryCount >= e.TotalRetryCount)
            {
                Log.Error("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", e.Name, e.DownloadUri, e.ErrorMessage, e.RetryCount.ToString());
                return;
            }
            else
            {
                Log.Info("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", e.Name, e.DownloadUri, e.ErrorMessage, e.RetryCount.ToString());
            }

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == e.Name)
                {
                    m_UpdateLengthData.Remove(m_UpdateLengthData[i]);
                    RefreshProgress();
                    return;
                }
            }

            Log.Error("Update resource '{0}' is invalid.", e.Name);
        }


        /// <summary>
        /// 刷新进度
        /// </summary>
        private void RefreshProgress()
        {
            int currentTotalUpdateLength = 0;
            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                currentTotalUpdateLength += m_UpdateLengthData[i].Length;
            }
            //计算更新进度
            float progressTotal = (float)currentTotalUpdateLength / m_UpdateTotalZipLength;

            //获取更新描述文本
            Log.Info("{0}  {1}  {2}  {3}  {4}  {5}", m_UpdateSuccessCount.ToString(), m_UpdateCount.ToString(), GetLengthString(currentTotalUpdateLength),
                GetLengthString(m_UpdateTotalZipLength), progressTotal, GetLengthString((int)Init.Download.CurrentSpeed));
        }

        /// <summary>
        /// 获取长度字符串
        /// </summary>
        private string GetLengthString(long length)
        {
            if (length < 1024)
            {
                return Utility.Text.Format("{0} Bytes", length.ToString());
            }

            if (length < 1024 * 1024)
            {
                return Utility.Text.Format("{0} KB", (length / 1024f).ToString("F2"));
            }

            if (length < 1024 * 1024 * 1024)
            {
                return Utility.Text.Format("{0} MB", (length / 1024f / 1024f).ToString("F2"));
            }

            return Utility.Text.Format("{0} GB", (length / 1024f / 1024f / 1024f).ToString("F2"));
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
