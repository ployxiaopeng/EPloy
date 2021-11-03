using EPloy.Fsm;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class ProcedureMap : ProcedureBase
    {
        private string loadSecnce = null;

        public override void OnEnter()
        {
            base.OnEnter();
            loadSecnce = null;
            Log.Info("Map  OnEnter");
            HotFixMudule.ECSActivate();
            HotFixMudule.GameScene.CreateSystem<MapSystem>();
            HotFixMudule.UI.CloseUIForm(UIName.LoadingForm);
        }
    }
}