using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MapRoleSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }
        private MapCpt mapCpt;
        private MapEntityCpt mapEntityCpt;

        public void Start()
        {
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            CreateRole();
            StartCamerafollow();
        }

        private void CreateRole()
        {
            mapEntityCpt.role = HotFixMudule.GameScene.CreateEntity("Role");
            MapRoleCpt mapRoleCpt = HotFixMudule.GameScene.AddCpt<MapRoleCpt>(mapEntityCpt.role);
            mapRoleCpt.roleDir = MoveDir.Stop;
            mapRoleCpt.UpdateMap = true;
            mapRoleCpt.role = mapCpt.mapParent.Find("Mian/Role").gameObject;
            mapRoleCpt.role.transform.localPosition = mapCpt.MapData.RoleBornPos;
            mapRoleCpt.role.transform.localEulerAngles = mapCpt.MapData.RolelRotate;
        }

        private void StartCamerafollow()
        {
            MapCameraCpt mapCameraCpt = HotFixMudule.GameScene.AddCpt<MapCameraCpt>(mapEntityCpt.role);
            mapCameraCpt.camera = mapCpt.mapParent.Find("Camera").GetComponent<Camera>();
            SetfollowPos();
        }

        public void Update()
        {
            SetfollowPos();
        }

        public void OnDestroy()
        {

        }

        private void SetfollowPos()
        {
            mapEntityCpt.role.GetComponent<MapCameraCpt>().followPos =
                mapEntityCpt.role.GetComponent<MapRoleCpt>().rolePos;
        }
    }
}

