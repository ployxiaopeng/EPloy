using ETModel;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{
    /// <summary>
    /// 热更新层UGUI界面
    /// </summary>
    public interface IUIHotfixLogic : IReference
    {
        Transform transform { get; }
        UIFormLogic UIFormLogic { get; set; }
        void OnInit(UIFormLogic uiFormLogic);
        void OnOpen(object userdata);
        void OnRefocus(object userData);
        void OnUpdate();
    }
}

