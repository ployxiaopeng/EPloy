using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    public class UserInputEvt : EventArg
    {
        public override int id
        {
            get
            {
                return EventId.UserInputEvt;
            }
        }

        public UserClrType inputType { get; private set; }
        public Vector3 dir { get; private set; }
        public int skillId { get; private set; }

        public static UserInputEvt Create(UserClrType inputType, Vector3 dir)
        {
            UserInputEvt evt = ReferencePool.Acquire<UserInputEvt>();
            evt.inputType = inputType;
            evt.dir = dir;
            return evt;
        }

        public static UserInputEvt Create(UserClrType inputType, int skillId)
        {
            UserInputEvt evt = ReferencePool.Acquire<UserInputEvt>();
            evt.inputType = inputType;
            evt.skillId = skillId;
            return evt;
        }

        public override void Clear()
        {
            inputType = UserClrType.None;
            dir = Vector3.zero;
        }
    }
}
