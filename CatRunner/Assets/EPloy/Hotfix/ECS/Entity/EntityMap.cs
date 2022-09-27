using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EPloy.ECS
{
    public class EntityMap : Entity
    {
        public MapCpt mapCpt;
        public MapVisualBoxCpt mapVisualBoxCpt;
        public override void Clear()
        {
            base.Clear();
            if (mapCpt != null)
            {
                ReferencePool.Release(mapCpt);
                mapCpt = null;
            }
            if (mapVisualBoxCpt != null)
            {
                ReferencePool.Release(mapVisualBoxCpt);
                mapVisualBoxCpt = null;
            }
        }
    }
}
