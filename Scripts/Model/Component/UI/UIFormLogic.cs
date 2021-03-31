//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.UI;

namespace ETModel
{
    /// <summary>
    /// 界面逻辑基类。
    /// </summary>
    public class UIFormLogic : UIForm
    {
        /// <summary>
        /// 对应的热更新层UGUI界面名字
        /// </summary>
        public string HotfixWndName { get; private set; }

        /// <summary>
        ///  设置对应的热更新层UGUI界面类名
        /// </summary>
        /// <param name="wndName"></param>
        public void SetHotfixWndName(string wndName)
        {
            HotfixWndName = wndName;
        }

        public override void OnInit(int serialId, string uiFormAssetName, IUIGroup uiGroup, bool pauseCoveredUIForm, bool isNewInstance, object userData)
        {
            base.OnInit(serialId, uiFormAssetName, uiGroup, pauseCoveredUIForm, isNewInstance, userData);
            HotfixUIEvent uIEvent = ReferencePool.Acquire<HotfixUIEvent>();
            uIEvent.SetEventData(UIFunType.OnInit, this);
            Init.Event.FireNow(HotfixUIEvent.EventId, uIEvent);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            HotfixUIEvent uIEvent = ReferencePool.Acquire<HotfixUIEvent>();
            uIEvent.SetEventData(UIFunType.OnOpen, this);
            Init.Event.Fire(HotfixUIEvent.EventId, uIEvent);
        }

        public override void OnClose(object userData)
        {
            base.OnClose(userData);
            HotfixUIEvent uIEvent = ReferencePool.Acquire<HotfixUIEvent>();
            uIEvent.SetEventData(UIFunType.OnClose, this);
            Init.Event.Fire(HotfixUIEvent.EventId, uIEvent);
        }
    }
}

