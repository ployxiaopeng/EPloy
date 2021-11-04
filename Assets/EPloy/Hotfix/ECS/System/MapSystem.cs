using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }
        private MapCpt mapCpt;
        private IDataTable<DRMap> dataMap;

        public void Start()
        {
            dataMap = HotFixMudule.DataTable.GetDataTable<DRMap>();
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            mapCpt.map = HotFixMudule.GameScene.CreateEntity("Map");
            mapCpt.MapData = dataMap.GetDataRow(10101);
            HotFixMudule.GameScene.AddCpt<MapEntityCpt>(mapCpt.map);
            mapCpt.mapParent = GameObject.Find("Map").transform;
            mapCpt.gridPrefab = mapCpt.mapParent.Find("Mian/Grid").gameObject;
            HotFixMudule.GameScene.CreateSystem<MapRoleSystem>();
            HotFixMudule.GameScene.CreateSystem<MapCameraSystem>();
            HotFixMudule.GameScene.CreateSystem<MapCreateSystem>();
        }

        public void Update()
        {

        }

        public void OnDestroy()
        {

        }
    }
}

