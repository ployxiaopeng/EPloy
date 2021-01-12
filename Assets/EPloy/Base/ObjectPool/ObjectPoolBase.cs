
using System;

namespace EPloy.ObjectPool
{
    /// <summary>
    /// 对象池基类。
    /// </summary>
    public abstract class ObjectPoolBase:IReference
    {
        protected  string m_Name;

        /// <summary>
        /// 设置对象池基本数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="allowMultiSpawn"></param>
        /// <param name="autoReleaseInterval"></param>
        /// <param name="capacity"></param>
        /// <param name="expireTime"></param>
        /// <param name=""></param>
        public abstract void Initialize(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime);

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 对象类型。
        /// </summary>
        public abstract Type ObjectType
        {
            get;
        }

        /// <summary>
        /// 对象数量。
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// 对象池的容量。
        /// </summary>
        public abstract int Capacity
        {
            get;
            set;
        }

        /// <summary>
        /// 自动释放间隔
        /// </summary>
        public abstract float AutoReleaseTime
        {
            get;
            set;
        }

        /// <summary>
        /// 对象过期秒数。
        /// </summary>
        public abstract float ExpireTime
        {
            get;
            set;
        }

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 释放对象池中的所有未使用对象。
        /// </summary>
        public abstract void ReleaseAllUnused();

        /// <summary>
        /// 获取所有对象信息。
        /// </summary>
        /// <returns>所有对象信息。</returns>
        public abstract ObjectInfo[] GetAllObjectInfos();

        public abstract void Clear();
    }
}
