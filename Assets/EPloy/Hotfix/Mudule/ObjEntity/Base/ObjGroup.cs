using System;
using EPloy.ObjectPool;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Obj
{
    /// <summary>
    /// 实体组。
    /// </summary>
    public sealed class ObjGroup : IReference
    {
        private Transform Parent;
        private ObjectPoolBase InstancePool;
        private TypeLinkedList<ObjBase> ObjEntities;
        private LinkedListNode<ObjBase> CachedNode;

        /// <summary>
        /// 名称
        /// </summary>
        public ObjGroupName GroupName
        {
            get;
            private set;
        }

        /// <summary>
        /// Transform 信息
        /// </summary>
        public GameObject Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// 实体数量
        /// </summary>
        public int ObjCount
        {
            get
            {
                return ObjEntities.Count;
            }
        }

        /// <summary>
        /// 自动释放间隔
        /// </summary>
        public float AutoReleaseTime
        {
            get
            {
                return InstancePool.AutoReleaseTime;
            }
            set
            {
                InstancePool.AutoReleaseTime = value;
            }
        }

        /// <summary>
        ///容量。
        /// </summary>
        public int Capacity
        {
            get
            {
                return InstancePool.Capacity;
            }
            set
            {
                InstancePool.Capacity = value;
            }
        }

        /// <summary>
        ///对象过期秒数。
        /// </summary>
        public float ExpireTime
        {
            get
            {
                return InstancePool.ExpireTime;
            }
            set
            {
                InstancePool.ExpireTime = value;
            }
        }

        /// <summary>
        /// 初始化实体组的新实例。
        /// </summary>
        /// <param name="name">实体组名称。</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数。</param>
        /// <param name="instanceCapacity">实体实例对象池容量。</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数。</param>
        /// <param name="instancePriority">实体实例对象池的优先级。</param>
        /// <param name="entityGroupHelper">实体组辅助器。</param>
        /// <param name="objectPoolManager">对象池管理器。</param>
        public ObjGroup(ObjGroupName groupName, ObjectPoolBase instancePool, Transform parent)
        {
            GroupName = groupName;
            Parent = parent;
            InstancePool = instancePool;
            ObjEntities = new TypeLinkedList<ObjBase>();
            CachedNode = null;
            CreateGroupInstance();
        }

        private void CreateGroupInstance()
        {
            Handle = new GameObject(GroupName.ToString());
            Handle.transform.SetParent(Parent);
            Handle.transform.localScale = Vector3.one;
            Handle.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 实体组轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update()
        {
            LinkedListNode<ObjBase> current = ObjEntities.First;
            while (current != null)
            {
                CachedNode = current.Next;
                current.Value.Update();
                current = CachedNode;
                CachedNode = null;
            }
        }

        /// <summary>
        ///是否存在实体。
        /// </summary>
        /// <param name="serialId">实体序列编号。</param>
        /// <returns>实体组中是否存在实体。</returns>
        public bool HasObj(int serialId)
        {
            foreach (ObjBase entity in ObjEntities)
            {
                if (entity.SerialId == serialId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="serialId">实体序列编号。</param>
        /// <returns>要获取的实体。</returns>
        public ObjBase GetObj(int serialId)
        {
            foreach (ObjBase entity in ObjEntities)
            {
                if (entity.SerialId == serialId)
                {
                    return entity;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有实体。
        /// </summary>
        /// <returns>实体组中的所有实体。</returns>
        public ObjBase[] GetAllObjs()
        {
            List<ObjBase> results = new List<ObjBase>();
            foreach (ObjBase entity in ObjEntities)
            {
                results.Add(entity);
            }
            return results.ToArray();
        }

        public ObjBase ShowObj(bool isNew, object obj, int serialId, Type objEntityType, object userData)
        {
            ObjBase objBase = null;
            if (isNew)
            {
                if (objEntityType == null || objEntityType.IsInstanceOfType(typeof(ObjBase)))
                {
                    Log.Fatal("can not fand ui C# class  uiName : " + objEntityType.ToString());
                    return objBase;
                }
                objBase = (ObjBase)ReferencePool.Acquire(objEntityType);
                ObjEntities.AddLast(objBase);
            }
            else
            {
                objBase = GetObj(serialId);
            }

            if (objBase == null)
            {
                Log.Fatal(Utility.Text.Format("objBase {0} is invalid.", objEntityType.ToString()));
                return objBase;
            }
            GameObject objEntityGo = obj as GameObject;
            objEntityGo.transform.SetParent(Handle.transform);
            objBase.Initialize(isNew, objEntityGo, serialId, this, userData);
            return objBase;
        }

        /// <summary>
        /// 从实体组移除实体。
        /// </summary>
        /// <param name="obj">要移除的实体。</param>
        public void RemoveObjEntity(ObjBase obj)
        {
            if (CachedNode != null && CachedNode.Value == obj)
            {
                CachedNode = CachedNode.Next;
            }

            if (!ObjEntities.Remove(obj))
            {
                Log.Error(Utility.Text.Format("obj group '{0}' not exists specified entity '[{1}]{2}'.", GroupName, obj.SerialId.ToString(), obj.Handle.name));
            }
            ReferencePool.Release(obj);
        }

        public void RegisterObj(ObjInstance obj, bool spawned)
        {
            InstancePool.Register(obj, spawned);
        }

        public ObjInstance SpawnObjInstance(string name)
        {
            return (ObjInstance)InstancePool.Spawn(name);
        }

        public void UnspawnObj(ObjBase entity)
        {
            InstancePool.Unspawn(entity.Handle);
        }

        public void Clear()
        {
            ObjEntities.Clear();
            CachedNode = null;
            GameObject.Destroy(Handle);
            InstancePool = null;
            Handle = null;
        }
    }
}
