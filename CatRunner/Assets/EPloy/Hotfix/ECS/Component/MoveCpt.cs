using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{

    /// <summary>
    /// 移动组件
    /// </summary>
    public class MoveCpt : CptBase
    {
        public CharacterController character;
        public Vector3 direction;
        public float speed = 150f;

        //已下是寻路
        public Vector3 target; //追踪目标点
        public Vector3 pathTarget; //当前追踪目标点
        public bool isTrace;//是否一直追踪
        public bool isUpdatePath;//是否更新路线
        public Path path;//当前寻路路径
        public bool isComplete;//寻路是否完成

        public override void Clear()
        {
            target = Vector3.zero;
            direction = Vector3.zero;
            pathTarget = Vector3.zero;
            isTrace = false;
            isUpdatePath = false;
            path = null;
            base.Clear();
        }
    }
}

