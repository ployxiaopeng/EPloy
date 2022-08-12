using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class MapSystem : IReference
    {
        public void Start(EntityMap entityMap, MapCpt mapCpt)
        {
            mapCpt.roleParent = GameObject.Find("RoleParent").transform;
            mapCpt.mapData = GameModule.Table.GetDataTable<DRMap>().GetDataRow(mapCpt.mapId);
            SetMapVisual(entityMap, mapCpt);
            //主角
            ECSModule.roleSys.CreateRole(entityMap, mapCpt, RoleType.Player, mapCpt.PlayerId);

            //怪物
            ECSModule.roleSys.CreateRole(entityMap, mapCpt, RoleType.Monster, 10002);
        }

        private void SetMapVisual(EntityMap entityMap, MapCpt mapCpt)
        {
            entityMap.mapVisualBoxCpt = ECSModule.GameScene.GetCpt<MapVisualBoxCpt>(entityMap);
            //初始视野定中心在角色出生点
            entityMap.mapVisualBoxCpt.mianCamera = mapCpt.roleParent.Find("MainCamera").GetComponent<Camera>();
            entityMap.mapVisualBoxCpt.visualBox = new Vector2(10, 5);
            entityMap.mapVisualBoxCpt.SetcurVisualCentre(mapCpt.mapData.RoleBornPos);
        }

        public void Clear()
        {

        }
    }
}