using EPloy.Obj;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPloy.Table;

public class UIEffectData : ObjBase
{
    private DREntity DREntity;
    public override string AssetName => DREntity.AssetName;

    public static UIEffectData Create(int objId, Transform parent)
    {
        UIEffectData uIEffectData = ReferencePool.Acquire<UIEffectData>();
        uIEffectData.InitData(objId, parent);
        uIEffectData.DREntity = objId.GetDtObj();
        return uIEffectData;
    }


    public override void Clear()
    {
        base.Clear();
    }
}
