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
        }

        public void PathfindingMove(EntityRole entityRole, MoveCpt moveCpt, Vector3 targetPos)
        {
            if (entityRole.roleCpt.roleState == RoleState.Att || entityRole.roleCpt.roleState == RoleState.Die) return;
            entityRole.roleCpt.actionHandler.OnPathfinding(targetPos, (path) =>
             {
                 moveCpt.path = path;
                 entityRole.roleCpt.roleState = RoleState.Pathfinding;
             });
        }

        public void Update(EntityRole entityRole, MoveCpt moveCpt)
        {
            if (moveCpt.direction != Vector3.zero)
            {
                float angle = UtilVector.Angle360(Vector3.forward, moveCpt.direction);
                if (angle != 0) moveCpt.character.transform.localRotation = Quaternion.Euler(0, angle, 0);
                if (!moveCpt.character.isGrounded) moveCpt.direction.y -= 100 * Time.deltaTime;
                moveCpt.character.Move(moveCpt.direction * Time.deltaTime * moveCpt.speed);
            }

            if (entityRole.roleCpt.roleState == RoleState.Pathfinding)
            {
                float dis = Vector3.Distance(moveCpt.character.transform.position, moveCpt.path.vectorPath[moveCpt.path.vectorPath.Count-1]);
                if (Mathf.Abs(dis) > 0.1f)
                {
                    entityRole.roleCpt.actionHandler.OnMove(1);
                }
                else
                {
                    entityRole.roleCpt.actionHandler.OnMove(0);
                }
            }
        }

        public void Clear()
        {

        }
    }
}

