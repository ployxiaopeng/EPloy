using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.Procedure;
using GameFramework.WebRequest;
using GameFramework;
using ETModel;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Network;
using System.Net;

namespace ETHotfix
{
    public class ProcedureLogin : ProcedureBase
    {
        private ProcedureOwner Owner;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Owner = procedureOwner;
            GameEntry.UI.OpenUIWnd(UIWnd.LoginWnd);

            GameEntry.Extension.GetComponent<AtlasComponent>().LoadAtlas("Map10101");
            GameEntry.Extension.GetComponent<AtlasComponent>().LoadAtlas("Map10100");
        }

        /// <summary>
        /// 流程转化
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeProcdure(string sceneName)
        {
            GameEntry.UI.OpenUIWnd(UIWnd.LoadingWnd, 2);
            Owner.SetData<VarString>("Secne", sceneName);
            ChangeState<ProcedureChangeScene>(Owner);
            GameEntry.UI.CloseUIWnd(UIWnd.LoginWnd);
        }

    }
}