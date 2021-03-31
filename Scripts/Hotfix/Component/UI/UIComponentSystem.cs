
using ETModel;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Resource;
using GameFramework.UI;
using System;
using UnityEngine;

namespace ETHotfix
{
    public static class UIComponentSystem
    {
        private static UIGroup[] uIGroups = new UIGroup[]
        {
            new UIGroup(){Name ="Lv1",Depth=1 },
            new UIGroup(){Name ="Lv2",Depth=2 },
            new UIGroup(){Name ="Lv3",Depth=3 },
        };
        public static async ETVoid HotfixAwake(this UIComponent self)
        {
            foreach (var uIGroup in uIGroups)
            {
                if (!self.AddUIGroup(uIGroup.Name, uIGroup.Depth))
                    Log.Warning("Add UI group '{0}' failure.", uIGroup.Name);
            }
        }
        private static UIHotfixComponent UI = null;

        public static void PreloadUIWnd(this UIComponent self, UIWnd uiWnd, LoadAssetCallbacks loadUIWndSuccess)
        {
            if (UI == null) UI = GameEntry.Extension.GetComponent<UIHotfixComponent>();
            string assetName = AssetUtility.GetUIWndAsset(uiWnd.ToString());
            GameEntry.Resource.LoadAsset(assetName, typeof(GameObject), loadUIWndSuccess, uiWnd.ToString());
        }

        public static bool HasUIWnd(this UIComponent self, UIWnd uiWnd, int depth = -1)
        {
            string assetName = AssetUtility.GetUIWndAsset(uiWnd.ToString());
            if (depth == -1)
                return self.HasUIForm(assetName);

            IUIGroup uiGroup = self.GetUIGroup(uIGroups[depth].Name);
            if (uiGroup == null) return false;
            return uiGroup.HasUIForm(assetName);
        }
        public static UIForm GetUIForm(this UIComponent self, UIWnd uiWnd, int depth = -1)
        {
            string assetName = AssetUtility.GetUIWndAsset(uiWnd.ToString());
            UIForm uiForm = null;
            if (depth == -1)
            {
                uiForm = self.GetUIForm(assetName);
                return uiForm;
            }

            IUIGroup uiGroup = self.GetUIGroup(uIGroups[depth].Name);
            if (uiGroup == null) return null;
            uiForm = (UIForm)uiGroup.GetUIForm(assetName);
            return uiForm;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="self"></param>
        /// <param name="uiWnd"></param>
        /// <param name="">uIGroup Index</param>
        /// <param name="userData"></param>
        public static void OpenUIWnd(this UIComponent self, UIWnd uiWnd, int depth = 0, object userData = null)
        {
            string assetName = AssetUtility.GetUIWndAsset(uiWnd.ToString());
            int uiSerialId = self.OpenUIForm(assetName, uIGroups[depth].Name, Constant.AssetPriority.UIWndAsset);
            UI.OnOpen(uiSerialId, userData);
        }
        public static void CloseUIWnd(this UIComponent self, UIWnd uiWnd)
        {
            UIForm UGuiForm = GetUIForm(self, uiWnd);
            if (UGuiForm == null) return;
            self.CloseUIForm(UGuiForm);
        }
        public static void RefocusUIWnd(this UIComponent self, UIWnd uiWnd, object userData = null)
        {
            UIForm uIForm = self.GetUIForm(uiWnd);
            if (uIForm != null)
                UI.OnRefocus(uIForm.SerialId, userData);
        }

        public static void OpenUICommon(this UIComponent self, UICommonData userData)
        {
            string AssetName = AssetUtility.GetUICommonAsset(userData.UICommon.ToString());
            int uiSerialId = self.OpenUIForm(AssetName, self.DefaultUIGroup.Name, Constant.AssetPriority.UIWndAsset);
            UI.OnOpen(uiSerialId, userData);
        }
        public static void CloseUICommon(this UIComponent self, UICommon uICommon)
        {
            string assetName = AssetUtility.GetUICommonAsset(uICommon.ToString());
            UIForm[] uiForm = self.GetUIForms(assetName);
            for (int i = 0; i < uiForm.Length; i++)
                self.CloseUIForm(uiForm[i]);
        }
    }
}