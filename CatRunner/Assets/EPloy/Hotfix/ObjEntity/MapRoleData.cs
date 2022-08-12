using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPloy.Table;
using System;
using EPloy.Obj;

public class MapRoleData : ObjBase
{
    private DREntity DREntity;
    public override string AssetName { get => DREntity.AssetName; }

    public static MapRoleData Create(int objId, Transform parent, Vector3 pos, Vector3 rotate)
    {
        MapRoleData uIEffectData = ReferencePool.Acquire<MapRoleData>();
        uIEffectData.InitData(objId, parent);
        uIEffectData.initPostion = pos;
        uIEffectData.initRotate = rotate;
        uIEffectData.DREntity = objId.GetDtObj();
        return uIEffectData;
    }


    public override void Clear()
    {
        base.Clear();
    }
}
