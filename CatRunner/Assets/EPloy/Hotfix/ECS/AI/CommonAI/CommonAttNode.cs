using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy.ECS
{
    public class CommonAttNode : BTAction
    {
        public static CommonAttNode Create()
        {
            CommonAttNode attNode = ReferencePool.Acquire<CommonAttNode>();
            return attNode;
        }

        public override bool Evaluate(CommonAICpt aiCpt)
        {
            return base.Evaluate(aiCpt);
        }

        public override BTResult Execute(CommonAICpt aiCpt)
        {
            RoleCpt roleCpt = aiCpt.Entity.GetCpt<RoleCpt>();
            RoleCpt player = aiCpt.target.GetCpt<RoleCpt>();

            float dis = Mathf.Abs(Vector3.Distance(roleCpt.rolePos, player.rolePos));
            if (dis<1f)
            {
                ECSModule.SkillSys.Att(aiCpt.Entity, aiCpt.Entity.GetCpt<SkillCpt>());
                Log.Info("ai π•ª˜ ¡À  ≤‚ ‘πÿ±’ai");
                aiCpt.isRuning = false;
            }
            return base.Execute(aiCpt);
        }
    }
}
