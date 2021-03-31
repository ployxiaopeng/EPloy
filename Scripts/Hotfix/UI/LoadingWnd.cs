using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    public class LoadingWnd : UIWndLogic
    {
        public override UIWnd UIWnd
        {
            get
            {
                return UIWnd.LoadingWnd;
            }
        }

        public override void OnOpen(object userdata)
        {
            Log.Info("loading...............");
        }

    }
}