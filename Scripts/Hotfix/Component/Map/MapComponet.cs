//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary> 
    /// 地图管理器。
    /// </summary>
    [HotfixExtension]
    public partial class MapComponet : Component
    {
        public override void Awake()
        {

        }

        private int MapReqionId;
        public void OnEnterMap(Transform map)
        {
            SetViewSize(19, 11);
            Camera = map.Find("Camera");
            MapReqionId = mapData.MapRegionId;
            _gridGo = map.Find("Mian/Grid").gameObject;
            _mapReqion = map.Find("Mian/GridRoot");
            CreateMap(MapReqionId, MapData.RoleBornPos);
            CreateRole(map.Find("Mian/ResRoot"));
        }

        //主角实体数据
        public RoleData roleData
        {
            get;
            private set;
        }
        private void CreateRole(Transform transform)
        {
            roleData = HotfixReferencePool.Acquire<RoleData>();
            roleData.SetDREntityData(3001.GetEntity(), transform);
            roleData.SetBaseData(MapData.RoleBornPos, MapData.RolelRotate);
            GameEntry.Entity.ShowHero(roleData);
            InitRole(MapData.RoleBornPos);
        }
    }
}
