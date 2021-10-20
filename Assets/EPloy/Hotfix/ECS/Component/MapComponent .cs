using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public class MapComponent : Component
    {
        public Transform map;
        public int viewSizeX;
        public int viewSizeY;
        public Transform mapReqion;
        public int mapRegionId;
        public Dictionary<Vector2,MapGridEntity> mapGridEntitys = new Dictionary<Vector2,MapGridEntity>();
        public GameObject gridGo;
        
        private DRMapCell[] _dataMapCell = null;
        private DRMapCell[] DateMapCell
        {
            get
            {
                if (_dataMapCell == null)
                {
                    _dataMapCell = HotFixMudule.DataTable.GetDataTable<DRMapCell>().GetAllDataRows(); ;
                }
                return _dataMapCell;
            }
        }
        
        private DRMap mapData;
        public DRMap MapData
        {
            get { return mapData; }
            set
            {
                mapData = value;
                SetMapAllCell();
                mapRegionId = mapData.MapRegionId;
            }
        }
        
        private Dictionary<Vector2, DRMapCell> MapAllCell;
        
        public DRMapCell GetMapCell(Vector2 _cellkey)
        {
            DRMapCell cell = null;
            MapAllCell.TryGetValue(_cellkey, out cell);
            return cell;
        }
        private void SetMapAllCell()
        {
            if (MapAllCell == null) MapAllCell = new Dictionary<Vector2, DRMapCell>();
            else MapAllCell.Clear();
            if (MapCellPass == null) MapCellPass = new Dictionary<Vector2, bool>();
            else MapCellPass.Clear();

            foreach (var cell in DateMapCell)
            {
                if (cell.RegionId == mapData.MapRegionId)
                {
                    MapAllCell.Add(cell.CellIndex, cell);
                    MapCellPass.Add(cell.CellIndex, cell.Pass);
                }
            }
        }
        private Dictionary<Vector2, bool> MapCellPass;
        
        public bool GetMapCellPass(Vector2 _cellkey)
        {
            bool pass = false;
            MapCellPass.TryGetValue(_cellkey, out pass);
            return pass;
        }
        
        public void SetMapCellPass(Vector2 _cellkey, bool pass)
        {
            if (MapCellPass.ContainsKey(_cellkey))
            {
                MapCellPass[_cellkey] = pass;
            }
        }
    }
}

