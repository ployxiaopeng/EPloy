using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
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
            if (moveCpt  ==null || entityRole.roleCpt.roleState == RoleState.Att || entityRole.roleCpt.roleState == RoleState.Die) return;
            moveCpt.direction = new Vector3(inputCpt.direction.x / inputCpt.radius, 0, inputCpt.direction.y / inputCpt.radius);
            AnimationHandler animationHandler = entityRole.roleCpt.role.GetComponent<AnimationHandler>();
            animationHandler.OnMove(inputCpt.direction.magnitude / inputCpt.radius);
        }

        public void PlayerStopMove(EntityRole entityRole, MoveCpt moveCpt)
        {
            moveCpt.direction = Vector3.zero;
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
        }

        public void Clear()
        {

        }
    }
}

