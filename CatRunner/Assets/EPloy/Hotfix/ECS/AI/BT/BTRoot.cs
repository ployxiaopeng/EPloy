using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy.ECS
{
    public class BTRoot : IReference
    {
        public BTNode startNode { get; private set; }
        public string bTName { get; private set; }

        public void Create(BTNode _startNode, string name)
        {
            startNode = _startNode;
            bTName = name;
        }

        public void Update(CommonAICpt aiCpt)
        {
            if (startNode.Evaluate(aiCpt))
            {
                startNode.Execute(aiCpt);
            }
        }

        public void Destroy()
        {
            ReferencePool.Release(startNode);
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            startNode = null;
            bTName = null;
        }
    }
}
