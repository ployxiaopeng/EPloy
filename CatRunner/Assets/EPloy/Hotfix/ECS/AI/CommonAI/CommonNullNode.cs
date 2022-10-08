using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{

    public class CommonNullNode : BTAction
    {
        public static CommonNullNode Create()
        {
            CommonNullNode nullNode = ReferencePool.Acquire<CommonNullNode>();
            return nullNode;
        }

        public override bool Evaluate(CommonAICpt aiCpt)
        {
            return base.Evaluate(aiCpt);
        }

        public override BTResult Execute(CommonAICpt aiCpt)
        {
            Log.Info("¼ÌÐø¾¯½ä");
            return base.Execute(aiCpt);
        }
    }
}
