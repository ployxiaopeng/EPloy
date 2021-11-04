using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class RoleSystem : ISystem
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
            SettingCamerafollow(mapEntityCpt.role);
            SettingMove(mapEntityCpt.role);
        }

        private void SettingCamerafollow(Entity entity)
        {
            CameraCpt cameraCpt = HotFixMudule.GameScene.AddCpt<CameraCpt>(entity);
            cameraCpt.camera = mapCpt.mapParent.Find("Camera").GetComponent<Camera>();
        }

        private void SettingMove(Entity entity)
        {
            MoveCpt moveCpt = HotFixMudule.GameScene.AddCpt<MoveCpt>(entity);
            moveCpt.SetMoveObj(entity.GetComponent<RoleCpt>().role);
            moveCpt.moveDir = MoveDir.Stop;
        }

        public void Update()
        {
            if (mapEntityCpt.role.HasComponent<CameraCpt>())
            {
                CameraCpt cameraCpt = mapEntityCpt.role.GetComponent<CameraCpt>();
                RoleCpt RoleCpt = mapEntityCpt.role.GetComponent<RoleCpt>();
                cameraCpt.followPos = RoleCpt.rolePos;
            }
        }

        public void OnDestroy()
        {

        }
    }
}

