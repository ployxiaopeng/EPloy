using System;

namespace EPloy.Fsm
{
    /// <summary>
    /// 有限状态机基类。
    /// </summary>
    public abstract class FsmBase : IReference
    {
        public string Name { get; private set; }

        /// <summary>
        /// 设置状态机名字
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        protected void SetName(string name)
        {
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// 获取有限状态机持有者类型。
        /// </summary>
        public abstract Type OwnerType
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public abstract int FsmStateCount
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public abstract bool IsRunning
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public abstract bool IsDestroyed
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public abstract string CurrentStateName
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public abstract float CurrentStateTime
        {
            get;
        }

        public abstract void Update();

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        public abstract void Clear();
    }
}