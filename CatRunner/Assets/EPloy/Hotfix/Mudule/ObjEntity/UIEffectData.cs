using EPloy.Hotfix.Obj;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPloy.Hotfix;

public class UIEffectData : ObjBase
{
    public static UIEffectData Create(int objId, Transform parent)
    {
        UIEffectData uIEffectData = ReferencePool.Acquire<UIEffectData>();
        uIEffectData.InitData(objId, parent);
        return uIEffectData;
    }


    public override void Clear()
    {
        base.Clear();
    }
}
