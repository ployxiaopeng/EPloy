using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Event
{
    public class TestEvt : EventArg
    {
        public override int id
        {
            get
            {
                return EventId.TestEvt;
            }
        }


        public override void Clear()
        {

        }
    }
}
