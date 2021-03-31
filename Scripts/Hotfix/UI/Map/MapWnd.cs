using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    public class MapWnd : UIWndLogic
    {
        public override UIWnd UIWnd
        {
            get
            {
                return UIWnd.MapWnd;
            }
        }

        private RoleMoveHelper roleMove;
        public override void Find()
        {
            roleMove = new RoleMoveHelper(transform.Find("Move"));
        }

        public override void OnOpen(object userdata)
        {
            Log.Info("MapWnd...............");
        }

        public override void OnUpdate()
        {
            roleMove.FixedUpdate();
        }
    }
}