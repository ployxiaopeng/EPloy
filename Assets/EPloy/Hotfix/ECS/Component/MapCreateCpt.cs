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
            get ;
            private set;
        }

        public int minY
        {
            get ;
            private set;
        }

        public int maxX
        {
            get ;
            private set;
        }
        public int maxY
        {
            get ;
            private set;
        }

        public Vector2 newCreate { get; private set; }

        public Vector2 CurCreate = Vector2.zero;

        public void SetCreate(Vector2 newCreate)
        {
            this.newCreate = new Vector2(Mathf.CeilToInt(newCreate.x), Mathf.CeilToInt(newCreate.y));
            if (isUpdate)
            {
                minX=(int) newCreate.x - viewSizeX / 2;
                minY=(int) newCreate.y - viewSizeY / 2;
                maxX = minX + viewSizeX;
                maxY=minY + viewSizeY;
            }
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

