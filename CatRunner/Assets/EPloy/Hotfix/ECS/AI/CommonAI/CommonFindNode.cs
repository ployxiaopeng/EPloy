using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{

    public class CommonFindNode : BTAction
    {
        public static CommonFindNode Create()
        {
            CommonFindNode findNode = ReferencePool.Acquire<CommonFindNode>();
            return findNode;
        }

        public override BTResult Execute(CommonAICpt aiCpt)
        {
            RoleCpt roleCpt = aiCpt.Entity.GetCpt<RoleCpt>();
            RoleCpt player = aiCpt.target.GetCpt<RoleCpt>();
            MoveCpt moveCpt;
            if (!aiCpt.Entity.HasGetCpt(out moveCpt))
            {
                ECSModule.moveSys.SetMoveControl(aiCpt.Entity, roleCpt.role);
                moveCpt = aiCpt.Entity.GetCpt<MoveCpt>();
                moveCpt.isTrace = true;
                moveCpt.isComplete = false;
            }
            moveCpt.target = player.rolePos;
            ECSModule.moveSys.PathfindingMove(aiCpt.Entity, moveCpt);

            if (moveCpt.isComplete) aiCpt.Entity.RemoveCpt<MoveCpt>();

            return BTResult.Running;
        }
    }
}
