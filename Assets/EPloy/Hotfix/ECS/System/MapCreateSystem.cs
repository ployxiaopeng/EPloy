using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapCreateSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }
        private MapCpt mapCpt;
        private MapEntityCpt mapEntityCpt;
        private MapCreateCpt mapCreateCpt;

        public void Start()
        {
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            mapCreateCpt = HotFixMudule.GameScene.AddCpt<MapCreateCpt>(mapCpt.map);
            mapCreateCpt.viewSizeX = 19;
            mapCreateCpt.viewSizeY = 11;
            mapCpt.gridPrefab = mapCpt.mapParent.Find("Mian/Grid").gameObject;
            mapCreateCpt.mapReqion = mapCpt.mapParent.Find("Mian/GridRoot");
            mapCpt.mapReqion = mapCreateCpt.mapReqion;
            mapCreateCpt.SetCreate(mapCpt.MapData.RoleBornPos);
            CreateGrid();
        }

        public void Update()
        {
            if (!mapCreateCpt.isUpdate) return;
            int minX = (int) mapCreateCpt.CurCreate.x - mapCreateCpt.viewSizeX / 2;
            int minY = (int) mapCreateCpt.CurCreate.y - mapCreateCpt.viewSizeY / 2;
            int maxX = minX + mapCreateCpt.viewSizeX;
            int maxY = minY + mapCreateCpt.viewSizeY;
            int index = 0;
            Entity[] grids = mapEntityCpt.grids.Values.ToArray();
            mapEntityCpt.grids.Clear();
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY = 0; y < maxY; y++)
                {
                    Vector2 vector2 = new Vector2(x, y);
                    Entity grid = grids[index];
                    grid.GetComponent<MapGirdCpt>().UpdateMapCell(mapCpt.GetMapCell(vector2));
                    mapEntityCpt.grids.Add(vector2, grids[index]);
                    index++;
                }
            }

            mapCreateCpt.CurCreate = mapCreateCpt.newCreate;
        }

        public void OnDestroy()
        {

        }

        private void CreateGrid()
        {
            int minX = (int) mapCreateCpt.CurCreate.x - mapCreateCpt.viewSizeX / 2;
            int minY = (int) mapCreateCpt.CurCreate.y - mapCreateCpt.viewSizeY / 2;
            int maxX = minX + mapCreateCpt.viewSizeX;
            int maxY = minY + mapCreateCpt.viewSizeY;
            mapCreateCpt.SetRegionName();
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    CreateGridEntity(mapEntityCpt, new Vector2(x, y));
                }
            }

            HotFixMudule.GameScene.CreateSystem<MapGirdSystem>();
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

