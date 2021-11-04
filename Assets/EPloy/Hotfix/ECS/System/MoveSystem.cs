using System.Collections.Generic;
using System.Linq;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class MoveSystem : ISystem
    {
        public int Priority
        {
            get => 100;
        }

        public bool IsPause { get; set; }
        private MapCpt mapCpt;
        private MapEntityCpt mapEntityCpt;

        public void Start()
        {
            mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
            mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
        }

        public void Update()
        {
            if (mapEntityCpt.role.HasComponent<MoveCpt>())
            {
                MoveCpt moveCpt = mapEntityCpt.role.GetComponent<MoveCpt>();
                switch (moveCpt.moveDir)
                {
                    case MoveDir.Stop:
                        moveCpt.PlayAnim(RoleAction.Stand);
                        break;
                    case MoveDir.Up:
                        moveCpt.transform.position += Vector3.up * Time.deltaTime * moveCpt.speed;
                        moveCpt.PlayAnim(RoleAction.Walk);
                        moveCpt.transform.eulerAngles = GetRotation(MoveDir.Up);
                        break;
                    case MoveDir.Down:
                        moveCpt.transform.position += Vector3.down * Time.deltaTime * moveCpt.speed;
                        moveCpt.PlayAnim(RoleAction.Walk);
                        moveCpt.transform.eulerAngles = GetRotation(MoveDir.Down);
                        break;
                    case MoveDir.Left:
                        moveCpt.transform.position += Vector3.left * Time.deltaTime * moveCpt.speed;
                        moveCpt.PlayAnim(RoleAction.Walk);
                        moveCpt.transform.eulerAngles = GetRotation(MoveDir.Left);
                        break;
                    case MoveDir.Right:
                        moveCpt.transform.position += Vector3.right * Time.deltaTime * moveCpt.speed;
                        moveCpt.PlayAnim(RoleAction.Walk);
                        moveCpt.transform.eulerAngles = GetRotation(MoveDir.Right);
                        break;
                }
            }
        }

        private Vector3 GetRotation(MoveDir moveDir)
        {
            switch (moveDir)
            {
                case MoveDir.Stop:
                    return Vector3.zero;
                case MoveDir.Up:
                    return new Vector3(0, 0, -180);
                case MoveDir.Down:
                    return new Vector3(0, 0, 0);
                case MoveDir.Left:
                    return new Vector3(0, 0, -90);
                case MoveDir.Right:
                    return new Vector3(0, 0, 90);
            }

            return Vector3.zero;
        }

        public void OnDestroy()
        {

        }
    }
}

