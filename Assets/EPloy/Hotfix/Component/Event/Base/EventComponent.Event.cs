
using System;

namespace EPloy
{
    public partial class EventComponent : Component
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
        private  class Event : IReference
        {
            private EventArg m_EventArgs;

            public Event()
            {
                m_EventArgs = null;
            }

            public EventArg EventArgs
            {
                get
                {
                    return m_EventArgs;
                }
            }

            public static Event Create(EventArg eventArg)
            {
                Event eventNode = ReferencePool.Acquire<Event>();
                eventNode.m_EventArgs = eventArg;
                return eventNode;
            }

            public void Clear()
            {
                m_EventArgs = null;
            }
        }
    }
}
