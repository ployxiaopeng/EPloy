
using EPloy.Game.Reference;
using System;

namespace EPloy.Game.ObjectPool
{
    /// <summary>
    /// 对象基类。
    /// </summary>
    public abstract class ObjectBase : IReference
    {
        protected DateTime lastUseTime;

        /// <summary>
        /// 对象的获取计数。
        /// </summary>
        public int SpawnCount
        {
            get;
            private set;
        }

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
        /// 获取对象是否正在使用。
        /// </summary>
        public bool IsInUse
        {
            get
            {
                return SpawnCount > 0;
            }
        }

        /// <summary>
        /// 初始化对象基类。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <param name="target">对象。</param>
        protected internal virtual void Initialize(string name, object target)
        {
            SpawnCount = 0;
            Name = name; Target = target;
        }

        /// <summary>
        /// 设置产生数量
        /// </summary>
        /// <param name="spawned"></param>
        protected internal virtual void SetSpawned(bool spawned)
        {
            SpawnCount = 0;
            if (spawned)
            {
                Spawn();
            }
        }

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <returns>对象。</returns>
        protected internal virtual void Spawn()
        {
            SpawnCount++;
            LastUseTime = DateTime.Now;
        }

        /// <summary>
        /// 回收对象。
        /// </summary>
        protected internal virtual void Unspawn()
        {
            LastUseTime = DateTime.Now;
            SpawnCount--;
            if (SpawnCount < 0)
            {
                Log.Fatal(UtilText.Format("Object '{0}' spawn count is less than 0.", Name));
            }
        }

        /// <summary>
        /// 清理对象基类。
        /// </summary>
        public virtual void Clear()
        {
            Name = null;
            Target = null;
            SpawnCount = 0;
            lastUseTime = default(DateTime);
        }

    }
}
