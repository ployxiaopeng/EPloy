using System.Collections;
using System.Collections.Generic;
using EPloy.Obj;
using EPloy;
using UnityEngine;

[ObjAttribute("ObjTest")]
public class UIEffectObj : ObjBase
{

    public override void Create()
    {
        Log.Info("obj Create: " + transform.name);
    }

    public override void Activate(object userData)
    {
        Transform parent = userData as Transform;
        this.transform.SetParent(parent);
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.one;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameEntry.Obj.HideObj(this);
        }
    }

    public override void Hide(object userData)
    {
        Log.Info("obj Hide: " + transform.name);
    }
}
