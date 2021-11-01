using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public class MapRoleCpt : Component
    {
        public Vector2 rolePos;
        public MoveDir roleDir = MoveDir.Stop;
        public int reqionId;
        public bool UpdateMap;
    }
}

