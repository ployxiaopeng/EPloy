using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy.Hotfix;
using EPloy.Game;

[UIAttribute(UIName.LoadingForm)]
public class LoadingForm: UIForm
{
    protected override bool isUpdate => false;

    public override void Create()
    {

        Log.Info("Loading......");
    }

    public override void Open(object userData)
    {
       
    }

}
