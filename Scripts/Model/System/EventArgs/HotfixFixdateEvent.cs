using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{

    public class HotfixFixUpdateEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(HotfixFixUpdateEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {

        }
    }
}