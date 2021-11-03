using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }

        private MapCpt mapCpt;
        private IDataTable<DRMap> dataMap;
        private bool enterMap;

        public void Start()
        {
            enterMap = false;
            dataMap = HotFixMudule.DataTable.GetDataTable<DRMap>();
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            mapCpt.map = HotFixMudule.GameScene.CreateEntity("Map");
            mapCpt.MapData = dataMap.GetDataRow(10101);
            HotFixMudule.GameScene.AddCpt<MapEntityCpt>(mapCpt.map);
        }

        public void Update()
        {
            if (!enterMap)
            {
                OnEnterMap();
                CreateRole();
                CreateGrid();
                enterMap = true;
            }

            UpdateMap();
        }

        public void OnDestroy()
        {

        }

        private void OnEnterMap()
        {
            mapCpt.mapParent = GameObject.Find("Map").transform;
            mapCpt.viewSizeX = 19;
            mapCpt.viewSizeY = 11;
            mapCpt.gridGo = mapCpt.mapParent.Find("Mian/Grid").gameObject;
            mapCpt.mapReqion = mapCpt.mapParent.Find("Mian/GridRoot");
        }

        private void CreateRole()
        {
            MapEntityCpt mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            mapEntityCpt.role = HotFixMudule.GameScene.CreateEntity("Role");
            MapRoleCpt mapRoleCpt = HotFixMudule.GameScene.AddCpt<MapRoleCpt>(mapEntityCpt.role);
            MapCameraCpt mapCameraCpt = HotFixMudule.GameScene.AddCpt<MapCameraCpt>(mapEntityCpt.role);
            mapRoleCpt.reqionId = mapCpt.mapRegionId;
            mapRoleCpt.roleDir = MoveDir.Stop;
            mapRoleCpt.rolePos = mapCpt.MapData.RoleBornPos;
            mapRoleCpt.UpdateMap = true;

            mapCameraCpt.camera = mapCpt.mapParent.Find("Camera").GetComponent<Camera>();
            mapCameraCpt.followPos = mapRoleCpt.rolePos;
            HotFixMudule.GameScene.CreateSystem<MapCameraSystem>();
        }

        private void CreateGrid()
        {
            MapEntityCpt mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            MapRoleCpt mapRoleCpt = mapEntityCpt.role.GetComponent<MapRoleCpt>();
            int MaxX, MaxY, originX, originY;
            originX = (int) mapRoleCpt.rolePos.x - mapCpt.viewSizeX / 2;
            originY = (int) mapRoleCpt.rolePos.y - mapCpt.viewSizeY / 2;
            MaxX = originX + mapCpt.viewSizeX;
            MaxY = originY + mapCpt.viewSizeY;
            mapCpt.mapReqion.name = string.Format("map{0}", mapRoleCpt.reqionId);
            for (int x = originX; x < MaxX; x++)
            {
                for (int y = originY; y < MaxY; y++)
                {
                    CreateGridEntity(mapEntityCpt, new Vector2(x, y));
                }
            }

            HotFixMudule.GameScene.CreateSystem<MapGirdSystem>();
        }

        private void UpdateMap()
        {
            MapEntityCpt mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            MapRoleCpt mapRoleCpt = mapEntityCpt.role.GetComponent<MapRoleCpt>();
            if (!mapRoleCpt.UpdateMap) return;

            //计算当前视野内左下角格子的坐标
            int originX = (int) mapRoleCpt.rolePos.x - mapCpt.viewSizeX / 2;
            int originY = (int) mapRoleCpt.rolePos.y - mapCpt.viewSizeY / 2;

            int MaxX, MaxY;
            switch (mapRoleCpt.roleDir)
            {
                case MoveDir.Stop:
                    mapCpt.mapReqion.name = string.Format("map{0}", mapRoleCpt.reqionId);
                    MaxX = originX + mapCpt.viewSizeX;
                    MaxY = originY + mapCpt.viewSizeY;
                    Entity[] grids = mapEntityCpt.grids.Values.ToArray();
                    mapEntityCpt.grids.Clear();
                    int index = 0;
                    for (int x = originX; x < MaxX; x++)
                    {
                        for (int y = originY; y < MaxY; y++)
                        {
                            Vector2 vector2 = new Vector2(x, y);
                            Entity grid = grids[index];
                            grid.GetComponent<MapGirdCpt>().UpdateMapCell(mapCpt.GetMapCell(vector2));
                            mapEntityCpt.grids.Add(vector2, grids[index]);
                            index++;
                        }
                    }

                    break;
                //最下面到 最上面
                case MoveDir.Up:
                    MaxX = originX + mapCpt.viewSizeX;
                    MaxY = originY - 1;
                    for (int x = originX; x < MaxX; x++)
                    {
                        UpdateGridEntity(mapEntityCpt, new Vector2(x, MaxY), new Vector2(x, MaxY + mapCpt.viewSizeY));
                    }

                    break;
                //最上面到 最下面
                case MoveDir.Down:
                    MaxX = originX + mapCpt.viewSizeX;
                    MaxY = originY + mapCpt.viewSizeY;
                    for (int x = originX; x < MaxX; x++)
                    {
                        UpdateGridEntity(mapEntityCpt, new Vector2(x, MaxY), new Vector2(x, originY));
                    }

                    break;
                //最右面到 最左面
                case MoveDir.Left:
                    MaxX = originX + mapCpt.viewSizeX;
                    MaxY = originY + mapCpt.viewSizeY;
                    for (int y = originY; y < MaxY; y++)
                    {
                        UpdateGridEntity(mapEntityCpt, new Vector2(MaxX, y), new Vector2(originX, y));
                    }

                    break;
                //最左面到 最右面
                case MoveDir.Right:
                    MaxX = originX - 1;
                    MaxY = originY + mapCpt.viewSizeY;
                    for (int y = originY; y < MaxY; y++)
                    {
                        UpdateGridEntity(mapEntityCpt, new Vector2(MaxX, y), new Vector2(MaxX + mapCpt.viewSizeX, y));
                    }

                    break;
            }

            mapRoleCpt.UpdateMap = false;
        }

        private void CreateGridEntity(MapEntityCpt mapEntityCpt, Vector2 vector2)
        {
            Entity entity = HotFixMudule.GameScene.CreateEntity("grid_" + vector2);
            MapGirdCpt mapGirdCpt = HotFixMudule.GameScene.AddCpt<MapGirdCpt>(entity);
            mapGirdCpt.UpdateMapCell(mapCpt.GetMapCell(vector2));
            mapEntityCpt.grids.Add(vector2, entity);
        }

        private void UpdateGridEntity(MapEntityCpt mapEntityCpt, Vector2 old, Vector2 cur)
        {
            Entity grid = mapEntityCpt.grids[old];
            if (grid == null) Log.Error(old);
            mapEntityCpt.grids.Remove(old);
            mapEntityCpt.grids.Add(cur, grid);
            grid.GetComponent<MapGirdCpt>().UpdateMapCell(mapCpt.GetMapCell(cur));
        }
    }
}

