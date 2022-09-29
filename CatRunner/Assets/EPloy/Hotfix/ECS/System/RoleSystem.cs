using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class RoleSystem : IReference
    {
        public void CreatePlayer(MapCpt mapCpt, int roleId)
        {
            //实体
            Entity entity = ECSModule.GameScene.CreatePlayerEntity<Entity>("Player");
            //基本组件
            RoleCpt roleCpt = entity.AddCpt<RoleCpt>();
            roleCpt.roleType =  RoleType.Player;
            roleCpt.SetRoleData(roleId);
            //技能数据
            SkillCpt skillCpt = entity.AddCpt<SkillCpt>();
            skillCpt.SetSkillData(roleCpt.playerData);

            //显示实体
            MapRoleData roleData = MapRoleData.Create(roleId, mapCpt.roleParent, mapCpt.mapData.RoleBornPos, mapCpt.mapData.RolelRotate);
            GameModule.Obj.ShowObj(roleData, (role) =>
            {
                //Obj组件
                roleCpt.roleData = (MapRoleData)role;
                roleCpt.actionHandler = RoleActionHandler.AddActionHandler(roleCpt.role);
                //主角 主相机跟随 移动控制
                ECSModule.cameraSys.SetCameraFollow(entity, MapVisualBoxCpt.Instance.mianCamera, roleCpt.role.transform);
                ECSModule.moveSys.SetMoveControl(entity, roleCpt.role);
            });
        }

        public void CreateMonster(MapCpt mapCpt, int roleId)
        {
            //实体
            Entity entity = ECSModule.GameScene.CreateMonsterEntity<Entity>("Monster");
            //基本组件
            RoleCpt roleCpt = entity.AddCpt<RoleCpt>();
            roleCpt.roleType = RoleType.Monster;
            roleCpt.SetRoleData(roleId);
            //技能数据
            SkillCpt skillCpt = entity.AddCpt<SkillCpt>();
            skillCpt.SetSkillData(roleCpt.playerData);

            //显示实体
            MapRoleData roleData = MapRoleData.Create(roleId, mapCpt.roleParent, mapCpt.mapData.RoleBornPos, mapCpt.mapData.RolelRotate);
            GameModule.Obj.ShowObj(roleData, (role) =>
            {
                //Obj组件
                roleCpt.roleData = (MapRoleData)role;
                roleCpt.actionHandler = RoleActionHandler.AddActionHandler(roleCpt.role);
                roleCpt.role.transform.position += Vector3.forward * 5;
                roleCpt.role.transform.localEulerAngles = Vector3.up * 180;
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

