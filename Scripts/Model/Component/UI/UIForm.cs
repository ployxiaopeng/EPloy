//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.UI;
using UnityEngine;
using ETModel;

namespace ETModel
{
    /// <summary>
    /// 界面。
    /// </summary>
    public class UIForm : MonoBehaviour, IUIForm
    {
        private bool m_Available = false;
        private bool m_Visible = false;
        private int m_OriginalLayer = 0;

        /// <summary>
        /// 获取或设置界面名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取界面是否可用。
        /// </summary>
        public bool Available { get; private set; }
        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_Available && m_Visible;
            }
            set
            {
                if (!m_Available)
                {
                    Log.Warning("UI form '{0}' is not available.", Name);
                    return;
                }

                if (m_Visible == value) return;
                m_Visible = value;
                InternalSetVisible(value);
            }
        }
        /// <summary>
        /// 获取界面序列编号。
        /// </summary>
        public int SerialId { get; private set; }
        /// <summary>
        /// 获取界面资源名称。
        /// </summary>
        public string UIFormAssetName { get; private set; }
        /// <summary>
        /// 获取界面实例。
        /// </summary>
        public object Handle { get; private set; }
        /// <summary>
        /// 获取界面所属的界面组。
        /// </summary>
        public IUIGroup UIGroup { get; private set; }
        /// <summary>
        /// 获取界面深度。
        /// </summary>
        public int DepthInUIGroup { get; private set; }
        /// <summary>
        /// 获取是否暂停被覆盖的界面。
        /// </summary>
        public bool PauseCoveredUIForm { get; private set; }

        /// <summary>
        /// 初始化界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="uiGroup">界面所处的界面组。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <param name="isNewInstance">是否是新实例。</param>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnInit(int serialId, string uiFormAssetName, IUIGroup uiGroup, bool pauseCoveredUIForm, bool isNewInstance, object userData)
        {
            SerialId = serialId;
            UIFormAssetName = uiFormAssetName;
            if (isNewInstance) UIGroup = uiGroup;
            else if (UIGroup != uiGroup)
            {
                Log.Error("UI group is inconsistent for non-new-instance UI form.");
                return;
            }
            Handle = this.gameObject;
            DepthInUIGroup = 0; PauseCoveredUIForm = pauseCoveredUIForm;
        }
        /// <summary>
        /// 界面回收。
        /// </summary>
        public virtual void OnRecycle()
        {
            DepthInUIGroup = 0;
            PauseCoveredUIForm = true;
        }
        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnOpen(object userData)
        {
            m_Available = true;
            Visible = true;
        }
        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnClose(object userData)
        {
            gameObject.SetLayerRecursively(m_OriginalLayer);
            Visible = false;
            m_Available = false;
        }
        /// <summary>
        /// 界面暂停。
        /// </summary>
        public virtual void OnPause()
        {
            Visible = false;
        }
        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        public virtual void OnResume()
        {
            Visible = true;
        }
        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            DepthInUIGroup = depthInUIGroup;
        }
        /// <summary>
        /// 设置界面的可见性。
        /// </summary>
        /// <param name="visible">界面的可见性。</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        public virtual void OnCover() { }
        public virtual void OnReveal() { }
        public virtual void OnRefocus(object userData) { }
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) {  }
    }
}
