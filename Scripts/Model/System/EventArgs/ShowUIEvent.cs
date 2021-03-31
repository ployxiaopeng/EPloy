using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public class ShowUIEvent<T> : GameEventArgs
    {
        public static readonly int EventId = typeof(ShowUIEvent<T>).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public T uiWnd { get; private set; }
        public void SetEventData(T _uiWnd)
        {
            uiWnd = _uiWnd;
        }

        public override void Clear()
        {
            uiWnd = default(T);
        }
    }
}