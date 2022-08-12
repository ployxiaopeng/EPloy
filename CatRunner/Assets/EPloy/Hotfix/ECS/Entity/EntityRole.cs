using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EPloy.ECS
{
    public class EntityRole : Entity
    {
        public RoleCpt roleCpt;
        public RoleBaseCpt roleBaseCpt;
        public RoleAcitonCpt roleAcitonCpt;
        public MoveCpt moveCpt;
        public CameraFollowCpt cameraFollowCpt;
        public SkillCpt skillCpt;

        public override void Clear()
        {
            base.Clear();

        }
    }
}
