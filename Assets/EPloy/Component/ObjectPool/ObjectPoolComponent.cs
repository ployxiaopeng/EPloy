
using System;
using EPloy.ObjectPool;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    [System]
    public class ObjectPoolComponentUpdateSystem : UpdateSystem<ObjectPoolComponent>
    {
        public override void Update(ObjectPoolComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 对象池管理器。
    /// </summary>
    public partial class ObjectPoolComponent : Component
    {
        private int Capacity;
        private float ExpireTime;
        private float ReleaseTime;
        private Dictionary<TypeNamePair, ObjectPoolBase> m_ObjectPools;

        /// <summary>
        /// 获取对象池数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_ObjectPools.Count;
            }
        }

        protected override void Init()
        {
            Capacity = Config.ObjectPoolCapacity;
            ExpireTime = Config.ObjectPoolExpireTime;
            ReleaseTime = Config.ObjectPoolReleaseTime;
            m_ObjectPools = new Dictionary<TypeNamePair, ObjectPoolBase>();
        }

        public void Update()
        {
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in m_ObjectPools)
            {
                objectPool.Value.AutoReleaseTime -= Time.deltaTime;
                if (objectPool.Value.AutoReleaseTime < objectPool.Value.AutoReleaseTime)
                {
                    return;
                }
                objectPool.Value.Release();
                objectPool.Value.AutoReleaseTime = ReleaseTime;
            }
        }

        /// <summary>
        /// 检查是否存在对象池。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        /// <returns>是否存在对象池。</returns>
        public bool HasObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                throw new EPloyException("Object type is invalid.");
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new EPloyException(string.Format("Object type '{0}' is invalid.", objectType.FullName));
            }

            return m_ObjectPools.ContainsKey(new TypeNamePair(objectType, name));
        }

        /// <summary>
        /// 获取对象池。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        /// <param name="name">对象池名称。</param>
        /// <returns>要获取的对象池。</returns>
        public ObjectPoolBase GetObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                throw new EPloyException("Object type is invalid.");
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new EPloyException(string.Format("Object type '{0}' is invalid.", objectType.FullName));
            }

            return GetObjectPool(new TypeNamePair(objectType, name));
        }

        /// <summary>
        /// 获取所有对象池。
        /// </summary>
        /// <returns>所有对象池。</returns>
        public ObjectPoolBase[] GetAllObjectPools()
        {
            return GetAllObjectPools();
        }

        /// <summary>
        /// 创建对象池。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        private ObjectPoolBase CreateObjectPool(Type objectType, string name, float autoRelease, int capacity, float expireTime)
        {
            if (objectType == null)
            {
                throw new EPloyException("Object type is invalid.");
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new EPloyException(string.Format("Object type '{0}' is invalid.", objectType.FullName));
            }

            TypeNamePair typeNamePair = new TypeNamePair(objectType, name);
            if (HasObjectPool(objectType, name))
            {
                throw new EPloyException(string.Format("Already exist object pool '{0}'.", typeNamePair.ToString()));
            }

            ObjectPool objectPool = ReferencePool.Acquire<ObjectPool>();
            objectPool.Initialize(objectType, name, autoRelease, capacity, expireTime);
            m_ObjectPools.Add(typeNamePair, objectPool);
            return objectPool;
        }

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        public void Release()
        {
            ObjectPoolBase[] ObjectPools = GetAllObjectPools();
            foreach (ObjectPoolBase objectPool in ObjectPools)
            {
                objectPool.Release();
            }
        }

        /// <summary>
        /// 销毁对象池。
        /// </summary>
        /// <param name="objectType">对象类型。</param>
        /// <returns>是否销毁对象池成功。</returns>
        public bool DestroyObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                throw new EPloyException("Object type is invalid.");
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new EPloyException(string.Format("Object type '{0}' is invalid.", objectType.FullName));
            }

            return DestroyObjectPool(new TypeNamePair(objectType, name));
        }

        /// <summary>
        /// 关闭并清理对象池管理器。
        /// </summary>
        public void OnDestroy()
        {
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in m_ObjectPools)
            {
                ReferencePool.Release(objectPool.Value);
            }
            m_ObjectPools.Clear();
        }

        private ObjectPoolBase GetObjectPool(TypeNamePair typeNamePair)
        {
            ObjectPoolBase objectPool = null;
            if (m_ObjectPools.TryGetValue(typeNamePair, out objectPool))
            {
                return objectPool;
            }
            return null;
        }

        private bool DestroyObjectPool(TypeNamePair typeNamePair)
        {
            ObjectPoolBase objectPool = null;
            if (m_ObjectPools.TryGetValue(typeNamePair, out objectPool))
            {
                ReferencePool.Release(objectPool);
                return m_ObjectPools.Remove(typeNamePair);
            }

            return false;
        }
    }
}
