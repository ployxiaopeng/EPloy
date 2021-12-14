using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Fsm
{
    /// <summary>
    /// 有限状态机事件响应函数。
    /// </summary>
    /// <param name="fsm">有限状态机引用。</param>
    /// <param name="sender">事件源。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void FsmEventHandler(IFsm fsm, object sender, object userData);
    /// <summary>
    /// 有限状态机。
    /// </summary>
    public sealed class LimitFsm : FsmBase, IFsm
    {
        private object _Owner;
        private Dictionary<string, FsmState> States;
        private Dictionary<string, Variable> Datas;
        private FsmState _CurrentState;
        private float _CurrentStateTime;
        private bool _IsDestroyed;

        /// <summary>
        /// 初始化有限状态机
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        public void InitLimitFsm(string name, object owner, params FsmState[] states)
        {
            if (owner == null || states == null || states.Length < 1)
            {
                Log.Error("FSM owner or states is invalid.");
                return;
            }

            SetName(name);
            _Owner = owner;
            States = new Dictionary<string, FsmState>();
            Datas = new Dictionary<string, Variable>();

            for (int i = 0; i < states.Length; i++)
            {
                FsmState state = states[i];
                if (state == null)
                {
                    Log.Error("FSM states is invalid.");
                    return;
                }

                string stateName = state.GetType().FullName;
                if (States.ContainsKey(stateName))
                {
                    Log.Error(Utility.Text.Format("FSM '{0}' : '{1}' state '{2}' is already exist.", OwnerType, name,
                        stateName));
                    return;
                }

                States.Add(stateName, state);
                state.OnInit(this);
            }

            _CurrentStateTime = 0f;
            _CurrentState = null;
            _IsDestroyed = false;
        }

        /// <summary>
        /// 获取有限状态机持有者。
        /// </summary>
        public object Owner
        {
            get
            {
                return _Owner;
            }
        }

        /// <summary>
        /// 获取有限状态机持有者类型。
        /// </summary>
        public override Type OwnerType
        {
            get
            {
                return _Owner.GetType();
            }
        }

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public override int FsmStateCount
        {
            get
            {
                return States.Count;
            }
        }

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return _CurrentState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public override bool IsDestroyed
        {
            get
            {
                return _IsDestroyed;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public FsmState CurrentState
        {
            get
            {
                return _CurrentState;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public override string CurrentStateName
        {
            get
            {
                return _CurrentState != null ? _CurrentState.GetType().FullName : null;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public override float CurrentStateTime
        {
            get
            {
                return _CurrentStateTime;
            }
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : FsmState
        {
            if (IsRunning)
            {
                Log.Error("FSM is running, can not start again.");
                return;
            }

            FsmState state = GetState<TState>();
            if (state == null)
            {
                Log.Error(Utility.Text.Format("FSM '{0}': '{1}' can not start state '{2}' which is not exist.", OwnerType, Name, typeof(TState).FullName));
                return;
            }

            _CurrentStateTime = 0f;
            _CurrentState = state;
            _CurrentState.OnEnter();
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型。</param>
        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                Log.Error("FSM is running, can not start again.");
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

            FsmState state = GetState(stateType);
            if (state == null)
            {
                Log.Error(Utility.Text.Format("FSM '{0}' :'{1}' can not start state '{2}' which is not exist.", OwnerType, Name, stateType.FullName));
                return;
            }

            _CurrentStateTime = 0f;
            _CurrentState = state;
            _CurrentState.OnEnter();
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : FsmState
        {
            return States.ContainsKey(typeof(TState).FullName);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型。</param>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                Log.Error("State type is invalid.");
                return false;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                Log.Error(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));
                return false;
            }

            return States.ContainsKey(stateType.FullName);
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : FsmState
        {
            FsmState state = null;
            if (States.TryGetValue(typeof(TState).FullName, out state))
            {
                return (TState)state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型。</param>
        /// <returns>要获取的有限状态机状态。</returns>
        public FsmState GetState(Type stateType)
        {
            if (stateType == null)
            {
                Log.Error("State type is invalid.");
                return null;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                Log.Error(Utility.Text.Format("State type '{0}' is invalid.", stateType.FullName));
                return null;
            }

            FsmState state = null;
            if (States.TryGetValue(stateType.FullName, out state))
            {
                return state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <returns>有限状态机的所有状态。</returns>
        public FsmState[] GetAllStates()
        {
            int index = 0;
            FsmState[] results = new FsmState[States.Count];
            foreach (KeyValuePair<string, FsmState> state in States)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <param name="results">有限状态机的所有状态。</param>
        public void GetAllStates(List<FsmState> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            foreach (KeyValuePair<string, FsmState> state in States)
            {
                results.Add(state.Value);
            }
        }

        /// <summary>
        /// 抛出有限状态机事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        public void FireEvent(object sender, int eventId)
        {
            if (_CurrentState == null)
            {
                Log.Error("Current state is invalid.");
                return;
            }

            _CurrentState.OnEvent(this, sender, eventId, null);
        }

        /// <summary>
        /// 抛出有限状态机事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void FireEvent(object sender, int eventId, object userData)
        {
            if (_CurrentState == null)
            {
                Log.Error("Current state is invalid.");
                return;
            }

            _CurrentState.OnEvent(this, sender, eventId, userData);
        }

        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Data name is invalid.");
                return false;
            }

            return Datas.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要获取的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public TData GetData<TData>(string name) where TData : Variable
        {
            return (TData)GetData(name);
        }

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        public Variable GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Data name is invalid.");
                return null;
            }

            Variable data = null;
            if (Datas.TryGetValue(name, out data))
            {
                return data;
            }

            return null;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <typeparam name="TData">要设置的有限状态机数据的类型。</typeparam>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData<TData>(string name, TData data) where TData : Variable
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Data name is invalid.");
                return;
            }

            Datas[name] = data;
        }

        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        public void SetData(string name, Variable data)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Data name is invalid.");
                return;
            }

            Datas[name] = data;
        }

        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Data name is invalid.");
                return false;
            }
            return Datas.Remove(name);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        public override void Update()
        {
            if (_CurrentState == null)
            {
                return;
            }
            _CurrentStateTime += Time.deltaTime;
            _CurrentState.OnUpdate();
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        public void ChangeState<TState>() where TState : FsmState
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        public void ChangeState(Type stateType)
        {
            if (_CurrentState == null)
            {
                Log.Error("Current state is invalid.");
                return;
            }

            FsmState state = GetState(stateType);
            if (state == null)
            {
                Log.Error(Utility.Text.Format("FSM '{0}': '{0}' can not change state to '{1}' which is not exist.", OwnerType, Name, stateType.FullName));
                return;
            }
            _CurrentState.OnLeave(false);
            _CurrentStateTime = 0f;
            _CurrentState = state;
            _CurrentState.OnEnter();
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        public override void Clear()
        {
            if (_CurrentState != null)
            {
                _CurrentState.OnLeave(true);
                _CurrentState = null;
                _CurrentStateTime = 0f;
            }

            foreach (KeyValuePair<string, FsmState> state in States)
            {
                ReferencePool.Release(state.Value);
            }

            States.Clear();
            Datas.Clear();

            _IsDestroyed = true;
        }
    }
}
