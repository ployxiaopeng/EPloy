using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public class MapCpt : Component
    {
        public Entity map;

        public Transform mapParent;
        public GameObject gridPrefab;
        public Transform mapReqion;

        public DRMap mapData { get; private set; }
        private Dictionary<Vector2, DRMapCell> dRMapCells = new Dictionary<Vector2, DRMapCell>();

        public void SetMapData(DRMap mapData)
        {
            this.mapData = mapData;
            DRMapCell[] dataMapCell = HotFixMudule.DataTable.GetDataTable<DRMapCell>().GetAllDataRows();
            for (int i = 0; i < dataMapCell.Length; i++)
            {
                DRMapCell dRMapCell = dataMapCell[i];
                if (dRMapCell.RegionId == mapData.MapRegionId)
                {
                    dRMapCells.Add(dRMapCell.CellIndex, dRMapCell);
                }
            }
        }

        public DRMapCell GetMapCell(Vector2 cellkey)
        {
            DRMapCell cell = null;
            dRMapCells.TryGetValue(cellkey, out cell);
            return cell;
        }
    }
}