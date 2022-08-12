using EPloy.Hotfix.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 角色状态
    /// </summary>
    public static class RoleAction
    {
        public const string Move = "Move";
        public const string Att1 = "Att1";//普攻第一段
        public const string Att2 = "Att2";//普攻第二段
        public const string Skill1 = "Skill1";//技能1
        public const string Skill2 = "Skill2";//技能2
        public const string Skill3 = "Skill3";//技能3
    }

    /// <summary>
    /// 角色Action组件
    /// </summary>
    public class RoleAcitonCpt : CptBase
    {
        // 当前所处Action
        public string curAction;
        public Animator animator;

        public int attCount;
        //当前动画播放进度
        public float curActionState
        {
            get
            {
                return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }
        }

        //当前动画播放进度
        public float curActionTime
        {
            get
            {
                return animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }

        //是否可以处理输入单一动作 move 自己循环不管
        public bool isHander = true;
        public override void Clear()
        {
            base.Clear();
        }
    }
}