using System.Collections.Generic;
using UnityEngine;
using System.Net;
using EPloy.Fsm;

namespace EPloy
{
    public class ProcedureLogin : ProcedureBase
    {
        private bool isCanSwitchScene = false;
        public override void OnEnter()
        {
            base.OnEnter();
            isCanSwitchScene = false;
            HotFixMudule.UI.OpenUIForm(UIName.LoginForm, UIGroupName.Default);
            HotFixMudule.Event.Subscribe(EventId.SwitchSceneEvt, OnSwitchScene);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (isCanSwitchScene)
            {
                ChangeState<ProcedureSwitchScene>();
            }
        }

        public override void OnLeave(bool isShutdown)
        {
            HotFixMudule.Event.Unsubscribe(EventId.SwitchSceneEvt, OnSwitchScene);
            HotFixMudule.UI.CloseUIForm(UIName.LoginForm);
            base.OnLeave(isShutdown);
        }

        /// <summary>
        /// 场景切换事件
        /// </summary>
        /// <param name="arg"></param>
        protected virtual void OnSwitchScene(EventArg arg)
        {
            SwitchSceneEvt switchSceneEvt = (SwitchSceneEvt)arg;
            base.ProcedureOwner.SetData<VarString>("Secne", switchSceneEvt.SwitchSceneName);
            isCanSwitchScene = true;
        }


    }
}