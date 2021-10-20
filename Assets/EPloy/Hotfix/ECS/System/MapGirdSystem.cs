using System.Collections;
using System.Collections.Generic;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapGirdSystem : ISystem
    {
        public void Start()
        {

        }

        public void CreateGirdEntity(MapEntity mapEntity, Vector2 position, int reqionId)
        {
            string gridName = string.Format("grid_{0},{1}", (int) position.x, (int) position.y);
            MapComponent mapCpt = mapEntity.GetComponent<MapComponent>();
            GameObject grid = Object.Instantiate(mapCpt.gridGo, mapCpt.mapReqion);
            grid.name = gridName;
            MapGridEntity gridEntity = HotFixMudule.GameEntity.CreateEntity<MapGridEntity>();
            gridEntity.AddComponent<MapGirdComponent>();
            grid.transform.localPosition = position;
            SetMapGird(gridEntity, mapCpt.GetMapCell(position));

            if (mapCpt.mapGridEntitys.ContainsKey(position))
                mapCpt.mapGridEntitys[position] = gridEntity;
            else
                mapCpt.mapGridEntitys.Add(position, gridEntity);
        }

        private void SetMapGird(MapGridEntity entity,  DRMapCell mapCell)
        {
            MapGirdComponent mapGirdCpt = entity.GetComponent<MapGirdComponent>();
            Reset(mapGirdCpt);
            if (mapCell == null)
            {
                Log.Info(Utility.Text.Format("没有找到 格子数据 坐标 {0}", mapGirdCpt.transform.localPosition));
                return;
            }

            mapGirdCpt.mapCell = mapCell;
            SetMainSprite(mapGirdCpt);
            SetbgSprite(mapGirdCpt);
        }

        private void Reset(MapGirdComponent mapGirdCpt)
        {
            if (mapGirdCpt.bgSprite.sprite != null) mapGirdCpt.bgSprite.sprite = null;
            if (mapGirdCpt.frontSprite.sprite != null) mapGirdCpt.frontSprite.sprite = null;
        }

        //主图片
        private void SetMainSprite(MapGirdComponent mapGirdCpt)
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
        private void SetbgSprite(MapGirdComponent mapGirdCpt)
        {
            if (mapGirdCpt.mapCell.ResBg == -1) return;
            mapGirdCpt.bgSprite.SetSprite(mapGirdCpt.mapCell.ResBg);
            mapGirdCpt.bgSprite.transform.localEulerAngles = mapGirdCpt.mapCell.ResBgRotate;
        }

        
        public void Update()
        {

        }

        public void OnDestroy()
        {

        }
    }
}

