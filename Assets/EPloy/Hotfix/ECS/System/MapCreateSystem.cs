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
            mapCreateCpt.viewSizeX = 22;
            mapCreateCpt.viewSizeY = 12;
            mapCreateCpt.mapReqion = mapCpt.mapParent.Find("Mian/GridRoot");
            mapCpt.mapReqion = mapCreateCpt.mapReqion;
            mapCreateCpt.SetCreate(mapCpt.mapData.RoleBornPos);
            CreateGrid();
            HotFixMudule.GameScene.CreateSystem<MapGirdSystem>();
        }

        public void Update()
        {
            if (!mapCreateCpt.isUpdate) return;
            Vector2[] keys = mapEntityCpt.grids.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                Entity entity = mapEntityCpt.grids[keys[i]];
                if (CheckGridInView(entity.GetComponent<MapGirdCpt>())) continue;
                mapEntityCpt.updateGrids.Add(entity);
                mapEntityCpt.grids.Remove(keys[i]);
            }

            int index = 0;
            for (int x = mapCreateCpt.minX; x < mapCreateCpt.maxX; x++)
            {
                for (int y = mapCreateCpt.minY; y < mapCreateCpt.maxY; y++)
                {
                    Vector2 vector2 = new Vector2(x, y);
                    if (mapEntityCpt.grids.ContainsKey(vector2)) continue;
                    Entity entity = mapEntityCpt.updateGrids[index];
                    MapGirdCpt mapGirdCpt = entity.GetComponent<MapGirdCpt>();
                    mapGirdCpt.UpdateMapCell(mapCpt.GetMapCell(vector2));
                    mapEntityCpt.grids.Add(vector2, entity);
                    index++;
                }
            }

            mapCreateCpt.CurCreate = mapCreateCpt.newCreate;
        }

        public void OnDestroy()
        {

        }


        private bool CheckGridInView(MapGirdCpt mapGirdCpt)
        {
            Vector2 pos = mapGirdCpt.mapCell.CellIndex;
            return pos.x >= mapCreateCpt.minX &&
                   pos.x <= mapCreateCpt.maxX &&
                   pos.y >= mapCreateCpt.minY &&
                   pos.y <= mapCreateCpt.maxY;
        }

        private void CreateGrid()
        {
            mapCreateCpt.SetRegionName();
            for (int x = mapCreateCpt.minX; x < mapCreateCpt.maxX; x++)
            {
                for (int y = mapCreateCpt.minY; y < mapCreateCpt.maxY; y++)
                {
                    Vector2 vector2 = new Vector2(x, y);
                    Entity entity = HotFixMudule.GameScene.CreateEntity("grid_" + vector2);
                    MapGirdCpt mapGirdCpt = HotFixMudule.GameScene.AddCpt<MapGirdCpt>(entity);
                    mapGirdCpt.UpdateMapCell(mapCpt.GetMapCell(vector2));
                    mapEntityCpt.grids.Add(vector2, entity);
                    mapEntityCpt.updateGrids.Add(entity);
                }
            }
        }
    }
}

