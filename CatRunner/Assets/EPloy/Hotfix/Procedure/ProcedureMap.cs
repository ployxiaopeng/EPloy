using EPloy.Event;
using EPloy.Fsm;
using EPloy.Table;
using UnityEngine;

public class ProcedureMap : FsmState
{
    private string loadSecnce = null;
    private int mapId = 1001;
    public override void OnEnter()
    {
        base.OnEnter();
        loadSecnce = null;
        Log.Info("Map  OnEnter");
        ECSModule.GameScene.EnterMap(1001, 10001);
        GameModule.UI.CloseUIForm(UIName.StartForm);
        GameModule.DataStore.GetDataStore<GameFormData>().SetPlayer(10001);
        GameModule.UI.OpenUIForm(UIName.GameForm, UIGroupName.Default);



        //HotFixMudule.Event.Subscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
        // HotFixMudule.Event.Subscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
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
}