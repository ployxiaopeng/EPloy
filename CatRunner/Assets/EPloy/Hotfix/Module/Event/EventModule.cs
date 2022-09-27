﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EPloy.Event;


/// <summary>
/// 事件模块
/// </summary>
public partial class EventModule : IGameModule
{
    private UnOrderMultiMap<int, EventHandler<EventArg>> EventHandlers;
    private Queue<Event> Events;

    /// <summary>
    /// 获取事件处理函数的数量。
    /// </summary>
    public int EventHandlerCount
    {
        get { return EventHandlers.Count; }
    }

    /// <summary>
    /// 获取事件数量。
    /// </summary>
    public int EventCount
    {
        get { return Events.Count; }
    }

    public void Awake()
    {
        EventHandlers = new UnOrderMultiMap<int, EventHandler<EventArg>>();
        Events = new Queue<Event>();
    }

    public void Update()
    {
        while (Events.Count > 0)
        {
            Event eventNode = null;
            lock (Events)
            {
                eventNode = Events.Dequeue();
                HandleEvent(eventNode.EventArgs);
            }

            ReferencePool.Release(eventNode);
        }
    }

    public void OnDestroy()
    {
        EventHandlers.Clear();
        lock (Events)
        {
            Events.Clear();
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
            Log.Fatal("Event handler is invalid.");
            return false;
        }

        return EventHandlers.Contains(evtId, handler);
    }

    /// <summary>
    /// 订阅事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要订阅的事件处理函数。</param>
    public void Subscribe(int evtId, EventHandler<EventArg> handler)
    {
        if (handler == null)
        {
            Log.Fatal("Event handler is invalid.");
            return;
        }
        else if (Check(evtId, handler))
        {
            Log.Fatal(UtilText.Format("Event '{0}' not allow duplicate handler.", evtId.ToString()));
            return;
        }
        else
        {
            EventHandlers.Add(evtId, handler);
        }
    }

    /// <summary>
    /// 取消订阅事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要取消订阅的事件处理函数。</param>
    public void Unsubscribe(int evtId, EventHandler<EventArg> handler)
    {
        if (handler == null)
        {
            Log.Fatal("Event handler is invalid.");
            return;
        }

        if (!EventHandlers.Remove(evtId, handler))
        {
            Log.Warning(UtilText.Format("Event '{0}' not exists specified handler.", evtId.ToString()));
            return;
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
            Log.Fatal("Event is invalid.");
            return;
        }

        Event eventNode = Event.Create(eventArg);
        lock (Events)
        {
            Events.Enqueue(eventNode);
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
            Log.Fatal("Event is invalid.");
            return;
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
        TypeLinkedList<EventHandler<EventArg>> range = default(TypeLinkedList<EventHandler<EventArg>>);
        if (EventHandlers.TryGetValue(eventArg.id, out range))
        {
            try
            {
                LinkedListNode<EventHandler<EventArg>> evt = range.First;
                while (evt != null)
                {
                    evt.Value(eventArg);
                    evt = evt.Next;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(UtilText.Format("事件 id：{0} err: {1}", eventArg.id, e.ToString()));
            }
        }
        else
        {
            noHandlerException = true;
        }

        ReferencePool.Release(eventArg);

        if (noHandlerException)
        {
            Log.Warning(UtilText.Format("Event '{0}' not allow no handler.", eventArg.GetType()));
        }
    }
}