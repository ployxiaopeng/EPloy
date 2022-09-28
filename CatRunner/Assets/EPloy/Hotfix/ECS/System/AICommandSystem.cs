using UnityEngine;
using EPloy.Table;

namespace EPloy.ECS
{
    public class AICommandSystem : IReference
    {
        private InputCpt inputCpt;
        public AICommandSystem()
        {
            inputCpt = ECSModule.GameScene.GetSingleCpt<InputCpt>();
        }

        public void Updata(EntityRole entityRole)
        {
            switch (entityRole.roleCpt.roleType)
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
            switch (inputCpt.inputType)
            {
                case UserClrType.Att:
                    ECSModule.SkillSys.Att(entityRole, entityRole.skillCpt);
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Skill1:
                    ECSModule.SkillSys.Skill1(entityRole, entityRole.skillCpt);
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Skill2:
                    ECSModule.SkillSys.Skill2(entityRole, entityRole.skillCpt);
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Skill3:
                    ECSModule.SkillSys.Skill3(entityRole, entityRole.skillCpt);
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Move:
                    ECSModule.moveSys.PlayerMove(entityRole, entityRole.moveCpt, inputCpt);
                    break;
                case UserClrType.Pathfinding:
                    ECSModule.moveSys.PathfindingMove(entityRole, entityRole.moveCpt, inputCpt.targetPos);
                    inputCpt.inputType = UserClrType.None;
                    break;
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


        public void Clear()
        {

        }
    }
}