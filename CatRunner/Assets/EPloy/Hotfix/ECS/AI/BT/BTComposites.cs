using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy.ECS
{
    public abstract class BTComposites : BTNode
    {
        public abstract ComPositesType comPositesType { get; }

        public BTComposites()
        {
            
        }
    }
}
