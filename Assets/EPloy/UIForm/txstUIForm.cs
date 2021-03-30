using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPloy;


public class txstUIForm : UIForm
{
    public override UIName UIName
    {
        get
        {
            return UIName.txstUIForm;
        }
    }

    public override void Open(object userData)
    {
        Log.Error("看看打不打得开");
    }

    public override void Close(object userData)
    {

    }
}
