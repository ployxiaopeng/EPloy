using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class SkillSystem : IReference
    {

        public void Att(EntityRole entityRole, SkillCpt skillCpt)
        {
            if (!SetSkillData(entityRole, skillCpt.AttData)) return;
            AnimationHandler animationHandler = entityRole.roleCpt.role.GetComponent<AnimationHandler>();
            animationHandler.OnAtt();
            SetAnimationEvent(entityRole, animationHandler);
        }

        public void Skill1(EntityRole entityRole, SkillCpt skillCpt)
        {
            if (!SetSkillData(entityRole, skillCpt.Skills[1])) return;
            AnimationHandler animationHandler = entityRole.roleCpt.role.GetComponent<AnimationHandler>();
            animationHandler.OnSkill1();
            SetAnimationEvent(entityRole, animationHandler);
        }

        public void Skill2(EntityRole entityRole, SkillCpt skillCpt)
        {
            if (!SetSkillData(entityRole, skillCpt.Skills[2])) return;
            AnimationHandler animationHandler = entityRole.roleCpt.role.GetComponent<AnimationHandler>();
            animationHandler.OnSkill2();
            SetAnimationEvent(entityRole, animationHandler);
        }

        public void Skill3(EntityRole entityRole, SkillCpt skillCpt)
        {
            if (!SetSkillData(entityRole, skillCpt.Skills[3])) return;
            AnimationHandler animationHandler = entityRole.roleCpt.role.GetComponent<AnimationHandler>();
            animationHandler.OnSkill3();
            SetAnimationEvent(entityRole, animationHandler);
        }

        public void SetAnimationEvent(EntityRole entityRole, AnimationHandler animationHandler)
        {
            animationHandler.RegisterHarmEvent((arg) =>
            {
                SkillSpotting(entityRole, entityRole.hurtCpt);
            });
            animationHandler.RegisterOverEvent((arg) =>
            {
                entityRole.roleCpt.roleState = RoleState.Idle;
            });
        }
        public bool SetSkillData(EntityRole entityRole, DRSkillData skillData)
        {
            if (!SkillCDCheck(entityRole, skillData)) return false;
            entityRole.hurtCpt = ECSModule.GameScene.GetCpt<HurtCpt>(entityRole);
            entityRole.hurtCpt.skillData = skillData;
            ECSModule.moveSys.PlayerStopMove(entityRole, entityRole.moveCpt);
            entityRole.roleCpt.roleState = RoleState.Att;
            return true;
        }

        private bool SkillCDCheck(EntityRole entityRole, DRSkillData skillData)
        {
            if (Time.time - entityRole.skillCpt.skillCDs[skillData.Id] < skillData.CDTime)
            {
                return false;
            }
            entityRole.skillCpt.skillCDs[skillData.Id] = Time.time;
            return true;
        }

        //技能释放索敌    
        public void SkillSpotting(EntityRole entityRole, HurtCpt  hurtCpt)
        {
            if (hurtCpt == null) return;
            hurtCpt.hurt = entityRole.roleCpt.att * hurtCpt.skillData.HarmArg;
            for (int i = 0; i < ECSModule.GameScene.entityRoles.Count; i++)
            {
                EntityRole targetRole = ECSModule.GameScene.entityRoles[i];
                if (entityRole == targetRole) continue;
                //被攻击减攻击者
                Vector3 vector = targetRole.roleCpt.rolePos - entityRole.roleCpt.rolePos;
                float num = Vector3.Dot(entityRole.roleCpt.role.transform.forward, vector);
                if (num > 0 && num <= entityRole.skillCpt.dis)
                {
                    if (Mathf.Abs(Vector3.Dot(entityRole.roleCpt.role.transform.right, vector)) < entityRole.skillCpt.dis / 2)
                    {
                        hurtCpt.targetEntitys.Add(targetRole);
                    }
                }
            }

            for (int i = 0; i < hurtCpt.targetEntitys.Count; i++)
            {
                SkillHurt(hurtCpt.targetEntitys[i], entityRole.hurtCpt);
            }
            ReferencePool.Release(entityRole.hurtCpt);
            entityRole.hurtCpt = null;
        }


        //技能伤害计算
        public void SkillHurt(EntityRole targetRole, HurtCpt hurtCpt)
        {
            targetRole.roleCpt.curHp = targetRole.roleCpt.curHp - ((int)hurtCpt.hurt - targetRole.roleCpt.def);
            Log.Info("敌人hp  " + targetRole.roleCpt.curHp);
            if (targetRole.roleCpt.curHp <= 0)
            {
                Log.Info("死了哦");
            }
        }

        public void Clear()
        {

        }
    }
}

