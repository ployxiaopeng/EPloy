//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;

namespace ETHotfix
{
    public class RoleData : HotfixEntityData
    {
        public  void SetBaseData(Vector3 _position, Vector3 _rotation)
        {
            Position = _position; Rotation = _rotation;
        }
        public override void SetDREntityData(DREntity _dREntity, Transform parentLevel)
        {
            base.DREntity = _dREntity;
            base.ParentLevel = parentLevel;
        }
    }
}