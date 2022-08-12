using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class RoleSystem : IReference
    {
        public void CreateRole(EntityMap entityMap, MapCpt mapCpt, RoleType roleType, int roleId)
        {
            //实体
            EntityRole entityRole = ECSModule.GameScene.CreateEntityRole("Role");
            //基本组件
            entityRole.roleBaseCpt = ECSModule.GameScene.GetCpt<RoleBaseCpt>(entityRole);
            entityRole.roleBaseCpt.roleType = roleType;
            entityRole.roleBaseCpt.SetRoleData(roleId);

            //显示实体
            MapRoleData roleData = MapRoleData.Create(roleId, entityMap.mapCpt.roleParent, mapCpt.mapData.RoleBornPos, mapCpt.mapData.RolelRotate);
            GameModule.Obj.ShowObj(roleData, (role) =>
            {
                //Obj组件
                entityRole.roleCpt = ECSModule.GameScene.GetCpt<RoleCpt>(entityRole);
                entityRole.roleCpt.roleData = (MapRoleData)role;
                //动画组件
                ECSModule.roleAcitonSys.SetAciton(entityRole);
                switch (roleType)
                {
                    case RoleType.Player:    //主角 主相机跟随 移动控制
                        ECSModule.cameraSys.SetCameraFollow(entityRole, entityMap.mapVisualBoxCpt.mianCamera);
                        ECSModule.moveSys.SetMoveControl(entityRole);
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

