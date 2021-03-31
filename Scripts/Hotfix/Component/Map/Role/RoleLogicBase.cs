using ETModel;
using GameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public abstract class RoleLogicBase : HotfixEntityLogic
    {
        public RoleAction roleAction { get; protected set; }
        protected Vector3 localPosition
        {
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
        }
        private Animator Animator;

        public override void OnShow(object userData)
        {
            base.OnShow(userData);
            roleAction = RoleAction.Stand;
            Animator = transform.GetComponent<Animator>();
        }

        public void PlayAnim(string animName)
        {
            if (Animator == null)
            {
                Log.Error("没有获取到 Animator 组件 ");
                return;
            }
            Animator.SetTrigger(animName);
        }
        /// <summary>
        /// 角色移动
        /// </summary>
        /// <param name="_MoveDir"></param>
        public abstract void SetRoleMoveDir(MoveDir _MoveDir);
        /// <summary>
        /// 只单单旋转
        /// </summary>
        /// <param name="_MoveDir"></param>
        public abstract void SetRoleRotate(MoveDir _MoveDir);
        /// <summary>
        /// 只单单改变状态
        /// </summary>
        /// <param name="_roleAction"></param>
        public abstract void SetIdle();

        public abstract void SetStateStand();
        public abstract void SetStateCut();
        public abstract void SetStateSearch();
        public abstract void SetStateWalk();
        public abstract void SetStateRotate();

        public abstract void SetRoleTransfer(Vector2 target);

        public abstract void SetRoleSearchPath(List<Vector2> pathList);

        public abstract void SetRoleActive(bool isActive);

        #region 换算方向和角度
        protected Vector3 GetVector2Direction(MoveDir MoveDirection)
        {
            switch (MoveDirection)
            {
                case MoveDir.Up:
                    return Vector3.up;
                case MoveDir.Down:
                    return Vector3.down;
                case MoveDir.Left:
                    return Vector3.left;
                case MoveDir.Right:
                    return Vector3.right;
            }
            return Vector3.zero;
        }
        protected Vector3 GetRotateirection(MoveDir MoveDirection)
        {
            switch (MoveDirection)
            {
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
        protected Vector2 GetDeltaNewVec(MoveDir MoveDirection)
        {
            int x, y;
            switch (MoveDirection)
            {
                case MoveDir.Up:
                    y = Mathf.CeilToInt(localPosition.y);
                    return new Vector2(localPosition.x, y);
                case MoveDir.Down:
                    y = Mathf.FloorToInt(localPosition.y);
                    return new Vector2(localPosition.x, y);
                case MoveDir.Left:
                    x = Mathf.FloorToInt(localPosition.x);
                    return new Vector2(x, localPosition.y);
                case MoveDir.Right:
                    x = Mathf.CeilToInt(localPosition.x);
                    return new Vector2(x, localPosition.y);
            }
            return Vector2.zero;
        }
        #endregion
    }
}