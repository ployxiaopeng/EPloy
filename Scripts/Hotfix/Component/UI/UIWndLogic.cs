//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using ETModel;

namespace ETHotfix
{
    public abstract class UIWndLogic : IUIHotfixLogic
    {
        public abstract UIWnd UIWnd { get; }
        public Transform transform
        {
            get
            {
                return UIFormLogic.transform;
            }
        }
        public  UIFormLogic UIFormLogic { get; set; }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="uiFormLogic"></param>
        public virtual void OnInit(UIFormLogic uiFormLogic)
        {
            UIFormLogic = uiFormLogic;
            if (StaticTextList == null) StaticTextList = new List<Text>();
            else StaticTextList.Clear();
            Find(); Event();
            SetStaticText();
        }
        /// <summary>
        /// 界面查找组件
        /// </summary>
        public virtual void Find() { }
        /// <summary>
        /// 界面事件添加
        /// </summary>
        public virtual void Event() { }
        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userdata"></param>
        public virtual void OnOpen(object userdata) { }
        public void Clear()
        {
            OnClose();
        }
        /// <summary>
        /// 界面关闭
        /// </summary>
        public virtual void OnClose() { }
        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnRefocus(object userData) { }
        /// <summary>
        /// 界面Update
        /// </summary>
        public virtual void OnUpdate() { }

        protected void SendShowUIEvent()
        {
            ShowUIEvent<UIWnd> ShowUIEvent = GameFramework.ReferencePool.Acquire<ShowUIEvent<UIWnd>>();
            ShowUIEvent.SetEventData(UIWnd);
            GameEntry.Event.Fire(ShowUIEvent.Id, ShowUIEvent);
        }
        protected void CloseUI()
        {
            GameEntry.UI.CloseUIForm(UIFormLogic);
        }
        protected void CloseUI(GameObject go)
        {
            CloseUI();
        }

        #region 语言设置
        private List<Text> StaticTextList;

        protected void SetStaticText(Transform textTrans)
        {
            StaticTextList.Add(textTrans.GetComponent<Text>());
        }
        protected void SetStaticText(Text text)
        {
            StaticTextList.Add(text);
        }
        private void SetStaticText()
        {

            if (StaticTextList == null)
                return;
            foreach (var txt in StaticTextList)
            {
                try
                {
                    // txt.SetStaticText(txt.text.Trim().ToInt());
                }
                catch (Exception e)
                {
                    Log.Error("设置静态语言错误:Id = {0}", txt.text);
                    txt.text = ("Error");
                }
            }
        }
        #endregion
    }
}