using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{

    public enum AIType
    {
        Monster,
        Npc,
        Boss,
        Leader,
        Quest,
    }


    public enum AIState
    {
        
    }

    /// <summary>
    /// 角色ai组件
    /// </summary>
    public class RoleAICpt : CptBase
    {
        public AIType aIType;

        public override void Clear()
        {
            base.Clear();
        }
    }
}