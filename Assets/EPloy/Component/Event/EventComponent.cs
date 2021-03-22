﻿
using UnityEngine;
using System.Collections.Generic;

namespace EPloy
{
    [System]
    public class EventComponentUpdateSystem : UpdateSystem<EventComponent>
    {
        public override void Update(EventComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 事件组件
    /// </summary>
    /// <typeparam name="T">事件类型。</typeparam>
    public partial class EventComponent : Component
    {
        private readonly UnOrderMultiMap<int, EventHandler<EventArg>> m_EventHandlers;
        /// <summary>
        /// 带发送队列
        /// </summary>
        private readonly Queue<Event> m_Events;

        public EventComponent()
        {
            m_EventHandlers = new UnOrderMultiMap<int, EventHandler<EventArg>>();
            m_Events = new Queue<Event>();
        }

        /// <summary>
        /// 获取事件处理函数的数量。
        /// </summary>
        public int EventHandlerCount
        {
            get
            {
                return m_EventHandlers.Count;
            }
        }

        /// <summary>
        /// 获取事件数量。
        /// </summary>
        public int EventCount
        {
            get
            {
                return m_Events.Count;
            }
        }

        /// <summary>
        /// 事件池轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update()
        {
            while (m_Events.Count > 0)
            {
                Event eventNode = null;
                lock (m_Events)
                {
                    eventNode = m_Events.Dequeue();
                    HandleEvent(eventNode.EventArgs);
                }

                ReferencePool.Release(eventNode);
            }
        }

        /// <summary>
        /// 关闭并清理事件池。
        /// </summary>
        public void OnDestroy()
        {
            Clear();
            m_EventHandlers.Clear();
        }

        /// <summary>
        /// 清理事件。
        /// </summary>
        public override void Clear()
        {
            lock (m_Events)
            {
                m_Events.Clear();
            }
        }

        /// <summary>
        /// 检查是否存在事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        public bool Check(int evtId, EventHandler<EventArg> handler)
        {
            if (handler == null)
            {
                throw new EPloyException("Event handler is invalid.");
            }

            return m_EventHandlers.Contains(evtId, handler);
        }

        /// <summary>
        /// 订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要订阅的事件处理函数。</param>
        public void Subscribe(int evtId,EventHandler<EventArg> handler)
        {
            if (handler == null)
            {
                throw new EPloyException("Event handler is invalid.");
            }
            else if (Check(evtId, handler))
            {
                throw new EPloyException(Utility.Text.Format("Event '{0}' not allow duplicate handler.", evtId.ToString()));
            }
            else
            {
                m_EventHandlers.Add(evtId, handler);
            }
        }

        /// <summary>
        /// 取消订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要取消订阅的事件处理函数。</param>
        public void Unsubscribe(int evtId,EventHandler<EventArg> handler)
        {
            if (handler == null)
            {
                throw new EPloyException("Event handler is invalid.");
            }
            if (!m_EventHandlers.Remove(evtId, handler))
            {
                throw new EPloyException(Utility.Text.Format("Event '{0}' not exists specified handler.", evtId.ToString()));
            }
        }

        /// <summary>
        /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void Fire(EventArg eventArg)
        {
            if (eventArg == null)
            {
                throw new EPloyException("Event is invalid.");
            }

            Event eventNode = Event.Create(eventArg);
            lock (m_Events)
            {
                m_Events.Enqueue(eventNode);
            }
        }

        /// <summary>
        /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void FireNow(EventArg eventArg)
        {
            if (eventArg == null)
            {
                throw new EPloyException("Event is invalid.");
            }

            HandleEvent(eventArg);
        }

        /// <summary>
        /// 处理事件结点。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        private void HandleEvent(EventArg eventArg)
        {
            bool noHandlerException = false;
            List<EventHandler<EventArg>> range = default(List<EventHandler<EventArg>>);
            if (m_EventHandlers.TryGetValue(eventArg.id, out range))
            {
                foreach (var evt in range)
                {
                    evt(eventArg);
                }
            }
            else
            {
                noHandlerException = true;
            }
            ReferencePool.Release(eventArg);

            if (noHandlerException)
            {
                throw new EPloyException(Utility.Text.Format("Event '{0}' not allow no handler.", eventArg.GetType()));
            }
        }
    }
}