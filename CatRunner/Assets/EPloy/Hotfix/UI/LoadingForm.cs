using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy.Hotfix;
using EPloy.UI;

[UIAttribute(UIName.LoadingForm)]
public class LoadingForm: UIForm
{

    public override void Create()
    {

        Log.Info("Loading......");
    }

    public override void Open(object userData)
    {
       
    }

}
