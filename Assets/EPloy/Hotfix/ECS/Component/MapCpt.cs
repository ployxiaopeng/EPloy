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
            foreach (var mapCell in dataMapCell)
            {

                UnityEngine.Vector2 vector2 = new Vector2(1, 1);
                Log.Error("1111 " + vector2);
                Log.Error(mapCell.CellIndex.x);
                if (mapCell.RegionId == mapData.MapRegionId)
                {
                    Log.Error(mapCell.CellIndex);
                    //dRMapCells.Add(mapCell.CellIndex, mapCell);
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