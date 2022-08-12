using EPloy.Hotfix.Obj;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPloy.Hotfix;
using System;

public class MapRoleData : ObjBase
{
    public static MapRoleData Create(int objId, Transform parent, Vector3 pos, Vector3 rotate)
    {
        MapRoleData uIEffectData = ReferencePool.Acquire<MapRoleData>();
        uIEffectData.InitData(objId, parent);
        uIEffectData.initPostion = pos;
        uIEffectData.initRotate = rotate;
        return uIEffectData;
    }


    public override void Clear()
    {
        base.Clear();
    }
}
