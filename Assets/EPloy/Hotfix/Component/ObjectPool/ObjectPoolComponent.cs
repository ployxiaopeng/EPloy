
using System;
using EPloy.ObjectPool;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 对象池管理器。
    /// </summary>
    public partial class ObjectPoolComponent : Component
    {
        private int Capacity;
        private float ExpireTime;
        private float ReleaseTime;
        private Dictionary<TypeNamePair, ObjectPoolBase> ObjectPools;

        /// <summary>
        /// 获取对象池数量。
        /// </summary>
        public int Count
        {
            get
            {
                return ObjectPools.Count;
            }
        }

        public override void Awake()
        {
            Capacity = Config.ObjectPoolCapacity;
            ExpireTime = Config.ObjectPoolExpireTime;
            ReleaseTime = Config.ObjectPoolReleaseTime;
            ObjectPools = new Dictionary<TypeNamePair, ObjectPoolBase>();
        }

        public override void Update()
        {
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in ObjectPools)
            {
                objectPool.Value.AutoReleaseTime -= Time.deltaTime;
                if (objectPool.Value.AutoReleaseTime > 0)
                {
                    return;
                }
                objectPool.Value.Release();
                objectPool.Value.AutoReleaseTime = ReleaseTime;
            }
        }

        public override void OnDestroy()
        {
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in ObjectPools)
            {
                ReferencePool.Release(objectPool.Value);
            }
            ObjectPools.Clear();
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
                Log.Fatal("Object type is invalid.");
                return false;
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                Log.Fatal(Utility.Text.Format("Object type '{0}' is invalid.", objectType.FullName));
                return false;
            }

            return ObjectPools.ContainsKey(new TypeNamePair(objectType, name));
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
                Log.Fatal("Object type is invalid.");
                return null;
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                Log.Fatal(Utility.Text.Format("Object type '{0}' is invalid.", objectType.FullName));
                return null;
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
        public ObjectPoolBase CreateObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                Log.Fatal("Object type is invalid.");
                return null;
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                Log.Fatal(Utility.Text.Format("Object type '{0}' is invalid.", objectType.FullName));
                return null;
            }

            TypeNamePair typeNamePair = new TypeNamePair(objectType, name);
            if (HasObjectPool(objectType, name))
            {
                Log.Fatal(Utility.Text.Format("Already exist object pool '{0}'.", typeNamePair.ToString()));
                return null;
            }

            EPloy.ObjectPool.ObjectPool objectPool = ReferencePool.Acquire<EPloy.ObjectPool.ObjectPool>();
            objectPool.Initialize(objectType, name, ReleaseTime, Capacity, ExpireTime);
            ObjectPools.Add(typeNamePair, objectPool);
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
                Log.Fatal("Object type is invalid.");
                return false;
            }

            if (!typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                Log.Fatal(Utility.Text.Format("Object type '{0}' is invalid.", objectType.FullName));
                return false;
            }

            return DestroyObjectPool(new TypeNamePair(objectType, name));
        }



        private ObjectPoolBase GetObjectPool(TypeNamePair typeNamePair)
        {
            ObjectPoolBase objectPool = null;
            if (ObjectPools.TryGetValue(typeNamePair, out objectPool))
            {
                return objectPool;
            }
            return null;
        }

        private bool DestroyObjectPool(TypeNamePair typeNamePair)
        {
            ObjectPoolBase objectPool = null;
            if (ObjectPools.TryGetValue(typeNamePair, out objectPool))
            {
                ReferencePool.Release(objectPool);
                return ObjectPools.Remove(typeNamePair);
            }

            return false;
        }
    }
}
