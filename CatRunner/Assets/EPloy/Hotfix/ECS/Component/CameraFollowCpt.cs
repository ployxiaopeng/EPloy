﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    /// <summary>
    /// 地图相机跟随组件
    /// </summary>
    public class CameraFollowCpt : CptBase
    {
        public float speed = 5;
        public Vector3 offset = new Vector3(0, 5, -5);
        public Camera camera;
        public Transform target;

        public override void Clear()
        {
            base.Clear();
            camera = null;
        }
    }
}

