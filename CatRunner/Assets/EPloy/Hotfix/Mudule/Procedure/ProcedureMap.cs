using EPloy.Game;
using EPloy.Hotfix.Fsm;
using EPloy.Hotfix.Table;
using UnityEngine;

namespace EPloy.Hotfix
{
    public class ProcedureMap : FsmState
    {
        private string loadSecnce = null;
        private int mapId = 1001;
        public override void OnEnter()
        {
            base.OnEnter();
            loadSecnce = null;
            Log.Info("Map  OnEnter");
            HotFixMudule.GameScene.ECSActivate();
            HotFixMudule.GameScene.EnterMap(1001, 10001);
            HotFixMudule.UI.CloseUIForm(UIName.StartForm);
            HotFixMudule.DataStore.GetDataStore<GameFormData>().SetPlayer(10001); 
            HotFixMudule.UI.OpenUIForm(UIName.GameForm, UIGroupName.Default);



            //HotFixMudule.Event.Subscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
            // HotFixMudule.Event.Subscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
        }
        private void OnLoadDataTableSuccess(EventArg arg)
        {
            HotFixMudule.Event.Unsubscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
            HotFixMudule.Event.Unsubscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
        }
        private void OnLoadDataTableFailure(EventArg arg)
        {
            DataTableFailureEvt e = (DataTableFailureEvt)arg;
            Log.Info(UtilText.Format("不能加载 '{0}' 错误信息 '{2}'.", e.DataAssetName, e.ErrorMessage));
            HotFixMudule.Event.Unsubscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
            HotFixMudule.Event.Unsubscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
        }
    }
}