using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    public class SwitchSceneEvt : EventArg
    {
        public override int id
        {
            get
            {
                return EventId.SwitchSceneEvt;
            }
        }

        public string SwitchSceneName
        {
            get;
            private set;
        }

        public void SetData(string switchSceneName)
        {
            SwitchSceneName = switchSceneName;
        }

        public override void Clear()
        {
            SwitchSceneName = null;
        }
    }
}
