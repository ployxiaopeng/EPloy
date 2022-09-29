using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class SkillSystem : IReference
    {

        public void Att(Entity entity, SkillCpt skillCpt)
        {
            if (!SetSkillData(entity, skillCpt.AttData)) return;
            entity.GetCpt<RoleCpt>().actionHandler.OnAtt("Att1");
            entity.GetCpt<RoleCpt>().actionHandler.RegisterHandler(entity, HarmHandler, OverHandler);
        }

        public void Skill1(Entity entity, SkillCpt skillCpt)
        {
            if (!SetSkillData(entity, skillCpt.Skills[1])) return;
            entity.GetCpt<RoleCpt>().actionHandler.OnAtt("Skill1");
            entity.GetCpt<RoleCpt>().actionHandler.RegisterHandler(entity, HarmHandler, OverHandler);
        }

        public void Skill2(Entity entity, SkillCpt skillCpt)
        {
            if (!SetSkillData(entity, skillCpt.Skills[2])) return;
            entity.GetCpt<RoleCpt>().actionHandler.OnAtt("Skill2");
            entity.GetCpt<RoleCpt>().actionHandler.RegisterHandler(entity, HarmHandler, OverHandler);
        }

        public void Skill3(Entity entity, SkillCpt skillCpt)
        {
            if (!SetSkillData(entity, skillCpt.Skills[3])) return;
            entity.GetCpt<RoleCpt>().actionHandler.OnAtt("Skill3");
            entity.GetCpt<RoleCpt>().actionHandler.RegisterHandler(entity, HarmHandler, OverHandler);
        }

        public void HarmHandler(int arg, object data)
        {
            Entity entity = data as Entity;
            SkillSpotting(entity, entity.GetCpt<HurtCpt>());
        }

        public void OverHandler(int arg, object data)
        {
            Entity entity = data as Entity;
            entity.GetCpt<RoleCpt>().roleState = RoleState.Idle;
        }
        public bool SetSkillData(Entity entity, DRSkillData skillData)
        {
            if (!SkillCDCheck(entity, skillData) || entity.HasCpt<HurtCpt>()) return false;
            HurtCpt hurtCpt= entity.AddCpt<HurtCpt>(entity);
            hurtCpt.skillData = skillData;
            ECSModule.moveSys.PlayerStopMove(entity, entity.GetCpt<MoveCpt>());
            entity.GetCpt<RoleCpt>().roleState = RoleState.Att;
            return true;
        }

        private bool SkillCDCheck(Entity entity, DRSkillData skillData)
        {
            SkillCpt skillCpt = entity.GetCpt<SkillCpt>();
            if (Time.time - skillCpt.skillCDs[skillData.Id] < skillData.CDTime)
            {
                return false;
            }
            skillCpt.skillCDs[skillData.Id] = Time.time;
            return true;
        }

        //技能释放索敌    
        public void SkillSpotting(Entity entity, HurtCpt hurtCpt)
        {
            if (hurtCpt == null) return;

            SkillCpt skillCpt = entity.GetCpt<SkillCpt>();
            RoleCpt roleCpt = entity.GetCpt<RoleCpt>();
            Entity target; Vector3 vector; float num;

            hurtCpt.hurt = roleCpt.att * hurtCpt.skillData.HarmArg;
            switch (roleCpt.roleType)
            {
                case RoleType.Player:
                    for (int i = 0; i < ECSModule.GameScene.monsterEntitys.Count; i++)
                    {
                        target = ECSModule.GameScene.monsterEntitys[i];
                        //被攻击减攻击者
                        vector = target.GetCpt<RoleCpt>().rolePos - roleCpt.rolePos;
                        num = Vector3.Dot(roleCpt.role.transform.forward, vector);
                        if (num > 0 && num <= skillCpt.dis && Mathf.Abs(Vector3.Dot(roleCpt.role.transform.right, vector)) < skillCpt.dis / 2)
                        {
                            hurtCpt.targetEntitys.Add(target);
                        }
                    }
                    break;
                case RoleType.NPC:
                    break;
                case RoleType.Monster:
                    target = ECSModule.GameScene.entityPlayer;
                    //被攻击减攻击者
                    vector = target.GetCpt<RoleCpt>().rolePos - roleCpt.rolePos;
                    num = Vector3.Dot(roleCpt.role.transform.forward, vector);
                    if (num > 0 && num <= skillCpt.dis && Mathf.Abs(Vector3.Dot(roleCpt.role.transform.right, vector)) < skillCpt.dis / 2)
                    {
                        hurtCpt.targetEntitys.Add(target);
                    }
                    break;
                default:
                    break;
            }

            for (int i = 0; i < hurtCpt.targetEntitys.Count; i++)
            {
                SkillHurt(hurtCpt.targetEntitys[i], entity.GetCpt<HurtCpt>());
            }
            entity.RemoveCpt<HurtCpt>();
        }

        //技能伤害计算
        public void SkillHurt(Entity target, HurtCpt hurtCpt)
        {
            RoleCpt roleCpt = target.GetCpt<RoleCpt>();
            roleCpt.curHp = roleCpt.curHp - ((int)hurtCpt.hurt - roleCpt.def);
            Log.Info("敌人hp  " + roleCpt.curHp);
            if (roleCpt.curHp <= 0)
            {
                Log.Info("死了哦");
            }
        }

        public void Clear()
        {

        }
    }
}

