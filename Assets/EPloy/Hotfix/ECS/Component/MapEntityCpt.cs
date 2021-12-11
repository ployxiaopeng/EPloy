using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图组件
    /// </summary>
    public class MapEntityCpt : Component
    {
        public Entity role;

        public Entity npc;

        public Entity enmity;

        public Dictionary<Vector2, Entity> grids = new Dictionary<Vector2, Entity>();

        public List<Entity> updateGrids = new List<Entity>();
    }
}

