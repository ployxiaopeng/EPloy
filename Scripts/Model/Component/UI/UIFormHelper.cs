//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.UI;
using System;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 默认界面辅助器。
    /// </summary>
    public class UIFormHelper : IUIFormHelper
    {
        /// <summary>
        /// 实例化界面。
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源。</param>
        /// <returns>实例化后的界面。</returns>
        public object InstantiateUIForm(object uiFormAsset)
        {
            GameObject UIForm = uiFormAsset as GameObject;
            string tpyeName = UIForm.name;
            UIForm = UnityEngine.Object.Instantiate(UIForm);
            UIFormLogic uIFormHotfix = UIForm.AddComponent<UIFormLogic>();
            uIFormHotfix.SetHotfixWndName(tpyeName);
            return UIForm;
        }

        /// <summary>
        /// 创建界面。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        public IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            GameObject gameObject = uiFormInstance as GameObject;
            if (gameObject == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }
            UIGroupHelper uIGroup = (UIGroupHelper)uiGroup.Helper;
            gameObject.transform.SetParent(uIGroup.Group.transform, false);
            gameObject.transform.localScale = Vector3.one;
            return gameObject.GetComponent<UIFormLogic>();
        }

        /// <summary>
        /// 释放界面。
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源。</param>
        /// <param name="uiFormInstance">要释放的界面实例。</param>
        public void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
        {
            Init.Resource.UnloadAsset(uiFormAsset);
            GameObject.Destroy((UnityEngine.Object)uiFormInstance);
        }
    }
}
