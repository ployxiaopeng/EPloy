using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EPloy
{
    public class LoginStartLogic : UIFormLogic
    {
        private GameObject btnStart;

        //查找UI组件的代码
        protected override void Create()
        {
            btnStart = transform.Find("btnStart").gameObject;
            UIEventListener.Get(btnStart).onClick = BtnStartClick;
        }

        public void BtnStartClick(GameObject go)
        {
            GameEntry.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
            SwitchSceneEvt switchSceneEvt = ReferencePool.Acquire<SwitchSceneEvt>();
            switchSceneEvt.SetData("Game");
            GameEntry.Event.Fire(switchSceneEvt);
        }
    }
}