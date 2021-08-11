
using UnityEngine;

namespace EPloy
{
    /// <summary> 
    /// 地图管理器。
    /// </summary>
    public partial class MapComponet : Component
    {
        private Entity mapEntity;

        public override void Awake()
        {
            mapEntity = GameEntry.Game.CreateEntity("Map");
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

        }
    }
}
