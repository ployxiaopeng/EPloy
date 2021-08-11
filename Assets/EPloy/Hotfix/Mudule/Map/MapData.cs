using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    //进入地图数据管理
    public partial class MapMudule : IHotfixModule
    {
        #region 基本数据
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
        #endregion

        private DRMap mapData;
        public DRMap MapData
        {
            get { return mapData; }
            set
            {
                mapData = value;
                SetMapAllCell();
            }
        }

        //地图所有的格子数据
        private Dictionary<Vector2, DRMapCell> MapAllCell;
        /// <summary>
        /// 获取格子数据
        /// </summary>
        /// <param name="_cellkey"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取格子可通过与否
        /// </summary>
        /// <param name="_cellkey"></param>
        /// <returns></returns>
        public bool GetMapCellPass(Vector2 _cellkey)
        {
            bool pass = false;
            MapCellPass.TryGetValue(_cellkey, out pass);
            return pass;
        }
        /// <summary>
        /// 设置格子可通过与否
        /// </summary>
        /// <param name="_cellkey"></param>
        /// <param name="pass"></param>
        public void SetMapCellPass(Vector2 _cellkey, bool pass)
        {
            if (MapCellPass.ContainsKey(_cellkey))
            {
                MapCellPass[_cellkey] = pass;
            }
        }

    }
}