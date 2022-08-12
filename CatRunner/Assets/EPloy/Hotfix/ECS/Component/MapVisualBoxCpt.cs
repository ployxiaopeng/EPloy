using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 地图视野
    /// </summary>
    public class MapVisualBoxCpt : CptBase
    {
        //地图主相机
        public Camera mianCamera;
        //地图视野包围盒大小
        public Vector2 visualBox;
        //当前视野中心
        public Vector2 curVisualCentre;
        //当前视野包围盒边界
        public Vector2 visualBoxMin;
        public Vector2 visualBoxMax;

        public MapVisualBoxCpt()
        {
            visualBoxMin = new Vector2();
            visualBoxMax = new Vector2();
            curVisualCentre = new Vector2();
        }

        public void SetcurVisualCentre(Vector3 position)
        {
            this.curVisualCentre.x = position.x;
            this.curVisualCentre.y = position.z;
        }

        public void CalculateBox()
        {
            visualBoxMin.x = curVisualCentre.x - visualBox.x / 2;
            visualBoxMin.y = visualBoxMin.x + visualBox.x;
            visualBoxMax.x = curVisualCentre.y - visualBox.y / 2;
            visualBoxMax.y = visualBoxMax.x + visualBox.y;
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}