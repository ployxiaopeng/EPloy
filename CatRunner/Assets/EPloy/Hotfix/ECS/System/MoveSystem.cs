using System.Collections.Generic;
using System.Linq;
using EPloy.Hotfix.Table;
using UnityEngine;

namespace EPloy.Hotfix
{
    public class MoveSystem : IReference
    {
        public void SetMoveControl(EntityRole entityRole)
        {
            entityRole.moveCpt = HotFixMudule.GameScene.GetCpt<MoveCpt>(entityRole);
            entityRole.moveCpt.direction = Vector3.zero;
            entityRole.moveCpt.character = entityRole.roleCpt.role.GetComponent<CharacterController>();
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

