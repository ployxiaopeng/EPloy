
using System;

namespace EPloy.ObjectPool
{
    /// <summary>
    /// 对象基类。
    /// </summary>
    public abstract class ObjectBase : IReference
    {
        protected DateTime lastUseTime;

        /// <summary>
        /// 获取对象名称。
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// 获取对象。
        /// </summary>
        public object Target
        {
            get;
            protected set;
        }

        /// <summary>
        /// 获取对象上次使用时间。
        /// </summary>
        public DateTime LastUseTime
        {
            get
            {
                return lastUseTime;
            }
            internal set
            {
                lastUseTime = value;
            }
        }

        /// <summary>
        /// 初始化对象基类。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <param name="target">对象。</param>
        public void Initialize(string name, object target)
        {
            Name = name; Target = target;
        }

        /// <summary>
        /// 获取对象时的事件。
        /// </summary>
        protected internal virtual void OnSpawn()
        {
        }

        /// <summary>
        /// 回收对象时的事件。
        /// </summary>
        protected internal virtual void OnUnspawn()
        {
        }

        /// <summary>
        /// 清理对象基类。
        /// </summary>
        public virtual void Clear()
        {
            Name = null;
            Target = null;
            lastUseTime = default(DateTime);
        }
    }
}
