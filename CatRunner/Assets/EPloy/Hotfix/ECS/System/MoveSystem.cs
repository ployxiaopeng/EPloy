using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using Pathfinding;
using UnityEngine;

namespace EPloy.ECS
{
    public class MoveSystem : IReference
    {
        public void SetMoveControl(EntityRole entityRole)
        {
            entityRole.moveCpt = ECSModule.GameScene.GetCpt<MoveCpt>(entityRole);
            entityRole.moveCpt.direction = Vector3.zero;
            entityRole.moveCpt.character = entityRole.roleCpt.role.GetComponent<CharacterController>();
        }

        public void PlayerMove(EntityRole entityRole, MoveCpt moveCpt, InputCpt inputCpt)
        {
            if (entityRole.roleCpt.roleState == RoleState.Att || entityRole.roleCpt.roleState == RoleState.Die) return;
            moveCpt.direction = new Vector3(inputCpt.direction.x / inputCpt.radius, 0, inputCpt.direction.y / inputCpt.radius);
            entityRole.roleCpt.actionHandler.OnMove(inputCpt.direction.magnitude / inputCpt.radius);
            if (entityRole.roleCpt.roleState == RoleState.Pathfinding)
            {
                moveCpt.path = null;
                entityRole.roleCpt.roleState = RoleState.Move;
            }
        }

        public void PlayerStopMove(EntityRole entityRole, MoveCpt moveCpt)
        {
            moveCpt.direction = Vector3.zero;
            moveCpt.character.SimpleMove(Vector3.zero);
            entityRole.roleCpt.actionHandler.OnMove(0);
        }

        public void PathfindingMove(EntityRole entityRole, MoveCpt moveCpt, Vector3 targetPos)
        {
            if (entityRole.roleCpt.roleState == RoleState.Att || entityRole.roleCpt.roleState == RoleState.Die) return;
            entityRole.roleCpt.actionHandler.OnPathfinding(targetPos, (path) =>
             {
                 moveCpt.path = path;
                 entityRole.roleCpt.roleState = RoleState.Pathfinding;
                 entityRole.roleCpt.actionHandler.OnMove(1);
             });
        }

        public void Update(EntityRole entityRole, MoveCpt moveCpt)
        {
            if (moveCpt.direction != Vector3.zero)
            {
                float angle = UtilVector.Angle360(Vector3.forward, moveCpt.direction);
                entityRole.roleCpt.roleRotation = Quaternion.Lerp(entityRole.roleCpt.roleRotation, Quaternion.Euler(0, angle, 0), Time.deltaTime * 10);
                moveCpt.character.SimpleMove(moveCpt.direction * Time.deltaTime * moveCpt.speed);
            }

            if (entityRole.roleCpt.roleState == RoleState.Pathfinding)
            {
                if (moveCpt.path.vectorPath.Count > 0)
                {
                    moveCpt.direction = (moveCpt.path.vectorPath[0] - entityRole.roleCpt.rolePos).normalized;
                    float dis = Vector3.Distance(entityRole.roleCpt.rolePos, moveCpt.path.vectorPath[0]);
                    if (Mathf.Abs(dis) < 0.5f) moveCpt.path.vectorPath.RemoveAt(0);
                }
                else 
                {
                    PlayerStopMove(entityRole, moveCpt);
                    entityRole.roleCpt.roleState = RoleState.Idle;
                }
            }
        }

        public void Clear()
        {

        }
    }
}

