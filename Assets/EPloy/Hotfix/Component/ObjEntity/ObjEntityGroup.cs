using System;
using EPloy.ObjectPool;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ObjEntity
{
    /// <summary>
    /// 实体组。
    /// </summary>
    public sealed class ObjEntityGroup : IReference
    {
        private Transform Parent;
        private ObjectPoolBase InstancePool;
        private TypeLinkedList<ObjEntityBase> ObjEntities;
        private LinkedListNode<ObjEntityBase> CachedNode;

        /// <summary>
        /// 名称
        /// </summary>
        public ObjEntityGroupName GroupName
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
        public int EntityCount
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
        public ObjEntityGroup(ObjEntityGroupName groupName, ObjectPoolBase instancePool, Transform parent)
        {
            GroupName = groupName;
            Parent = parent;
            InstancePool = instancePool;
            ObjEntities = new TypeLinkedList<ObjEntityBase>();
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
            LinkedListNode<ObjEntityBase> current = ObjEntities.First;
            while (current != null)
            {
                CachedNode = current.Next;
                current.Value.OnUpdate();
                current = CachedNode;
                CachedNode = null;
            }
        }

        /// <summary>
        ///是否存在实体。
        /// </summary>
        /// <param name="entityId">实体序列编号。</param>
        /// <returns>实体组中是否存在实体。</returns>
        public bool HasObjEntity(int entityId)
        {
            foreach (ObjEntityBase entity in ObjEntities)
            {
                if (entity.Id == entityId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="entityId">实体序列编号。</param>
        /// <returns>要获取的实体。</returns>
        public ObjEntityBase GetObjEntity(int entityId)
        {
            foreach (ObjEntityBase entity in ObjEntities)
            {
                if (entity.Id == entityId)
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
        public ObjEntityBase[] GetAllObjEntities()
        {
            List<ObjEntityBase> results = new List<ObjEntityBase>();
            foreach (ObjEntityBase entity in ObjEntities)
            {
                results.Add(entity);
            }
            return results.ToArray();
        }

        public ObjEntityBase ShowObjEntity(bool isNew, object obj, int entityId, Type objEntityType, object userData)
        {
            ObjEntityBase objEntityBase = null;
            if (isNew)
            {
                if (objEntityType == null || objEntityType.IsInstanceOfType(typeof(ObjEntityBase)))
                {
                    Log.Fatal("can not fand ui C# class  uiName : " + objEntityType.ToString());
                    return objEntityBase;
                }
                objEntityBase = (ObjEntityBase)ReferencePool.Acquire(objEntityType);
                ObjEntities.AddLast(objEntityBase);
            }
            else
            {
                objEntityBase = GetObjEntity(entityId);
            }

            if (objEntityBase == null)
            {
                Log.Fatal(Utility.Text.Format("UIForm {0} is invalid.", objEntityType.ToString()));
                return objEntityBase;
            }
            GameObject objEntityGo = obj as GameObject;
            objEntityGo.transform.SetParent(Handle.transform);
            objEntityBase.Initialize(isNew, objEntityGo, entityId, this, userData);
            return objEntityBase;
        }

        /// <summary>
        /// 从实体组移除实体。
        /// </summary>
        /// <param name="entity">要移除的实体。</param>
        public void RemoveObjEntity(ObjEntityBase entity)
        {
            if (CachedNode != null && CachedNode.Value == entity)
            {
                CachedNode = CachedNode.Next;
            }

            if (!ObjEntities.Remove(entity))
            {
                Log.Error(Utility.Text.Format("Entity group '{0}' not exists specified entity '[{1}]{2}'.", GroupName, entity.Id.ToString(), entity.Handle.name));
            }
        }

        public void RegisterObjEntity(ObjEntityInstance obj, bool spawned)
        {
            InstancePool.Register(obj, spawned);
        }

        public ObjEntityInstance SpawnObjEntityInstance(string name)
        {
            return (ObjEntityInstance)InstancePool.Spawn(name);
        }

        public void UnspawnObjEntity(ObjEntityBase entity)
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
