
using System;
using EPloy.Game.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using EPloy.Game.Reference;

namespace EPloy.Game
{
    /// <summary>
    /// 对象池管理器。
    /// </summary>
    public partial class ObjectPoolMudule : IGameModule
    {
        private int Capacity;
        private float ExpireTime;
        private float ReleaseTime;
        private Dictionary<TypeNamePair, ObjectPoolBase> ObjectPools;
        private List<ObjectPoolBase> Pools;

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

        public void Awake()
        {
            Capacity = Config.ObjectPoolCapacity;
            ExpireTime = Config.ObjectPoolExpireTime;
            ReleaseTime = Config.ObjectPoolReleaseTime;
            Pools = new List<ObjectPoolBase>();
            ObjectPools = new Dictionary<TypeNamePair, ObjectPoolBase>();
        }

        public void Update()
        {
            for (int i = 0; i < Pools.Count; i++)
            {
                Pools[i].AutoReleaseTime -= Time.deltaTime;
                if (Pools[i].AutoReleaseTime > 0)
                {
                    return;
                }
                Pools[i].Release();
                Pools[i].AutoReleaseTime = ReleaseTime;
            }
        }

        public void OnDestroy()
        {
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in ObjectPools)
            {
                ReferencePool.Release(objectPool.Value);
            }
            ObjectPools.Clear();
            Pools.Clear();
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
                Log.Fatal(UtilText.Format("Object type '{0}' is invalid.", objectType.FullName));
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
                Log.Fatal(UtilText.Format("Object type '{0}' is invalid.", objectType.FullName));
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
                Log.Fatal(UtilText.Format("Object type '{0}' is invalid.", objectType.FullName));
                return null;
            }

            TypeNamePair typeNamePair = new TypeNamePair(objectType, name);
            if (HasObjectPool(objectType, name))
            {
                Log.Fatal(UtilText.Format("Already exist object pool '{0}'.", typeNamePair.ToString()));
                return null;
            }

            EPloy.Game.ObjectPool.ObjectPool objectPool = ReferencePool.Acquire<EPloy.Game.ObjectPool.ObjectPool>();
            objectPool.Initialize(objectType, name, ReleaseTime, Capacity, ExpireTime);
            ObjectPools.Add(typeNamePair, objectPool);
            Pools.Add(objectPool);
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
                Log.Fatal(UtilText.Format("Object type '{0}' is invalid.", objectType.FullName));
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
                Pools.Remove(objectPool);
                return ObjectPools.Remove(typeNamePair);
            }
            return false;
        }
    }
}
