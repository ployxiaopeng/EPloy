using System.Collections.Generic;
using System.Linq;
using EPloy.Hotfix.Table;
using UnityEngine;

namespace EPloy.Hotfix
{
    public class SkillSystem : IReference
    {
        //技能释放索敌    
        public void SkillSpotting(EntityRole entityRole, SkillCpt skillCpt)
        {
            if (skillCpt == null) return;
            skillCpt.hurt = entityRole.roleBaseCpt.att * skillCpt.skillData.HarmArg;
            for (int i = 0; i < HotFixMudule.GameScene.EntityRoles.Count; i++)
            {
                EntityRole targetRole = HotFixMudule.GameScene.EntityRoles[i];
                if (entityRole == targetRole) continue;
                //被攻击减攻击者
                Vector3 vector = targetRole.roleCpt.rolePos - entityRole.roleCpt.rolePos;
                float num = Vector3.Dot(entityRole.roleCpt.role.transform.forward, vector);
                if (num > 0 && num <= skillCpt.dis)
                {
                    if (Mathf.Abs(Vector3.Dot(entityRole.roleCpt.role.transform.right, vector)) < skillCpt.dis / 2)
                    {
                        skillCpt.targetEntitys.Add(targetRole);
                    }
                }
            }

            for (int i = 0; i < skillCpt.targetEntitys.Count; i++)
            {
                SkillHurt(skillCpt.targetEntitys[i], skillCpt);
            }
            ReferencePool.Release(skillCpt);
            entityRole.skillCpt = null;
        }


        //技能伤害计算
        public void SkillHurt(EntityRole targetRole, SkillCpt skillCpt)
        {
            targetRole.roleBaseCpt.curHp = targetRole.roleBaseCpt.curHp - ((int)skillCpt.hurt - targetRole.roleBaseCpt.def);
            Log.Info("敌人hp  " + targetRole.roleBaseCpt.curHp);
            if (targetRole.roleBaseCpt.curHp <= 0)
            {
                Log.Info("死了哦");
            }
        }

        public void Clear()
        {

        }
    }
}

