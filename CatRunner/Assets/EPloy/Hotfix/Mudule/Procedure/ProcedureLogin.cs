using LitJson;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using EPloy.Hotfix.Fsm;

namespace EPloy.Hotfix
{
    public class ProcedureLogin : FsmState
    {
        private bool isCanSwitchScene = false;
        public override void OnEnter()
        {
            base.OnEnter();
            isCanSwitchScene = false;
            HotFixMudule.UI.OpenUIForm(UIName.StartForm, UIGroupName.Default);
            HotFixMudule.Event.Subscribe(EventId.SwitchSceneEvt, OnSwitchScene);
        }

        public override void OnUpdate()
        {
            if (isCanSwitchScene)
            {
                ChangeState<ProcedureSwitchScene>();
            }
        }

        public override void OnLeave(bool isShutdown)
        {
            HotFixMudule.Event.Unsubscribe(EventId.SwitchSceneEvt, OnSwitchScene);
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