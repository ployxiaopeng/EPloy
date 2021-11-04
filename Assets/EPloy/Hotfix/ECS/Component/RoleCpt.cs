using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public class RoleCpt : Component
    {
        public Vector2 rolePos
        {
            get { return role.transform.position; }
        }

        public MoveDir roleDir;
        public GameObject role;
    }
}

