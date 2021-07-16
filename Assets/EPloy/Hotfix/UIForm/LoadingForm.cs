using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EPloy
{
    [UIAttribute(UIName.LoadingForm)]
    public class LoadingForm : UIForm
    {
        public override void Close(object userData)
        {
            
        }

        public override void Create()
        {
          
        }

        public override void Open(object userData)
        {
            Log.Info("loading...............");
        }
    }
}