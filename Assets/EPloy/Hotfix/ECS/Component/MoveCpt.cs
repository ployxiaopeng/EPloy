using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 移动组件
    /// </summary>
    public class MoveCpt : Component
    {
        public Animator animator { get; private set; }
        public Transform transform { get; private set; }
        public MoveDir moveDir;
        public RoleAction roleAction{ get; private set; }
        public float speed = 1.5f;

        public void SetMoveObj(GameObject gameObject)
        {
            transform = gameObject.transform;
            animator = gameObject.GetComponent<Animator>();
            roleAction = RoleAction.Stand;
        }

        public void PlayAnim(RoleAction roleAction)
        {
            if (this.roleAction==roleAction) return;
            animator.SetTrigger(roleAction.ToString());
            this.roleAction = roleAction;
        }
    }
}

