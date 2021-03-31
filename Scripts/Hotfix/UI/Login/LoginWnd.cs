using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    public class LoginWnd : UIWndLogic
    {
        public override UIWnd UIWnd
        {
            get
            {
                return UIWnd.LoginWnd;
            }
        }

        #region UI组件
        public Transform Login { get; private set; }
        private GameObject StartGameBtn;
        private Text VersionText;
        #endregion

        private ProcedureLogin ProcedureLogin
        {
            get
            {
                return (ProcedureLogin)GameEntry.Procedure.CurrentProcedure;
            }
        }
        //查找UI组件的代码
        public override void Find()
        {
            Login = transform.Find("Longin");
            StartGameBtn = Login.Find("StartGameBtn").gameObject;
            VersionText = Login.Find("Version/Text").GetComponent<Text>();
            #region 静态语言设置
            SetStaticText(transform.Find("Longin/TxtCopyright"));
            SetStaticText(transform.Find("Longin/TxtNotes"));
            #endregion
        }
        public override void Event()
        {
            UIEventListener.Get(StartGameBtn).onClick = StartGameBtnClick;
        }
        public override void OnOpen(object userdata)
        {
            StartGameBtn.SetActive(true);
        }
        public void StartGameBtnClick(GameObject go)
        {
            ProcedureLogin.ChangeProcdure("GameMap");
        }
    }
}