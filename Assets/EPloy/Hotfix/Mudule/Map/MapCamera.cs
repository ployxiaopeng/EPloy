using UnityEngine;
using System.Collections;

namespace EPloy
{
    //地图相机管理
    public partial class MapMudule : IHotfixModule
    {
        private Transform Camera;
        private Transform target;
        private float offsetZ = -5;
        private Vector3 offset;

        public void StartCamera(Transform _target)
        {
            this.target = _target;
            offset = new Vector3(0, 0, offsetZ);
        }

        private void RemoveCamera()
        {
            this.target = null;
        }

        public void Update()
        {
            if (target == null) return;
            Camera.position = target.position + offset;
        }
    }
}