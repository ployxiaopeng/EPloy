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
        public MoveCpt moveCpt;
        public CameraFollowCpt cameraFollowCpt;
        public SkillCpt skillCpt;
        public HurtCpt hurtCpt;

        public override void Clear()
        {
            base.Clear();
            if (roleCpt != null)
            {
                ReferencePool.Release(roleCpt);
                roleCpt = null;
            }

            if (moveCpt != null)
            {
                ReferencePool.Release(moveCpt);
                moveCpt= null;
            }

            if (cameraFollowCpt != null)
            {
                ReferencePool.Release(cameraFollowCpt);
                cameraFollowCpt = null;
            }
            if (skillCpt != null)
            {
                ReferencePool.Release(skillCpt);
                skillCpt = null;
            }
            if (hurtCpt != null)
            {
                ReferencePool.Release(hurtCpt);
                hurtCpt = null;
            }
        }
    }
}
