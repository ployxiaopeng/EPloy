﻿using System.Collections.Generic;
using System.Linq;
using EPloy.Hotfix.Obj;
using EPloy.Hotfix.Table;
using UnityEngine;

namespace EPloy.Hotfix
{
    public class RoleSystem : IReference
    {
        public void CreateRole(EntityMap entityMap, MapCpt mapCpt, RoleType roleType, int roleId)
        {
            //实体
            EntityRole entityRole = HotFixMudule.GameScene.CreateEntityRole("Role");
            //基本组件
            entityRole.roleBaseCpt = HotFixMudule.GameScene.GetCpt<RoleBaseCpt>(entityRole);
            entityRole.roleBaseCpt.roleType = roleType;
            entityRole.roleBaseCpt.SetRoleData(roleId);

            //显示实体
            MapRoleData roleData = MapRoleData.Create(roleId, entityMap.mapCpt.roleParent, mapCpt.mapData.RoleBornPos, mapCpt.mapData.RolelRotate);
            HotFixMudule.Obj.ShowObj(roleData, (role) =>
            {
                //Obj组件
                entityRole.roleCpt = HotFixMudule.GameScene.GetCpt<RoleCpt>(entityRole);
                entityRole.roleCpt.roleData = (MapRoleData)role;
                //动画组件
                HotFixMudule.GameScene.roleAcitonSys.SetAciton(entityRole);
                switch (roleType)
                {
                    case RoleType.Player:    //主角 主相机跟随 移动控制
                        HotFixMudule.GameScene.cameraSys.SetCameraFollow(entityRole, entityMap.mapVisualBoxCpt.mianCamera);
                        HotFixMudule.GameScene.moveSys.SetMoveControl(entityRole);
                        break;
                    case RoleType.NPC:
             
                        break;
                    case RoleType.Monster:
                        entityRole.roleCpt.role.transform.position += Vector3.forward * 5;
                        entityRole.roleCpt.role.transform.localEulerAngles = Vector3.up * 180;
                        break;
                }
            });
        }
        public void OnDestroy()
        {

        }

        public void Clear()
        {

        }
    }
}

