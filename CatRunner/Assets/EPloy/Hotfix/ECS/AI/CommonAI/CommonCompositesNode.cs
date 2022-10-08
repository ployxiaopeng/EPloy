using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy.ECS
{
    public class CommonCompositesNode : BTComposites
    {

        public static CommonCompositesNode Create()
        {
            CommonCompositesNode compositesNode = ReferencePool.Acquire<CommonCompositesNode>();
            return compositesNode;
        }

        public override ComPositesType comPositesType => ComPositesType.Sequence;

        public override bool Evaluate(CommonAICpt aiCpt)
        {
            return base.Evaluate(aiCpt);
        }

        public override BTResult Execute(CommonAICpt aiCpt)
        {
            foreach (var node in childNodes)
            {
                if (!node.Evaluate(aiCpt)) return BTResult.Failure;
                node.Execute(aiCpt);
            }
            return base.Execute(aiCpt);
        }
    }
}
