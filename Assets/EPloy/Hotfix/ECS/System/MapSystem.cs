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
        private MapEntityCpt mapEntityCpt;
        private IDataTable<DRMap> dataMap;

        public void Start()
        {
            dataMap = HotFixMudule.DataTable.GetDataTable<DRMap>();
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            SettingMap();
            SettingRole();
            HotFixMudule.GameScene.CreateSystem<RoleSystem>();
            HotFixMudule.GameScene.CreateSystem<MoveSystem>();
            HotFixMudule.GameScene.CreateSystem<CameraSystem>();
            HotFixMudule.GameScene.CreateSystem<MapCreateSystem>();
        }

        private void SettingMap()
        {
            mapCpt.map = HotFixMudule.GameScene.CreateEntity("Map");
            mapCpt.MapData = dataMap.GetDataRow(10101);
            mapEntityCpt = HotFixMudule.GameScene.AddCpt<MapEntityCpt>(mapCpt.map);
            mapCpt.mapParent = GameObject.Find("Map").transform;
            mapCpt.gridPrefab = mapCpt.mapParent.Find("Mian/Grid").gameObject;
        }

        private void SettingRole()
        {
            mapEntityCpt.role = HotFixMudule.GameScene.CreateEntity("Role");
            RoleCpt roleCpt = HotFixMudule.GameScene.AddCpt<RoleCpt>(mapEntityCpt.role);
            roleCpt.roleDir = MoveDir.Up;
            roleCpt.role = mapCpt.mapParent.Find("Mian/Role").gameObject;
            roleCpt.role.transform.localPosition = mapCpt.MapData.RoleBornPos;
            roleCpt.role.transform.localEulerAngles = mapCpt.MapData.RolelRotate;
        }

        private void SettingNpc()
        {

        }

        private void SettingEvent()
        {

        }

        public void Update()
        {

        }

        public void OnDestroy()
        {

        }
    }
}