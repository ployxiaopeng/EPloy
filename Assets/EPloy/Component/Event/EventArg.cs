
using System;

namespace EPloy
{
    /// <summary>
    /// 游戏逻辑事件基类。
    /// </summary>
    public abstract class EventArg : EventArgs, IReference
    {
        /// <summary>
        /// 事件id
        /// </summary>
        /// <value></value>
        public abstract int id { get; }
        public abstract void Clear();
    }
}
