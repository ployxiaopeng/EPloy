using System;
using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy.ECS
{
    public class RoleAcitonSystem : IReference
    {
        public void SetAciton(EntityRole entityRole)
        {
            Animator animator = entityRole.roleCpt.role.GetComponent<Animator>();
            if (animator != null)
            {
                entityRole.roleAcitonCpt = ECSModule.GameScene.GetCpt<RoleAcitonCpt>(entityRole);
                entityRole.roleAcitonCpt.animator = animator;
                entityRole.roleAcitonCpt.curAction = RoleAction.Move;
            }
        }

        public void SetRoleSkill(EntityRole entityRole, RoleAcitonCpt roleAcitonCpt, string roleAction)
        {
            roleAcitonCpt.curAction = roleAction;
            roleAcitonCpt.animator.SetTrigger(roleAction);
        }

        public void SetRoleAtt(EntityRole entityRole, RoleAcitonCpt roleAcitonCpt)
        {
            int type = UnityEngine.Random.Range(0, 2);
            if (type == 0)
            {
                roleAcitonCpt.curAction = RoleAction.Att1;
                roleAcitonCpt.animator.SetTrigger(roleAcitonCpt.curAction);
                return;
            }
            if (type == 1)
            {
                roleAcitonCpt.curAction = RoleAction.Att2;
                roleAcitonCpt.animator.SetTrigger(roleAcitonCpt.curAction);
                return;
            }
        }

        public void SetRoleMove(EntityRole entityRole, RoleAcitonCpt roleAcitonCpt, float speed)
        {
            roleAcitonCpt.curAction = RoleAction.Move;
            roleAcitonCpt.animator.SetFloat(roleAcitonCpt.curAction, speed);
        }

        public void UpdateRoleAction(EntityRole entityRole, RoleAcitonCpt roleAcitonCpt)
        {
            switch (roleAcitonCpt.curAction)
            {
                case RoleAction.Att1:
                    //当前动画播放未完毕 发生连击
                    if (roleAcitonCpt.curActionState < 0.9f) return;
                    roleAcitonCpt.isHander = true;
                    roleAcitonCpt.curAction = RoleAction.Move;
                    ECSModule.SkillSys.SkillSpotting(entityRole, entityRole.skillCpt);
                    break;
                case RoleAction.Att2:
                    //当前动画播放未完毕
                    if (roleAcitonCpt.curActionState < 0.9f) return;
                    roleAcitonCpt.isHander = true;
                    roleAcitonCpt.curAction = RoleAction.Move;
                    ECSModule.SkillSys.SkillSpotting(entityRole, entityRole.skillCpt);
                    break;
                case RoleAction.Skill1:
                    //当前动画播放未完毕
                    if (roleAcitonCpt.curActionState < 0.9f) return;
                    roleAcitonCpt.isHander = true;
                    roleAcitonCpt.curAction = RoleAction.Move;
                    ECSModule.SkillSys.SkillSpotting(entityRole, entityRole.skillCpt);
                    break;
                case RoleAction.Skill2:
                    //当前动画播放未完毕
                    if (roleAcitonCpt.curActionState < 0.9f) return;
                    roleAcitonCpt.isHander = true;
                    roleAcitonCpt.curAction = RoleAction.Move;
                    ECSModule.SkillSys.SkillSpotting(entityRole, entityRole.skillCpt);
                    break;
                case RoleAction.Skill3:
                    //当前动画播放未完毕
                    if (roleAcitonCpt.curActionState < 0.9f) return;
                    roleAcitonCpt.isHander = true;
                    roleAcitonCpt.curAction = RoleAction.Move;
                    ECSModule.SkillSys.SkillSpotting(entityRole, entityRole.skillCpt);
                    break;
            }
        }

        public void Clear()
        {

        }
    }
}

