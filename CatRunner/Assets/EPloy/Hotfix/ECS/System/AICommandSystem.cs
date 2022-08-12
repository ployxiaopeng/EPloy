using UnityEngine;
using EPloy.Game;
using EPloy.Hotfix.Table;

namespace EPloy.Hotfix
{
    public class AICommandSystem : IReference
    {
        private InputCpt inputCpt;
        public AICommandSystem()
        {
            inputCpt = HotFixMudule.GameScene.GetSingleCpt<InputCpt>();
        }

        public void Updata(EntityRole entityRole)
        {
            switch (entityRole.roleBaseCpt.roleType)
            {
                case RoleType.Player:
                    PlayerInput(entityRole);
                    return;
                case RoleType.NPC:
                    return;
                case RoleType.Monster:
                    return;
            }
        }

        //处理玩家输入
        private void PlayerInput(EntityRole entityRole)
        {
            if (entityRole.roleAcitonCpt == null) return;

            if (entityRole.roleAcitonCpt.isHander)
            {
                switch (inputCpt.inputType)
                {
                    case UserClrType.Att:
                        PlayerAttCrl(entityRole, entityRole.roleBaseCpt.AttData);
                        entityRole.roleAcitonCpt.isHander = false;
                        break;
                    case UserClrType.Skill1:
                        PlayerSkillCrl(entityRole, entityRole.roleBaseCpt.Skills[1], RoleAction.Skill1);
                        entityRole.roleAcitonCpt.isHander = false;
                        break;
                    case UserClrType.Skill2:
                        PlayerSkillCrl(entityRole, entityRole.roleBaseCpt.Skills[2], RoleAction.Skill2);
                        entityRole.roleAcitonCpt.isHander = false;
                        break;
                    case UserClrType.Skill3:
                        PlayerSkillCrl(entityRole, entityRole.roleBaseCpt.Skills[3], RoleAction.Skill3);
                        entityRole.roleAcitonCpt.isHander = false;
                        break;
                    case UserClrType.Move:
                        PlayerMoveCrl(entityRole, entityRole.moveCpt);
                        break;
                }
              
            }
            else
            {
                HotFixMudule.GameScene.roleAcitonSys.UpdateRoleAction(entityRole, entityRole.roleAcitonCpt);
            }

            #region wasd
            if (Input.GetKey(KeyCode.W))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.up * 256;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.down * 256;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.left * 256;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.right * 256;
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.zero;
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.zero;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.zero;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                inputCpt.inputType = UserClrType.Move;
                inputCpt.direction = Vector2.zero;
            }
            #endregion
        }

        private void PlayerMoveCrl(EntityRole entityRole, MoveCpt moveCpt)
        {
            moveCpt.direction = new Vector3(inputCpt.direction.x / inputCpt.radius, 0, inputCpt.direction.y / inputCpt.radius);
            HotFixMudule.GameScene.roleAcitonSys.SetRoleMove(entityRole, entityRole.roleAcitonCpt, inputCpt.direction.magnitude / inputCpt.radius);
        }

        private void PlayerSkillCrl(EntityRole entityRole, DRSkillData skillData, string action)
        {
            if (!SkillCDCheck(entityRole, skillData)) return;
            HotFixMudule.GameScene.roleAcitonSys.SetRoleSkill(entityRole, entityRole.roleAcitonCpt, action);
            entityRole.skillCpt = HotFixMudule.GameScene.GetCpt<SkillCpt>(entityRole);
            entityRole.skillCpt.skillData = skillData;
            StopMoveCrl(entityRole);
        }

        private void PlayerAttCrl(EntityRole entityRole, DRSkillData skillData)
        {
            HotFixMudule.GameScene.roleAcitonSys.SetRoleAtt(entityRole, entityRole.roleAcitonCpt);
            entityRole.skillCpt = HotFixMudule.GameScene.GetCpt<SkillCpt>(entityRole);
            entityRole.skillCpt.skillData = skillData;
            StopMoveCrl(entityRole);
        }

        private void StopMoveCrl(EntityRole entityRole)
        {
            entityRole.moveCpt.direction = Vector3.zero;
            inputCpt.direction = Vector2.zero;
            inputCpt.inputType = UserClrType.None;
        }

        private bool SkillCDCheck(EntityRole entityRole, DRSkillData skillData)
        {
            if (Time.time - entityRole.roleBaseCpt.skillCDs[skillData.Id] < skillData.CDTime)
            {
                return false;
            }
            entityRole.roleBaseCpt.skillCDs[skillData.Id] = Time.time;
            return true;
        }

        public void Clear()
        {

        }
    }
}