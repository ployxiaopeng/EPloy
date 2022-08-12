﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    public enum UserClrType
    {
        None,
        Move,
        Att,
        Skill1,
        Skill2,
        Skill3,
    }

    /// <summary>
    /// 用户输入单例组件
    /// </summary>
    public class InputCpt : IReference
    {
        public float radius = 256;
        public UserClrType inputType;
        //用户输入移动
        public Vector2 direction = new Vector3(0, 0);
        //用户放技能
        public int skillId = 0;
        //单点判断 当前最后按下什么键位 如果 超过一定时间不处理
        public float clickTime;

        public void Clear()
        {

        }
    }
}