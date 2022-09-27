using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EPloy.Fsm;
using EPloy.Event;

public class ProcedureLogin : FsmState
{
    private bool isCanSwitchScene = false;
    public override void OnEnter()
    {
        base.OnEnter();
        isCanSwitchScene = false;
        GameModule.UI.OpenUIForm(UIName.LoginForm, UIGroupName.Default);
        GameModule.Event.Subscribe(EventId.SwitchSceneEvt, OnSwitchScene);
        GameModule.UI.CloseUIForm(UIName.LoadingForm);
    }

    public override void OnUpdate()
    {
        if (isCanSwitchScene)
        {
            ChangeState<ProcedureSwitchScene>();
            GameModule.UI.CloseUIForm(UIName.LoginForm);
        }
    }

    public override void OnLeave(bool isShutdown)
    {
        GameModule.Event.Unsubscribe(EventId.SwitchSceneEvt, OnSwitchScene);
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