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
        }
    }
}
