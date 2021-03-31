using UnityEngine;
using System.Collections;
using GameFramework.Event;
using ETModel;

namespace ETHotfix
{
    //��ͼ�������
    public partial class MapComponet : Component
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
        public override void Update()
        {
            if (target == null) return;
            Camera.position = target.position + offset;
        }
    }
}