using ETModel;
using UnityEngine;
using GameFramework;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace ETHotfix
{
    public abstract class UICommonLogic : IUIHotfixLogic
    {
        public int SiblingIndex
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }
        public Transform transform
        {
            get
            {
                return UIFormLogic.transform;
            }
        }
        public UIFormLogic UIFormLogic { get; set; }
        protected UICommonData UICommonData { get; private set; }

        public virtual void OnInit(UIFormLogic uiFormLogic)
        {
            if (StaticTextList == null) StaticTextList = new List<Text>();
            else StaticTextList.Clear();
            UIFormLogic = uiFormLogic;
            FindUIComponent();
            AddEevent();
            SetStaticText();
        }
        public virtual void FindUIComponent() { }
        public virtual void AddEevent() { }
        public virtual void OnOpen(object userData)
        {
            SetBaseData((UICommonData)userData);
            if (UICommonData.parentLevel != null)
                transform.SetParent(UICommonData.parentLevel);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }
        public virtual void OnUpdate() { }
        public void OnRefocus(object userData)
        {
            Log.Error("在通用UI 中 这个方法不生效");
        }
        public void Clear()
        {
            OnClose();
        }
        public virtual void OnClose()
        {
            UIGroupHelper uIGroupHelper = (UIGroupHelper)UIFormLogic.UIGroup.Helper;
            transform.SetParent(uIGroupHelper.Group.transform);
            HotfixReferencePool.Release(UICommonData);
        }
        public void CloseUI()
        {
            GameEntry.UI.CloseUIForm(UIFormLogic);
        }
        private void SetBaseData(UICommonData data)
        {
            UICommonData = data;
            if (UICommonData != null)
                UICommonData.CommonLogic = this; ;
        }

        #region 语言设置
        protected  List<Text> StaticTextList { get; set; }
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