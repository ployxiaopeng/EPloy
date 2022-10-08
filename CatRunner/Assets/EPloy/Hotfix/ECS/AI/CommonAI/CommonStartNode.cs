using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy.ECS
{

    public class CommonStartNode : BTCondition
    {
        public static CommonStartNode Create()
        {
            CommonStartNode startNode = ReferencePool.Acquire<CommonStartNode>();
            return startNode;
        }

        //¾¯½ä·¶Î§
        public override bool Evaluate(CommonAICpt aiCpt)
        {
            RoleCpt roleCpt = aiCpt.Entity.GetCpt<RoleCpt>();
            RoleCpt player = aiCpt.target.GetCpt<RoleCpt>();

            float dis = Mathf.Abs(Vector3.Distance(roleCpt.rolePos, player.rolePos));
            return dis < 20;
        }

        public override BTResult Execute(CommonAICpt aiCpt)
        {
            RoleCpt roleCpt = aiCpt.Entity.GetCpt<RoleCpt>();
            RoleCpt player = aiCpt.target.GetCpt<RoleCpt>();
            float dis = Mathf.Abs(Vector3.Distance(roleCpt.rolePos, player.rolePos));
            if (dis > 10)
            {
                if (childNodes[0].Evaluate(aiCpt))
                {
                    return childNodes[0].Execute(aiCpt);
                }
            }
            else
            {
                if (childNodes[1].Evaluate(aiCpt))
                {
                    return childNodes[1].Execute(aiCpt);
                }
            }
            return BTResult.Success;
        }
    }
}