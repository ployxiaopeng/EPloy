
using System;
using EPloy.Event;


public partial class EventModule : IGameModule
{
    /// <summary>
    /// 事件处理委托
    /// </summary>
    /// <typeparam name="TEventArg"></typeparam>
    /// <param name="e"></param>
    public delegate void EventHandler<TEventArg>(TEventArg e) where TEventArg : EventArg;

    /// <summary>
    /// 事件结点。
    /// </summary>
    private class Event : IReference
    {
        public Event()
        {
            EventArgs = null;
        }

        public EventArg EventArgs { get; private set; }

        public static Event Create(EventArg eventArg)
        {
            Event eventNode = ReferencePool.Acquire<Event>();
            eventNode.EventArgs = eventArg;
            return eventNode;
        }

        public void Clear()
        {
            EventArgs = null;
        }
    }
}
