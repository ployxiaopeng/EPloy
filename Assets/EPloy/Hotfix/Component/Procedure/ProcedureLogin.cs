using LitJson;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using EPloy.Fsm;

namespace EPloy
{
    public class ProcedureLogin : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.UI.OpenUIForm(UIName.StartForm, UIGroupName.Default);
        }

        /// <summary>
        /// 流程转化
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeProcdure(string sceneName)
        {
            // GameEntry.UI.OpenUIWnd(UIWnd.LoadingWnd, 2);
            // Owner.SetData<VarString>("Secne", sceneName);
            // ChangeState<ProcedureChangeScene>(Owner);
            // GameEntry.UI.CloseUIWnd(UIWnd.LoginWnd);
        }
    }
}