
using System;

namespace EPloy.ObjectPool
{
    /// <summary>
    /// 对象池基类。
    /// </summary>
    public abstract class ObjectPoolBase : IReference
    {
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
            get;
            protected set;
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
        /// 自动释放读秒
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
        /// 创建对象。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <param name="spawned">对象是否已被获取。</param>
        public abstract void Register(ObjectBase obj, bool spawned);

        /// <summary>
        /// 检查对象。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <returns>要检查的对象是否存在。</returns>
        public abstract bool CanSpawn(string name);

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <returns>要获取的对象。</returns>
        public abstract ObjectBase Spawn(string name);

        /// <summary>
        /// 回收对象。
        /// </summary>
        /// <param name="target">要回收的对象。</param>
        public abstract void Unspawn(object target);

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
