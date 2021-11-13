using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图生成组件
    /// </summary>
    public class MapCreateCpt : Component
    {
        public int viewSizeX;
        public int viewSizeY;
        public Transform mapReqion;
        public int regionId;

        public int minX
        {
            get { return (int) newCreate.x - viewSizeX / 2; }
        }

        public int minY
        {
            get { return (int) newCreate.y - viewSizeY / 2; }
        }

        public int maxX
        {
            get { return minX + viewSizeX; }
        }

        public int maxY
        {
            get { return minY + viewSizeY; }
        }

        public Vector2 newCreate { get; private set; }

        public Vector2 CurCreate = Vector2.zero;

        public void SetCreate(Vector2 newCreate)
        {
            this.newCreate = new Vector2(Mathf.CeilToInt(newCreate.x), Mathf.CeilToInt(newCreate.y));
        }

        public bool isUpdate
        {
            get { return newCreate != CurCreate; }
        }

        public void SetRegionName()
        {
            mapReqion.name = string.Format("map{0}", regionId);
        }
    }
}

