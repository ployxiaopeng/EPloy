using EPloy.Event;
using EPloy.Fsm;
using EPloy.Table;
using UnityEngine;

public class ProcedureMap : FsmState
{
    private string loadSecnce = null;
    private bool isCanSwitchScene = false;
    private int mapId = 1001;
    public override void OnEnter()
    {
        base.OnEnter();
        isCanSwitchScene = false;
        GameModule.Event.Subscribe(EventId.SwitchSceneEvt, OnSwitchScene);
        loadSecnce = null;
        Log.Info("Map  OnEnter");
        ECSModule.GameScene.EnterMap(1001, 10001);
        GameModule.DataStore.GetDataStore<GameFormData>().SetPlayer(10001);
        GameModule.UI.OpenUIForm(UIName.GameForm, UIGroupName.Default);
        GameModule.UI.CloseUIForm(UIName.LoadingForm);

        //HotFixMudule.Event.Subscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
        // HotFixMudule.Event.Subscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
    }

    public override void OnUpdate()
    {
        if (isCanSwitchScene)
        {
            ECSModule.GameScene.ExitMap();
            ChangeState<ProcedureSwitchScene>();
            GameModule.UI.CloseUIForm(UIName.GameForm);

        }
    }

    public override void OnLeave(bool isShutdown)
    {
        GameModule.Event.Unsubscribe(EventId.SwitchSceneEvt, OnSwitchScene);
        base.OnLeave(isShutdown);
    }

    private void OnLoadDataTableSuccess(EventArg arg)
    {
        GameModule.Event.Unsubscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
        GameModule.Event.Unsubscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
    }
    private void OnLoadDataTableFailure(EventArg arg)
    {
        DataTableFailureEvt e = (DataTableFailureEvt)arg;
        Log.Info(UtilText.Format("不能加载 '{0}' 错误信息 '{2}'.", e.DataAssetName, e.ErrorMessage));
        GameModule.Event.Unsubscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
        GameModule.Event.Unsubscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
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