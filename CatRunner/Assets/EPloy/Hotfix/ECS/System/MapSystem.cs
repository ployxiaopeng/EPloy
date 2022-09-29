using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class MapSystem : IReference
    {
        public void Start(MapCpt mapCpt)
        {
            mapCpt.roleParent = GameObject.Find("RoleParent").transform;
            mapCpt.mapData = GameModule.Table.GetDataTable<DRMap>().GetDataRow(mapCpt.mapId);
            SetMapVisual(mapCpt);
            //主角
            ECSModule.roleSys.CreatePlayer(mapCpt, mapCpt.PlayerId);

            //怪物
            ECSModule.roleSys.CreateMonster(mapCpt, 10002);


            //AstarPath.active.Scan();
        }

        private void SetMapVisual(MapCpt mapCpt)
        {
            MapVisualBoxCpt mapVisualBoxCpt = ECSModule.GameScene.GetSingleCpt<MapVisualBoxCpt>();
            //初始视野定中心在角色出生点
            mapVisualBoxCpt.mianCamera = mapCpt.roleParent.Find("MainCamera").GetComponent<Camera>();
            mapVisualBoxCpt.visualBox = new Vector2(10, 5);
            mapVisualBoxCpt.SetcurVisualCentre(mapCpt.mapData.RoleBornPos);
        }

        public void Clear()
        {

        }
    }
}