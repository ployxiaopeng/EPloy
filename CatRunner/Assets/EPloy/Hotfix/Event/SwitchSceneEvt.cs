using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Event
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

        public static SwitchSceneEvt Create(string switchSceneName)
        {
            SwitchSceneEvt switchScene = ReferencePool.Acquire<SwitchSceneEvt>();
            switchScene.SwitchSceneName = switchSceneName;
            return switchScene;
        }

        public override void Clear()
        {
            SwitchSceneName = null;
        }
    }
}
