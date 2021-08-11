using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    // 地图生成管理
    public partial class MapMudule : IHotfixModule
    {
        /// <summary>
        /// 视野格子比例
        /// </summary>
        private int _sizeX, _sizeY = 0;
        /// <summary>
        /// 区域父级
        /// </summary>
        private Transform _mapReqion;
        /// <summary>
        /// 视野生成格子数
        /// </summary>
        private Dictionary<int, Dictionary<int, MapGrid>> MapGridDict = new Dictionary<int, Dictionary<int, MapGrid>>();
        /// <summary>
        /// 格子预制体
        /// </summary>
        private GameObject _gridGo;

        /// <summary>
        ///  视野格子比例
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        public void SetViewSize(int sizeX, int sizeY)
        {
            _sizeX = sizeX; _sizeY = sizeY;
        }

        /// <summary>
        /// 根据视野生成格子的算法
        /// </summary>
        /// <param name="reqionId"></param>
        /// <param name="palyerpos"></param>
        /// <param name="dir"></param>
        public void CreateMap(int reqionId, Vector2 palyerpos, MoveDir dir = MoveDir.Stop)
        {
            //计算当前视野内左下角格子的坐标
            int originX = (int)palyerpos.x - _sizeX / 2;
            int originY = (int)palyerpos.y - _sizeY / 2;

            int MaxX, MaxY;
            switch (dir)
            {
                case MoveDir.Stop:
                    _mapReqion.name = string.Format("map{0}", reqionId);
                    MaxX = originX + _sizeX; MaxY = originY + _sizeY;
                    //生成
                    if (MapGridDict.Count == 0)
                    {
                        for (int x = originX; x < MaxX; x++)
                        {
                            for (int y = originY; y < MaxY; y++)
                            {
                                CreateGrid(x, y, reqionId);
                            }
                        }
                        break;
                    }
                    //全部更换
                    List<MapGrid> GridList = GetAllGridData();
                    MapGridDict.Clear();
                    int index = 0;
                    for (int x = originX; x < MaxX; x++)
                    {
                        for (int y = originY; y < MaxY; y++)
                        {
                            MapGrid grid = GridList[index];
                            SetGridPos(x, y, grid, reqionId);
                            index++;
                        }
                    }
                    break;
                //最下面到 最上面
                case MoveDir.Up:

                    MaxX = originX + _sizeX; MaxY = originY - 1;
                    for (int x = originX; x < MaxX; x++)
                    {
                        MapGrid grid = GetGridData(x, MaxY);
                        if (grid == null) Debug.LogError(x);
                        RemoveGridData(x, MaxY);
                        SetGridPos(x, MaxY + _sizeY, grid, reqionId);
                    }
                    break;
                //最上面到 最下面
                case MoveDir.Down:
                    MaxX = originX + _sizeX; MaxY = originY + _sizeY;
                    for (int x = originX; x < MaxX; x++)
                    {
                        MapGrid grid = GetGridData(x, MaxY);
                        if (grid == null) Debug.LogError(x);
                        RemoveGridData(x, MaxY);
                        SetGridPos(x, originY, grid, reqionId);
                    }
                    break;
                //最右面到 最左面
                case MoveDir.Left:
                    MaxX = originX + _sizeX; MaxY = originY + _sizeY;
                    for (int y = originY; y < MaxY; y++)
                    {
                        MapGrid grid = GetGridData(MaxX, y);
                        if (grid == null) Debug.LogError(y);
                        RemoveGridData(MaxX, y);
                        SetGridPos(originX, y, grid, reqionId);
                    }
                    break;
                //最左面到 最右面
                case MoveDir.Right:
                    MaxX = originX - 1; MaxY = originY + _sizeY;
                    for (int y = originY; y < MaxY; y++)
                    {
                        MapGrid grid = GetGridData(MaxX, y);
                        if (grid == null) Debug.LogError(y);
                        RemoveGridData(MaxX, y);
                        SetGridPos(MaxX + _sizeX, y, grid, reqionId);
                    }
                    break;
            }
        }
        //格子设置
        private void CreateGrid(int x, int y, int reqionId)
        {
            GameObject grid = UnityEngine.Object.Instantiate(_gridGo, _mapReqion);
            MapGrid mapGrid = new MapGrid(grid.transform);
            SetGridPos(x, y, mapGrid, reqionId);
        }
        private void SetGridPos(int x, int y, MapGrid grid, int reqionId)
        {
            grid.transform.name = string.Format("Grid_{0},{1}", x, y);
            Vector3 localPosition = new Vector3(x, y);
            grid.transform.localPosition = localPosition;
            grid.Init(GetMapCell(localPosition));
            SaveGridData(x, y, grid);
            //if (grid.mapCell.EventGroupId == 0 || grid.mapCell == null) return;
            ////如果当前格子事件未完成 根据进度设置对应数据
            //GameEntry.MapMgr.SetIntEventGrid(grid);
        }
        //格子数据设置
        private void SaveGridData(int x, int y, MapGrid value)
        {
            if (MapGridDict.ContainsKey(x))
            {
                var ItemDict = MapGridDict[x];
                if (ItemDict.ContainsKey(y))
                    ItemDict[y] = value;
                else
                    ItemDict.Add(y, value);
            }
            else
            {
                var ItemDict = new Dictionary<int, MapGrid>();
                ItemDict.Add(y, value);
                MapGridDict.Add(x, ItemDict);
            }
        }
        private MapGrid GetGridData(int x, int y, MapGrid defaultValue = null)
        {
            if (MapGridDict.ContainsKey(x))
            {
                var ItemDict = MapGridDict[x];
                if (ItemDict.ContainsKey(y))
                    return ItemDict[y];
            }
            return defaultValue;
        }
        private List<MapGrid> GetAllGridData()
        {
            List<MapGrid> GridList = new List<MapGrid>();
            foreach (var x in MapGridDict)
            {
                var ItemDic = x.Value;
                foreach (var y in ItemDic)
                    GridList.Add(y.Value);
            }
            return GridList;
        }
        private void RemoveGridData(int x, int y)
        {
            if (MapGridDict.ContainsKey(x))
            {
                var ItemDict = MapGridDict[x];
                if (ItemDict.ContainsKey(y))
                    ItemDict.Remove(y);
            }
        }
        //根据X,Y获取格子
        public MapGrid GetGridByPos(float x, float y, int reqionId)
        {
            MapGrid MapGrid = null;
            if (MapReqionId == reqionId) MapGrid = GetGridData((int)x, (int)y);
            if (MapGrid == null)
            {
                Log.Error(reqionId + ":" + x + "," + y);
                Log.Error("格子没找到 有问题或者不在视野内");
            }
            return MapGrid;
        }
        public MapGrid GetGridByPos(float x, float y)
        {
            MapGrid MapGrid = GetGridData((int)x, (int)y);
            if (MapGrid == null)
            {
                Log.Error("视野内未发现：" + x + "," + y);
            }
            return MapGrid;
        }

        public void EmptyMap()
        {
            foreach (var x in MapGridDict)
            {
                foreach (var y in x.Value)
                {
                    y.Value.Init();
                }
            }
        }
    }
}