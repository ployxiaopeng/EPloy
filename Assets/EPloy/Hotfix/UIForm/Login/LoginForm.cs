using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EPloy
{
    [UIAttribute(UIName.LoginForm)]
    public class LoginForm : UIForm
    {
        #region UI组件
        public Transform Login { get; private set; }
        private GameObject btnStart;
        private Text VersionText;
        #endregion

       
        //查找UI组件的代码
        public override void Create()
        {
            Login = transform.Find("Longin");
            btnStart = Login.Find("btnStart").gameObject;
            VersionText = Login.Find("Version/Text").GetComponent<Text>();

            UIEventListener.Get(btnStart).onClick = BtnStartClick;
        }

        public void BtnStartClick(GameObject go)
        {
            GameEntry.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
            SwitchSceneEvt switchSceneEvt = ReferencePool.Acquire<SwitchSceneEvt>();
            switchSceneEvt.SetData("Game");
            GameEntry.Event.Fire(switchSceneEvt);
        }


        public override void Open(object userData)
        {
            btnStart.SetActive(true);
            GameEntry.UI.CloseUIForm(UIName.LoadingForm);
        }

        public override void Close(object userData)
        {
         
           
        }
    }
}