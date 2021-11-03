using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图相机组件
    /// </summary>
    public class MapCameraCpt : Component
    {
        private const float offsetZ = -5;
        public Camera camera;
        public Vector3 followPos;
        public Vector3 offset = new Vector3(0, 0, offsetZ);
    }
}

