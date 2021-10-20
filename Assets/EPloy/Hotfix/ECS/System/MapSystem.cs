using System.Collections.Generic;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapSystem : ISystem
    {
        private MapEntity mapEntity;
        private MapComponent MapCpt;
        private  IDataTable<DRMap> _dataMap = null;
        private IDataTable<DRMap> DataMap
        {
            get
            {
                if (_dataMap == null)
                {
                    _dataMap = HotFixMudule.DataTable.GetDataTable<DRMap>();
                }

                return _dataMap;
            }
        }
        
        public void Start()
        {
            mapEntity = HotFixMudule.GameEntity.CreateEntity<MapEntity>("Map");
        }

        public void OnEnterMap(Transform map, int mapId)
        {
            MapCpt = mapEntity.AddComponent<MapComponent>();
            MapCpt.map = map;
            MapCpt.viewSizeX = 19;
            MapCpt.viewSizeY = 11;
            MapCpt.MapData = DataMap.GetDataRow(mapId);
            MapCpt.gridGo = map.Find("Mian/Grid").gameObject;
            MapCpt.mapReqion = map.Find("Mian/GridRoot");
            CreateMap(MapCpt.mapRegionId, MapCpt.MapData.RoleBornPos);
        }

        public void CreateMap(int reqionId, Vector2 palyerpos, MoveDir dir = MoveDir.Stop)
        {
            //计算当前视野内左下角格子的坐标
            int originX = (int)palyerpos.x - MapCpt.viewSizeX / 2;
            int originY = (int)palyerpos.y - MapCpt.viewSizeY / 2;

            int MaxX, MaxY;
            switch (dir)
            {
                case MoveDir.Stop:
                    MapCpt.mapReqion.name = string.Format("map{0}", reqionId);
                    MaxX = originX + MapCpt.viewSizeX; MaxY = originY + MapCpt.viewSizeY;
                    //生成
                    if (MapCpt.mapGridEntitys.Count == 0)
                    {
                        for (int x = originX; x < MaxX; x++)
                        {
                            for (int y = originY; y < MaxY; y++)
                            {
                                //CreateEntity(x, y, reqionId);
                            }
                        }

                        break;
                    }
                    //全部更换
                    // List<MapGrid> GridList = GetAllGridData();
                    MapCpt.mapGridEntitys.Clear();
                    int index = 0;
                    for (int x = originX; x < MaxX; x++)
                    {
                        for (int y = originY; y < MaxY; y++)
                        {
                            // MapGrid grid = GridList[index];
                            // SetGridPos(x, y, grid, reqionId);
                            index++;
                        }
                    }
                    break;
                //最下面到 最上面
                case MoveDir.Up:

                    MaxX = originX + MapCpt.viewSizeX; MaxY = originY - 1;
                    for (int x = originX; x < MaxX; x++)
                    {
                        // MapGrid grid = GetGridData(x, MaxY);
                        // if (grid == null) Debug.LogError(x);
                        // RemoveGridData(x, MaxY);
                        // SetGridPos(x, MaxY + MapCpt.viewSizeY, grid, reqionId);
                    }
                    break;
                //最上面到 最下面
                case MoveDir.Down:
                    MaxX = originX + MapCpt.viewSizeX; MaxY = originY + MapCpt.viewSizeY;
                    for (int x = originX; x < MaxX; x++)
                    {
                        // MapGrid grid = GetGridData(x, MaxY);
                        // if (grid == null) Debug.LogError(x);
                        // RemoveGridData(x, MaxY);
                        // SetGridPos(x, originY, grid, reqionId);
                    }
                    break;
                //最右面到 最左面
                case MoveDir.Left:
                    MaxX = originX + MapCpt.viewSizeX; MaxY = originY + MapCpt.viewSizeY;
                    for (int y = originY; y < MaxY; y++)
                    {
                        // MapGrid grid = GetGridData(MaxX, y);
                        // if (grid == null) Debug.LogError(y);
                        // RemoveGridData(MaxX, y);
                        // SetGridPos(originX, y, grid, reqionId);
                    }
                    break;
                //最左面到 最右面
                case MoveDir.Right:
                    MaxX = originX - 1; MaxY = originY + MapCpt.viewSizeY;
                    for (int y = originY; y < MaxY; y++)
                    {
                        // MapGrid grid = GetGridData(MaxX, y);
                        // if (grid == null) Debug.LogError(y);
                        // RemoveGridData(MaxX, y);
                        // SetGridPos(MaxX + MapCpt.viewSizeX, y, grid, reqionId);
                    }
                    break;
            }
        }
        
        public MapGridEntity GetGridEntityByPos(Vector2 pos)
        {
            MapGridEntity MapGrid = GetGridData(pos);
            if (MapGrid == null)
            {
                Log.Error("视野内未发现：" + pos.x + "," + pos.y);
            }
            return MapGrid;
        }

        private MapGridEntity GetGridData(Vector2 pos, MapGridEntity defaultValue = null)
        {
            if (MapCpt.mapGridEntitys.ContainsKey(pos))
            {
                return MapCpt.mapGridEntitys[pos];
            }

            return defaultValue;
        }

        public void Update()
        {
           
        }

        public void OnDestroy()
        {
            
        }
    }
}

