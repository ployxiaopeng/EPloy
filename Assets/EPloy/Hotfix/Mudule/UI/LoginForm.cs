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
        private Text VersionText;
        #endregion

        private LoginStartLogic LoginStartLogic;

        //查找UI组件的代码
        public override void Create()
        {
            Login = transform.Find("Longin");
            VersionText = Login.Find("Version/Text").GetComponent<Text>();
            LoginStartLogic = CreateChildLogic<LoginStartLogic>(Login);
        }



        public override void Open(object userData)
        {
            HotFixMudule.UI.CloseUIForm(UIName.LoadingForm);
        }

        public override void Close(object userData)
        {
         
           
        }
    }
}