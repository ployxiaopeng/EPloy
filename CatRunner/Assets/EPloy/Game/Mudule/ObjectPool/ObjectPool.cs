using EPloy.Game.Reference;
using System;
using System.Collections.Generic;

namespace EPloy.Game.ObjectPool
{
    /// <summary>
    /// 默认对象池的实现
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    internal class ObjectPool : ObjectPoolBase
    {
        private UnOrderMultiMap<string, ObjectBase> Objects;
        private Dictionary<object, ObjectBase> ObjectMap;
        private List<ObjectBase> CanReleaseObjects;//未使用能释放的对象
        private List<ObjectBase> ToReleaseObjects;//确定能释放的对象

        private Type objectType;
        private int capacity;
        private float expireTime;
        private float autoReleaseTime;


        /// <summary>
        /// 对象类型。
        /// </summary>
        public override Type ObjectType
        {
            get
            {
                return objectType;
            }
        }

        /// <summary>
        /// 对象的数量。
        /// </summary>
        public override int Count
        {
            get
            {
                return ObjectMap.Count;
            }
        }

        /// <summary>
        /// 对象池的容量。
        /// </summary>
        public override int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                if (value < 0)
                {
                    Log.Fatal("Capacity is invalid.");
                    return;
                }

                if (capacity == value)
                {
                    return;
                }

                capacity = value;
                Release();
            }
        }

        /// <summary>
        /// 自动释放间隔
        /// </summary>
        public override float AutoReleaseTime
        {
            get
            {
                return autoReleaseTime;
            }

            set
            {
                if (autoReleaseTime == value)
                {
                    return;
                }

                autoReleaseTime = value;
                if (autoReleaseTime == 0)
                {
                    Release();
                }
            }
        }

        /// <summary>
        /// 对象过期秒数。
        /// </summary>
        public override float ExpireTime
        {
            get
            {
                return expireTime;
            }

            set
            {
                if (value < 0f)
                {
                    Log.Fatal("ExpireTime is invalid.");
                    return;
                }

                if (expireTime == value)
                {
                    return;
                }

                expireTime = value;
                Release();
            }
        }

        /// <summary>
        /// 设置对象池基本数据
        /// </summary>
        public override void Initialize(Type objectType, string name, float autoReleaseTime, int capacity, float expireTime)
        {
            Objects = new UnOrderMultiMap<string, ObjectBase>();
            ObjectMap = new Dictionary<object, ObjectBase>();
            CanReleaseObjects = new List<ObjectBase>();
            ToReleaseObjects = new List<ObjectBase>();
            this.objectType = objectType;
            Name = name;
            this.autoReleaseTime = autoReleaseTime;
            this.capacity = capacity;
            this.expireTime = expireTime;
        }

        /// <summary>
        /// 创建对象。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <param name="spawned">对象是否已被获取。</param>
        public override void Register(ObjectBase obj, bool spawned)
        {
            if (obj == null)
            {
                Log.Fatal("Object is invalid.");
                return;
            }
            obj.SetSpawned(spawned);
            Objects.Add(obj.Name, obj);
            ObjectMap.Add(obj.Target, obj);

            if (Count > Capacity)
            {
                Release();
            }
        }

        /// <summary>
        /// 检查对象。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <returns>要检查的对象是否存在。</returns>
        public override bool CanSpawn(string name)
        {
            TypeLinkedList<ObjectBase> objectRange = default(TypeLinkedList<ObjectBase>);
            if (Objects.TryGetValue(name, out objectRange))
            {
                foreach (ObjectBase internalObject in objectRange)
                {
                    if (internalObject.IsInUse)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <returns>要获取的对象。</returns>
        public override ObjectBase Spawn(string name)
        {
            TypeLinkedList<ObjectBase> objectRange = default(TypeLinkedList<ObjectBase>);
            if (Objects.TryGetValue(name, out objectRange))
            {
                foreach (ObjectBase internalObject in objectRange)
                {
                    if (!internalObject.IsInUse)
                    {
                        internalObject.Spawn();
                        return internalObject;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 回收对象。
        /// </summary>
        /// <param name="target">要回收的对象。</param>
        public override void Unspawn(object target)
        {
            if (target == null)
            {
                Log.Fatal("Target is invalid.");
                return;
            }

            ObjectBase internalObject = GetObject(target);
            if (internalObject != null)
            {
                internalObject.Unspawn();
                if (Count > Capacity && internalObject.SpawnCount <= 0)
                {
                    Release();
                }
            }
            else
            {
                Log.Fatal(UtilText.Format("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.",
                new TypeNamePair(target.GetType(), Name).ToString(), target.GetType().FullName, target.ToString()));
            }
        }

        /// <summary>
        /// 释放可释放对象。
        /// </summary>
        public override void Release()
        {
            DateTime expireTime = DateTime.MinValue;
            if (ExpireTime < float.MaxValue)
            {
                expireTime = DateTime.Now.AddSeconds(-ExpireTime);
            }

            AutoReleaseTime = 0f;
            GetCanReleaseObjects(CanReleaseObjects);
            List<ObjectBase> toReleaseObjects = ConfirmReleaseObjectFiltrate(CanReleaseObjects, expireTime);
            if (toReleaseObjects == null || toReleaseObjects.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < toReleaseObjects.Count; i++)
            {
                ReleaseObject(toReleaseObjects[i]);
            }
        }

        /// <summary>
        /// 释放所有未使用对象。
        /// </summary>
        public override void ReleaseAllUnused()
        {
            AutoReleaseTime = 0f;
            GetCanReleaseObjects(CanReleaseObjects);
            for (int i = 0; i < CanReleaseObjects.Count; i++)
            {
                ReleaseObject(CanReleaseObjects[i]);
            }
        }

        /// <summary>
        /// 释放对象。
        /// </summary>
        /// <param name="target">要释放的对象。</param>
        /// <returns>释放对象是否成功。</returns>
        public bool ReleaseObject(object target)
        {
            if (target == null)
            {
                Log.Fatal("Target is invalid.");
                return false;
            }

            ObjectBase internalObject = GetObject(target);
            if (internalObject == null)
            {
                Log.Fatal("Can not release object which is not found.");
                return false;
            }

            if (internalObject.IsInUse)
            {
                return false;
            }

            Objects.Remove(internalObject.Name, internalObject);
            ObjectMap.Remove(internalObject.Target);
            ReferencePool.Release(internalObject);
            return true;
        }

        /// <summary>
        /// 获取所有对象信息。
        /// </summary>
        /// <returns>所有对象信息。</returns>
        public override ObjectInfo[] GetAllObjectInfos()
        {
            TypeLinkedList<ObjectInfo> results = new TypeLinkedList<ObjectInfo>();
            foreach (KeyValuePair<string, TypeLinkedList<ObjectBase>> objectRanges in Objects)
            {
                foreach (ObjectBase internalObject in objectRanges.Value)
                {
                    results.Add(new ObjectInfo(internalObject.Name, internalObject.LastUseTime, internalObject.SpawnCount));
                }
            }
            ObjectInfo[] ObjectInfo = new ObjectInfo[results.Count];
            results.CopyTo(ObjectInfo, 0);
            return ObjectInfo;
        }

        private ObjectBase GetObject(object target)
        {
            if (target == null)
            {
                Log.Fatal("Target is invalid.");
                return null;
            }

            ObjectBase internalObject = null;
            if (ObjectMap.TryGetValue(target, out internalObject))
            {
                return internalObject;
            }
            return null;
        }

        private void GetCanReleaseObjects(List<ObjectBase> results)
        {
            if (results == null)
            {
                Log.Fatal("Results is invalid.");
                return;
            }

            results.Clear();
            foreach (KeyValuePair<object, ObjectBase> objectInMap in ObjectMap)
            {
                ObjectBase internalObject = objectInMap.Value;
                if (internalObject.IsInUse)
                {
                    continue;
                }
                results.Add(internalObject);
            }
        }

        private List<ObjectBase> ConfirmReleaseObjectFiltrate(List<ObjectBase> canReleaseObjects, DateTime expireTime)
        {
            ToReleaseObjects.Clear();

            if (expireTime > DateTime.MinValue)
            {
                for (int i = canReleaseObjects.Count - 1; i >= 0; i--)
                {
                    if (canReleaseObjects[i].LastUseTime <= expireTime)
                    {
                        ToReleaseObjects.Add(canReleaseObjects[i]);
                        canReleaseObjects.RemoveAt(i);
                        continue;
                    }
                }
            }
            return ToReleaseObjects;
        }

        public override void Clear()
        {
            foreach (KeyValuePair<object, ObjectBase> objectInMap in ObjectMap)
            {
                ReferencePool.Release(objectInMap.Value);
            }
            Objects.Clear();
            ObjectMap.Clear();
            CanReleaseObjects.Clear();
            ToReleaseObjects.Clear();
        }

    }
}
