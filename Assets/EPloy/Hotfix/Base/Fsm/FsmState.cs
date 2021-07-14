using System;
using System.Collections.Generic;

namespace EPloy.Fsm
{
    /// <summary>
    /// 有限状态机状态基类。
    /// </summary>
    public abstract class FsmState : IReference
    {
        private readonly Dictionary<int, FsmEventHandler> EventHandlers;

        /// <summary>
        /// 状态机持有者
        /// </summary>
        /// <value></value>
        protected IFsm ProcedureOwner { get; private set; }

        /// <summary>
        /// 初始化有限状态机状态基类的新实例。
        /// </summary>
        public FsmState()
        {
            EventHandlers = new Dictionary<int, FsmEventHandler>();
        }

        /// <summary>
        /// 有限状态机状态初始化时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void OnInit(IFsm fsm)
        {
            this.ProcedureOwner = fsm;
        }

        /// <summary>
        /// 有限状态机状态进入时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void OnEnter()
        {

        }

        /// <summary>
        /// 有限状态机状态轮询时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发。</param>
        public virtual void OnLeave(bool isShutdown)
        {

        }

        /// <summary>
        /// 有限状态机状态销毁时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        public virtual void Clear()
        {
            ProcedureOwner = null;
            EventHandlers.Clear();
        }

        /// <summary>
        /// 订阅有限状态机事件。
        /// </summary>
        /// <param name="eventId">事件编号。</param>
        /// <param name="eventHandler">有限状态机事件响应函数。</param>
        protected void SubscribeEvent(int eventId, FsmEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                Log.Error("Event handler is invalid.");
                return;
            }

            if (!EventHandlers.ContainsKey(eventId))
            {
                EventHandlers[eventId] = eventHandler;
            }
            else
            {
                EventHandlers[eventId] += eventHandler;
            }
        }

        /// <summary>
        /// 取消订阅有限状态机事件。
        /// </summary>
        /// <param name="eventId">事件编号。</param>
        /// <param name="eventHandler">有限状态机事件响应函数。</param>
        protected void UnsubscribeEvent(int eventId, FsmEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                Log.Error("Event handler is invalid.");
                return;
            }

            if (EventHandlers.ContainsKey(eventId))
            {
                EventHandlers[eventId] -= eventHandler;
            }
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        /// <param name="fsm">有限状态机引用。</param>
        protected void ChangeState<TState>() where TState : FsmState
        {
            LimitFsm fsmImplement = (LimitFsm)ProcedureOwner;
            if (fsmImplement == null)
            {
                Log.Error("FSM is invalid.");
                return;
            }

            fsmImplement.ChangeState<TState>();
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        protected void ChangeState(Type stateType)
        {
            LimitFsm fsmImplement = (LimitFsm)ProcedureOwner;
            if (fsmImplement == null)
            {
                Log.Error("FSM is invalid.");
                return;
            }

            if (stateType == null)
            {
                Log.Error("State type is invalid.");
                return;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                Log.Error(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));
                return;
            }

            fsmImplement.ChangeState(stateType);
        }

        /// <summary>
        /// 响应有限状态机事件时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        internal void OnEvent(IFsm fsm, object sender, int eventId, object userData)
        {
            FsmEventHandler eventHandlers = null;
            if (EventHandlers.TryGetValue(eventId, out eventHandlers))
            {
                if (eventHandlers != null)
                {
                    eventHandlers(fsm, sender, userData);
                }
            }
        }
    }
}
