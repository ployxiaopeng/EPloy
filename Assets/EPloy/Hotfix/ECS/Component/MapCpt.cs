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
        
        private DRMapCell[] dataMapCell;
        private DRMap mapData;

        public DRMap MapData
        {
            get { return mapData; }
            set
            {
                mapData = value;
                dataMapCell = HotFixMudule.DataTable.GetDataTable<DRMapCell>().GetAllDataRows();
                SetMapAllCell();
            }
        }

        private Dictionary<Vector2, DRMapCell> MapAllCell;

        public DRMapCell GetMapCell(Vector2 cellkey)
        {
            DRMapCell cell = null;
            MapAllCell.TryGetValue(cellkey, out cell);
            return cell;
        }

        private void SetMapAllCell()
        {
            if (MapAllCell == null) MapAllCell = new Dictionary<Vector2, DRMapCell>();
            else MapAllCell.Clear();
            if (MapCellPass == null) MapCellPass = new Dictionary<Vector2, bool>();
            else MapCellPass.Clear();

            foreach (var cell in dataMapCell)
            {
                MapAllCell.Add(cell.CellIndex, cell);
                MapCellPass.Add(cell.CellIndex, cell.Pass);
            }
        }

        private Dictionary<Vector2, bool> MapCellPass;

        public bool GetMapCellPass(Vector2 cellkey)
        {
            bool pass = false;
            MapCellPass.TryGetValue(cellkey, out pass);
            return pass;
        }

        public void SetMapCellPass(Vector2 cellkey, bool pass)
        {
            if (MapCellPass.ContainsKey(cellkey))
            {
                MapCellPass[cellkey] = pass;
            }
        }
    }
}

