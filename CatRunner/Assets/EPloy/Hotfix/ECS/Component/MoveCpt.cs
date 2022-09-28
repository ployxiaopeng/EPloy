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
        public float speed = 2.5f;

        public Path path;
        public override void Clear()
        {
            base.Clear();
        }
    }
}

