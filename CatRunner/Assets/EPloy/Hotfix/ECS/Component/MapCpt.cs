using EPloy.Hotfix.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public class MapCpt : CptBase
    {
        public int mapId;
        public int PlayerId;
        public Transform roleParent;
        public DRMap mapData;
        public override void Clear()
        {
            base.Clear();
        }
    }
}