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

        //处理玩家输入
        public void PlayerUpdateInput(Entity entity)
        {
            switch (inputCpt.inputType)
            {
                case UserClrType.Att:
                    ECSModule.SkillSys.Att(entity, entity.GetCpt<SkillCpt>());
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Skill1:
                    ECSModule.SkillSys.Skill1(entity, entity.GetCpt<SkillCpt>());
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Skill2:
                    ECSModule.SkillSys.Skill2(entity, entity.GetCpt<SkillCpt>());
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Skill3:
                    ECSModule.SkillSys.Skill3(entity, entity.GetCpt<SkillCpt>());
                    inputCpt.inputType = UserClrType.None;
                    break;
                case UserClrType.Move:
                    ECSModule.moveSys.PlayerMove(entity, entity.GetCpt<MoveCpt>(), inputCpt);
                    break;
                case UserClrType.Pathfinding:
                    entity.GetCpt<MoveCpt>().target = inputCpt.targetPos;
                    ECSModule.moveSys.PathfindingMove(entity, entity.GetCpt<MoveCpt>());
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

        //怪物AI
        public void MoserAIUodate(Entity entity, Entity entityPlayer)
        {
            CommonAICpt aICpt;
            if (entity.HasGetCpt(out aICpt))
            {
                //复杂的以后在说
                if (!aICpt.isRuning) return;
                aICpt.target = entityPlayer;
                BTRoot aiBt = ECSModule.GetBehaviorTree(aICpt.aiName);
                if (aiBt != null) aiBt.Update(aICpt);
            }
        }

        public void Clear()
        {

        }
    }
}