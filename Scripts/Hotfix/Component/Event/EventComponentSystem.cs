using ETModel;
using GameFramework;
using System;

namespace ETHotfix
{
    public static class EventComponentSystem
    {

        private static HotfixEventComponet _event = null;
        private static HotfixEventComponet Event
        {
            get
            {
                if (_event == null) _event = GameEntry.Extension.GetComponent<HotfixEventComponet>();
                return Event;
            }
        }

        /// <summary>
        /// 订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要订阅的事件处理函数。</param>
        public static void HotfixSubscribe(this EventComponent self, int id, EventHandler<HotfixEventArgs> handler)
        {
            Event.Subscribe(id, handler);
        }

        /// <summary>
        /// 取消订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要取消订阅的事件处理函数。</param>
        public static void HotfixUnsubscribe(this EventComponent self, int id, EventHandler<HotfixEventArgs> handler)
        {
            Event.Unsubscribe(id, handler);
        }

        /// <summary>
        /// 设置默认事件处理函数。
        /// </summary>
        /// <param name="handler">要设置的默认事件处理函数。</param>
        public static void HotfixSetDefaultHandler(this EventComponent self, EventHandler<HotfixEventArgs> handler)
        {
            Event.SetDefaultHandler(handler);
        }

        /// <summary>
        /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public static void HotfixFire(this EventComponent self, object sender, HotfixEventArgs e)
        {
            Event.Fire(sender, e);
        }

        /// <summary>
        /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public static void HotfixFireNow(this EventComponent self, object sender, HotfixEventArgs e)
        {
            Event.FireNow(sender, e);
        }
    }
}