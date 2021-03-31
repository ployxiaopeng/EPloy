using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public enum UIFunType
    {
        Null,
        OnInit,
        OnOpen,
        OnClose
    }
    public class HotfixUIEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(HotfixUIEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public UIFormLogic uIFormLogic { get; private set; }
        public UIFunType uIFunType { get; private set; }
        public void SetEventData(UIFunType funType, UIFormLogic formLogic)
        {
            uIFunType = funType; uIFormLogic = formLogic;
        }

        public override void Clear()
        {
            uIFunType = UIFunType.Null;
            uIFormLogic = null;
        }
    }
}