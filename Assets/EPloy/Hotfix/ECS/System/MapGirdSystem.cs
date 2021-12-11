using System.Collections;
using System.Collections.Generic;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapGirdSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }

        private MapCpt mapCpt;

        public void Start()
        {
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
        }

        public void Update()
        {
            MapEntityCpt mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            if (mapEntityCpt.updateGrids.Count <= 0) return;
            for (int i = 0; i < mapEntityCpt.updateGrids.Count; i++)
            {
                MapGirdCpt mapGirdCpt = mapEntityCpt.updateGrids[i].GetComponent<MapGirdCpt>();
                if (mapGirdCpt.transform == null) CreateGird(mapGirdCpt);
                GirdUpdate(mapGirdCpt);
            }

            mapEntityCpt.updateGrids.Clear();
        }

        public void OnDestroy()
        {

        }

        private void CreateGird(MapGirdCpt mapGirdCpt)
        {
            Vector2 position = mapGirdCpt.mapCell.CellIndex;
            string gridName = string.Format("grid_{0},{1}", position.x, position.y);
            GameObject grid = Object.Instantiate(mapCpt.gridPrefab, mapCpt.mapReqion);
            grid.name = gridName;
            mapGirdCpt.Init(grid.transform);
        }

        private void GirdUpdate(MapGirdCpt mapGirdCpt)
        {
            Vector2 position = mapGirdCpt.mapCell.CellIndex;
            mapGirdCpt.transform.localPosition = position;
            Reset(mapGirdCpt);
            if (mapGirdCpt.mapCell == null)
            {
                Log.Info(Utility.Text.Format("没有找到 格子数据 坐标 {0}", mapGirdCpt.transform.localPosition));
                return;
            }

            SetMainSprite(mapGirdCpt);
            SetbgSprite(mapGirdCpt);
        }

        private void Reset(MapGirdCpt mapGirdCpt)
        {
            if (mapGirdCpt.bgSprite.sprite != null) mapGirdCpt.bgSprite.sprite = null;
            if (mapGirdCpt.frontSprite.sprite != null) mapGirdCpt.frontSprite.sprite = null;
        }

        //主图片
        private void SetMainSprite(MapGirdCpt mapGirdCpt)
        {
            if (mapGirdCpt.mapCell.ResMain == -1)
            {
                Log.Error(string.Format("没有找到 '{0}'格子数据", mapGirdCpt.mapCell.CellIndex));
                return;
            }

            mapGirdCpt.mainSprite.SetSprite(mapGirdCpt.mapCell.ResMain);
            mapGirdCpt.mainSprite.transform.localEulerAngles = mapGirdCpt.mapCell.ResMainRotate;
        }

        //背景图片
        private void SetbgSprite(MapGirdCpt mapGirdCpt)
        {
            if (mapGirdCpt.mapCell.ResBg == -1) return;
            mapGirdCpt.bgSprite.SetSprite(mapGirdCpt.mapCell.ResBg);
            mapGirdCpt.bgSprite.transform.localEulerAngles = mapGirdCpt.mapCell.ResBgRotate;
        }
    }
}

