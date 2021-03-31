//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 实体组件。
    /// </summary>
    public  class EntityComponent : Component
    {
        private const int DefaultPriority = 0;

        public IEntityManager EntityManager  { get;set;}
        private readonly List<IEntity> m_InternalEntityLogicResultsCache = new List<IEntity>();


        /// <summary>
        /// 获取实体数量。
        /// </summary>
        public int EntityCount
        {
            get
            {
                return EntityManager.EntityCount;
            }
        }
        /// <summary>
        /// 获取实体组数量。
        /// </summary>
        public int EntityGroupCount
        {
            get
            {
                return EntityManager.EntityGroupCount;
            }
        }

        /// <summary>
        /// 是否存在实体组。
        /// </summary>
        /// <param name="EntityGroupName">实体组名称。</param>
        /// <returns>是否存在实体组。</returns>
        public bool HasEntityGroup(string EntityGroupName)
        {
            return EntityManager.HasEntityGroup(EntityGroupName);
        }

        /// <summary>
        /// 获取实体组。
        /// </summary>
        /// <param name="EntityGroupName">实体组名称。</param>
        /// <returns>要获取的实体组。</returns>
        public IEntityGroup GetEntityGroup(string EntityGroupName)
        {
            return EntityManager.GetEntityGroup(EntityGroupName);
        }

        /// <summary>
        /// 获取所有实体组。
        /// </summary>
        /// <returns>所有实体组。</returns>
        public IEntityGroup[] GetAllEntityGroups()
        {
            return EntityManager.GetAllEntityGroups();
        }

        /// <summary>
        /// 获取所有实体组。
        /// </summary>
        /// <param name="results">所有实体组。</param>
        public void GetAllEntityGroups(List<IEntityGroup> results)
        {
            EntityManager.GetAllEntityGroups(results);
        }

        /// <summary>
        /// 增加实体组。
        /// </summary>
        /// <param name="EntityGroupName">实体组名称。</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数。</param>
        /// <param name="instanceCapacity">实体实例对象池容量。</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数。</param>
        /// <param name="instancePriority">实体实例对象池的优先级。</param>
        /// <returns>是否增加实体组成功。</returns>
        public bool AddEntityGroup(string EntityGroupName, float instanceAutoReleaseInterval, int instanceCapacity, float instanceExpireTime, int instancePriority)
        {
            if (EntityManager.HasEntityGroup(EntityGroupName))
            {
                return false;
            }
            EntityGroupHelper groupHelper = new EntityGroupHelper(Utility.Text.Format("Entity - {0}", EntityGroupName), Init.Instance.transform.Find("Entity"));
            return EntityManager.AddEntityGroup(EntityGroupName, instanceAutoReleaseInterval, instanceCapacity, instanceExpireTime, instancePriority, groupHelper);
        }

        /// <summary>
        /// 是否存在实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <returns>是否存在实体。</returns>
        public bool HasEntity(int EntityId)
        {
            return EntityManager.HasEntity(EntityId);
        }

        /// <summary>
        /// 是否存在实体。
        /// </summary>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <returns>是否存在实体。</returns>
        public bool HasEntity(string EntityLogicAssetName)
        {
            return EntityManager.HasEntity(EntityLogicAssetName);
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <returns>实体。</returns>
        public EntityLogic GetEntity(int EntityId)
        {
            return (EntityLogic)EntityManager.GetEntity(EntityId);
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <returns>要获取的实体。</returns>
        public EntityLogic GetEntity(string EntityLogicAssetName)
        {
            return (EntityLogic)EntityManager.GetEntity(EntityLogicAssetName);
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <returns>要获取的实体。</returns>
        public EntityLogic[] GetEntities(string EntityLogicAssetName)
        {
            IEntity[] entities = EntityManager.GetEntities(EntityLogicAssetName);
            EntityLogic[] EntityLogicImpls = new EntityLogic[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                EntityLogicImpls[i] = (EntityLogic)entities[i];
            }

            return EntityLogicImpls;
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <param name="results">要获取的实体。</param>
        public void GetEntities(string EntityLogicAssetName, List<EntityLogic> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            EntityManager.GetEntities(EntityLogicAssetName, m_InternalEntityLogicResultsCache);
            foreach (IEntity EntityLogic in m_InternalEntityLogicResultsCache)
            {
                results.Add((EntityLogic)EntityLogic);
            }
        }

        /// <summary>
        /// 获取所有已加载的实体。
        /// </summary>
        /// <returns>所有已加载的实体。</returns>
        public EntityLogic[] GetAllLoadedEntities()
        {
            IEntity[] entities = EntityManager.GetAllLoadedEntities();
            EntityLogic[] EntityLogicImpls = new EntityLogic[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                EntityLogicImpls[i] = (EntityLogic)entities[i];
            }

            return EntityLogicImpls;
        }

        /// <summary>
        /// 获取所有已加载的实体。
        /// </summary>
        /// <param name="results">所有已加载的实体。</param>
        public void GetAllLoadedEntities(List<EntityLogic> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            EntityManager.GetAllLoadedEntities(m_InternalEntityLogicResultsCache);
            foreach (IEntity EntityLogic in m_InternalEntityLogicResultsCache)
            {
                results.Add((EntityLogic)EntityLogic);
            }
        }

        /// <summary>
        /// 获取所有正在加载实体的编号。
        /// </summary>
        /// <returns>所有正在加载实体的编号。</returns>
        public int[] GetAllLoadingEntityIds()
        {
            return EntityManager.GetAllLoadingEntityIds();
        }

        /// <summary>
        /// 获取所有正在加载实体的编号。
        /// </summary>
        /// <param name="results">所有正在加载实体的编号。</param>
        public void GetAllLoadingEntityIds(List<int> results)
        {
            EntityManager.GetAllLoadingEntityIds(results);
        }

        /// <summary>
        /// 是否正在加载实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <returns>是否正在加载实体。</returns>
        public bool IsLoadingEntity(int EntityId)
        {
            return EntityManager.IsLoadingEntity(EntityId);
        }

        /// <summary>
        /// 是否是合法的实体。
        /// </summary>
        /// <param name="EntityLogic">实体。</param>
        /// <returns>实体是否合法。</returns>
        public bool IsValidEntity(EntityLogic EntityLogic)
        {
            return EntityManager.IsValidEntity(EntityLogic);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <param name="EntityLogicType">实体逻辑类型。</param>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <param name="EntityGroupName">实体组名称。</param>
        public void ShowEntity(int EntityId, string EntityLogicAssetName, string EntityGroupName)
        {
            ShowEntity(EntityId, EntityLogicAssetName, EntityGroupName, DefaultPriority, null);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <param name="EntityLogicType">实体逻辑类型。</param>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <param name="EntityGroupName">实体组名称。</param>
        /// <param name="priority">加载实体资源的优先级。</param>
        public void ShowEntity(int EntityId,  string EntityLogicAssetName, string EntityGroupName, int priority)
        {
            ShowEntity(EntityId, EntityLogicAssetName, EntityGroupName, priority, null);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <param name="EntityLogicType">实体逻辑类型。</param>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <param name="EntityGroupName">实体组名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void ShowEntity(int EntityId,  string EntityLogicAssetName, string EntityGroupName, object userData)
        {
            ShowEntity(EntityId, EntityLogicAssetName, EntityGroupName, DefaultPriority, userData);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <param name="EntityLogicType">实体逻辑类型。</param>
        /// <param name="EntityLogicAssetName">实体资源名称。</param>
        /// <param name="EntityGroupName">实体组名称。</param>
        /// <param name="priority">加载实体资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void ShowEntity(int EntityId,string EntityLogicAssetName, string EntityGroupName, int priority, object userData)
        {
            EntityManager.ShowEntity(EntityId, EntityLogicAssetName, EntityGroupName, priority,  userData);
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        public void HideEntity(int EntityId)
        {
            EntityManager.HideEntity(EntityId);
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="EntityId">实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void HideEntity(int EntityId, object userData)
        {
            EntityManager.HideEntity(EntityId, userData);
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="EntityLogic">实体。</param>
        public void HideEntity(EntityLogic EntityLogic)
        {
            EntityManager.HideEntity(EntityLogic);
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="EntityLogic">实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void HideEntity(EntityLogic EntityLogic, object userData)
        {
            EntityManager.HideEntity(EntityLogic, userData);
        }

        /// <summary>
        /// 隐藏所有已加载的实体。
        /// </summary>
        public void HideAllLoadedEntities()
        {
            EntityManager.HideAllLoadedEntities();
        }

        /// <summary>
        /// 隐藏所有已加载的实体。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void HideAllLoadedEntities(object userData)
        {
            EntityManager.HideAllLoadedEntities(userData);
        }

        /// <summary>
        /// 隐藏所有正在加载的实体。
        /// </summary>
        public void HideAllLoadingEntities()
        {
            EntityManager.HideAllLoadingEntities();
        }

        /// <summary>
        /// 获取父实体。
        /// </summary>
        /// <param name="childEntityId">要获取父实体的子实体的实体编号。</param>
        /// <returns>子实体的父实体。</returns>
        public EntityLogic GetParentEntity(int childEntityId)
        {
            return (EntityLogic)EntityManager.GetParentEntity(childEntityId);
        }

        /// <summary>
        /// 获取父实体。
        /// </summary>
        /// <param name="childEntity">要获取父实体的子实体。</param>
        /// <returns>子实体的父实体。</returns>
        public EntityLogic GetParentEntity(EntityLogic childEntity)
        {
            return (EntityLogic)EntityManager.GetParentEntity(childEntity);
        }

        /// <summary>
        /// 获取子实体。
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号。</param>
        /// <returns>子实体数组。</returns>
        public EntityLogic[] GetChildEntities(int parentEntityId)
        {
            IEntity[] entities = EntityManager.GetChildEntities(parentEntityId);
            EntityLogic[] EntityLogicImpls = new EntityLogic[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                EntityLogicImpls[i] = (EntityLogic)entities[i];
            }

            return EntityLogicImpls;
        }

        /// <summary>
        /// 获取子实体。
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号。</param>
        /// <param name="results">子实体数组。</param>
        public void GetChildEntities(int parentEntityId, List<IEntity> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            EntityManager.GetChildEntities(parentEntityId, m_InternalEntityLogicResultsCache);
            foreach (IEntity EntityLogic in m_InternalEntityLogicResultsCache)
            {
                results.Add((EntityLogic)EntityLogic);
            }
        }

        /// <summary>
        /// 获取子实体。
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体。</param>
        /// <returns>子实体数组。</returns>
        public EntityLogic[] GetChildEntities(EntityLogic parentEntity)
        {
            IEntity[] entities = EntityManager.GetChildEntities(parentEntity);
            EntityLogic[] EntityLogicImpls = new EntityLogic[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                EntityLogicImpls[i] = (EntityLogic)entities[i];
            }

            return EntityLogicImpls;
        }

        /// <summary>
        /// 获取子实体。
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体。</param>
        /// <param name="results">子实体数组。</param>
        public void GetChildEntities(IEntity parentEntity, List<IEntity> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            EntityManager.GetChildEntities(parentEntity, m_InternalEntityLogicResultsCache);
            foreach (IEntity EntityLogic in m_InternalEntityLogicResultsCache)
            {
                results.Add((EntityLogic)EntityLogic);
            }
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        public void AttachEntityLogic(int childEntityId, int parentEntityId)
        {
            AttachEntityLogic(GetEntity(childEntityId), GetEntity(parentEntityId), string.Empty, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        public void AttachEntityLogic(int childEntityId, EntityLogic parentEntity)
        {
            AttachEntityLogic(GetEntity(childEntityId), parentEntity, string.Empty, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        public void AttachEntityLogic(EntityLogic childEntity, int parentEntityId)
        {
            AttachEntityLogic(childEntity, GetEntity(parentEntityId), string.Empty, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        public void AttachEntityLogic(EntityLogic childEntity, EntityLogic parentEntity)
        {
            AttachEntityLogic(childEntity, parentEntity, string.Empty, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(int childEntityId, int parentEntityId, string parentTransformPath)
        {
            AttachEntityLogic(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(int childEntityId, EntityLogic parentEntity, string parentTransformPath)
        {
            AttachEntityLogic(GetEntity(childEntityId), parentEntity, parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(EntityLogic childEntity, int parentEntityId, string parentTransformPath)
        {
            AttachEntityLogic(childEntity, GetEntity(parentEntityId), parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(EntityLogic childEntity, EntityLogic parentEntity, string parentTransformPath)
        {
            AttachEntityLogic(childEntity, parentEntity, parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(int childEntityId, int parentEntityId, Transform parentTransform)
        {
            AttachEntityLogic(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransform, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(int childEntityId, EntityLogic parentEntity, Transform parentTransform)
        {
            AttachEntityLogic(GetEntity(childEntityId), parentEntity, parentTransform, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(EntityLogic childEntity, int parentEntityId, Transform parentTransform)
        {
            AttachEntityLogic(childEntity, GetEntity(parentEntityId), parentTransform, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        public void AttachEntityLogic(EntityLogic childEntity, EntityLogic parentEntity, Transform parentTransform)
        {
            AttachEntityLogic(childEntity, parentEntity, parentTransform, null);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(int childEntityId, int parentEntityId, object userData)
        {
            AttachEntityLogic(GetEntity(childEntityId), GetEntity(parentEntityId), string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(int childEntityId, EntityLogic parentEntity, object userData)
        {
            AttachEntityLogic(GetEntity(childEntityId), parentEntity, string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(EntityLogic childEntity, int parentEntityId, object userData)
        {
            AttachEntityLogic(childEntity, GetEntity(parentEntityId), string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(EntityLogic childEntity, EntityLogic parentEntity, object userData)
        {
            AttachEntityLogic(childEntity, parentEntity, string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(int childEntityId, int parentEntityId, string parentTransformPath, object userData)
        {
            AttachEntityLogic(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(int childEntityId, EntityLogic parentEntity, string parentTransformPath, object userData)
        {
            AttachEntityLogic(GetEntity(childEntityId), parentEntity, parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(EntityLogic childEntity, int parentEntityId, string parentTransformPath, object userData)
        {
            AttachEntityLogic(childEntity, GetEntity(parentEntityId), parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(EntityLogic childEntity, EntityLogic parentEntity, string parentTransformPath, object userData)
        {
            if (childEntity == null)
            {
                Log.Warning("Child EntityLogic is invalid.");
                return;
            }

            if (parentEntity == null)
            {
                Log.Warning("Parent EntityLogic is invalid.");
                return;
            }

            Transform parentTransform = null;
            if (string.IsNullOrEmpty(parentTransformPath))
            {
                parentTransform = parentEntity.transform;
            }
            else
            {
                parentTransform = parentEntity.transform.Find(parentTransformPath);
                if (parentTransform == null)
                {
                    Log.Warning("Can not find transform path '{0}' from parent EntityLogic '{1}'.", parentTransformPath, parentEntity.gameObject.name);
                    parentTransform = parentEntity.transform;
                }
            }
            AttachEntityLogic(childEntity, parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(int childEntityId, int parentEntityId, Transform parentTransform, object userData)
        {
            AttachEntityLogic(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(int childEntityId, EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            AttachEntityLogic(GetEntity(childEntityId), parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(EntityLogic childEntity, int parentEntityId, Transform parentTransform, object userData)
        {
            AttachEntityLogic(childEntity, GetEntity(parentEntityId), parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体。
        /// </summary>
        /// <param name="childEntity">要附加的子实体。</param>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="parentTransform">相对于被附加父实体的位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void AttachEntityLogic(EntityLogic childEntity, EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            if (childEntity == null)
            {
                Log.Warning("Child EntityLogic is invalid.");
                return;
            }

            if (parentEntity == null)
            {
                Log.Warning("Parent EntityLogic is invalid.");
                return;
            }

            if (parentTransform == null)
            {
                parentTransform = parentEntity.transform;
            }

            EntityManager.AttachEntity(childEntity, parentEntity, new AttachEntityInfo(parentTransform, userData));
        }

        /// <summary>
        /// 解除子实体。
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号。</param>
        public void DetachEntity(int childEntityId)
        {
            EntityManager.DetachEntity(childEntityId);
        }

        /// <summary>
        /// 解除子实体。
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void DetachEntity(int childEntityId, object userData)
        {
            EntityManager.DetachEntity(childEntityId, userData);
        }

        /// <summary>
        /// 解除子实体。
        /// </summary>
        /// <param name="childEntity">要解除的子实体。</param>
        public void DetachEntity(EntityLogic childEntity)
        {
            EntityManager.DetachEntity(childEntity);
        }

        /// <summary>
        /// 解除子实体。
        /// </summary>
        /// <param name="childEntity">要解除的子实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void DetachEntity(EntityLogic childEntity, object userData)
        {
            EntityManager.DetachEntity(childEntity, userData);
        }

        /// <summary>
        /// 解除所有子实体。
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号。</param>
        public void DetachChildEntities(int parentEntityId)
        {
            EntityManager.DetachChildEntities(parentEntityId);
        }

        /// <summary>
        /// 解除所有子实体。
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void DetachChildEntities(int parentEntityId, object userData)
        {
            EntityManager.DetachChildEntities(parentEntityId, userData);
        }

        /// <summary>
        /// 解除所有子实体。
        /// </summary>
        /// <param name="parentEntity">被解除的父实体。</param>
        public void DetachChildEntities(EntityLogic parentEntity)
        {
            EntityManager.DetachChildEntities(parentEntity);
        }

        /// <summary>
        /// 解除所有子实体。
        /// </summary>
        /// <param name="parentEntity">被解除的父实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void DetachChildEntities(EntityLogic parentEntity, object userData)
        {
            EntityManager.DetachChildEntities(parentEntity, userData);
        }

        /// <summary>
        /// 设置实体是否被加锁。
        /// </summary>
        /// <param name="EntityLogic">实体。</param>
        /// <param name="locked">实体是否被加锁。</param>
        public void SetEntityLogicInstanceLocked(EntityLogic EntityLogic, bool locked)
        {
            if (EntityLogic == null)
            {
                Log.Warning("EntityLogic is invalid.");
                return;
            }

            IEntityGroup EntityGroup = EntityLogic.EntityGroup;
            if (EntityGroup == null)
            {
                Log.Warning("EntityLogic group is invalid.");
                return;
            }

            EntityGroup.SetEntityInstanceLocked(EntityLogic.gameObject, locked);
        }

        /// <summary>
        /// 设置实体的优先级。
        /// </summary>
        /// <param name="EntityLogic">实体。</param>
        /// <param name="priority">实体优先级。</param>
        public void SetInstancePriority(EntityLogic EntityLogic, int priority)
        {
            if (EntityLogic == null)
            {
                Log.Warning("EntityLogic is invalid.");
                return;
            }

            IEntityGroup EntityGroup = EntityLogic.EntityGroup;
            if (EntityGroup == null)
            {
                Log.Warning("EntityLogic group is invalid.");
                return;
            }

            EntityGroup.SetEntityInstancePriority(EntityLogic.gameObject, priority);
        }
    }
}
