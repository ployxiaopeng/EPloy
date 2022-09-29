using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using Pathfinding;
using UnityEngine;

namespace EPloy.ECS
{
    public class MoveSystem : IReference
    {
        public void SetMoveControl(Entity entity,GameObject gameObject)
        {
            MoveCpt moveCpt = entity.AddCpt<MoveCpt>();
            moveCpt.direction = Vector3.zero;
            moveCpt.character = gameObject.GetComponent<CharacterController>();
        }

        public void PlayerMove(Entity entity, MoveCpt moveCpt, InputCpt inputCpt)
        {
            RoleCpt roleCpt = entity.GetCpt<RoleCpt>();
            if (roleCpt.roleState == RoleState.Att || roleCpt.roleState == RoleState.Die) return;
            moveCpt.direction = new Vector3(inputCpt.direction.x / inputCpt.radius, 0, inputCpt.direction.y / inputCpt.radius);
            roleCpt.actionHandler.OnMove(inputCpt.direction.magnitude / inputCpt.radius);
            if (roleCpt.roleState == RoleState.Pathfinding)
            {
                moveCpt.path = null;
                roleCpt.roleState = RoleState.Move;
            }
        }

        public void PlayerStopMove(Entity entity, MoveCpt moveCpt)
        {
            moveCpt.direction = Vector3.zero;
            moveCpt.character.SimpleMove(Vector3.zero);
            entity.GetCpt<RoleCpt>().actionHandler.OnMove(0);
        }

        public void PathfindingMove(Entity entity, MoveCpt moveCpt, Vector3 targetPos)
        {
            RoleCpt roleCpt = entity.GetCpt<RoleCpt>();
            if (roleCpt.roleState == RoleState.Att || roleCpt.roleState == RoleState.Die) return;
            roleCpt.actionHandler.OnPathfinding(targetPos, (path) =>
             {
                 moveCpt.path = path;
                 roleCpt.roleState = RoleState.Pathfinding;
                 roleCpt.actionHandler.OnMove(1);
             });
        }

        public void Update(Entity entity, MoveCpt moveCpt)
        {
            RoleCpt roleCpt = entity.GetCpt<RoleCpt>();
            if (moveCpt.direction != Vector3.zero)
            {
                float angle = UtilVector.Angle360(Vector3.forward, moveCpt.direction);
                roleCpt.roleRotation = Quaternion.Lerp(roleCpt.roleRotation, Quaternion.Euler(0, angle, 0), Time.deltaTime * 10);
                moveCpt.character.SimpleMove(moveCpt.direction * Time.deltaTime * moveCpt.speed);
            }
            
            if (roleCpt.roleState == RoleState.Pathfinding)
            {
                if (moveCpt.path.vectorPath.Count > 0)
                {
                    moveCpt.direction = (moveCpt.path.vectorPath[0] - roleCpt.rolePos).normalized;
                    float dis = Vector3.Distance(roleCpt.rolePos, moveCpt.path.vectorPath[0]);
                    if (Mathf.Abs(dis) < 0.5f) moveCpt.path.vectorPath.RemoveAt(0);
                }
                else 
                {
                    PlayerStopMove(entity, moveCpt);
                    roleCpt.roleState = RoleState.Idle;
                }
            }
        }

        public void Clear()
        {

        }
    }
}

