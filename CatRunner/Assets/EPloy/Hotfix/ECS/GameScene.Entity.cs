using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// EcsGame场景
    /// </summary>
    public partial class GameScene : IHotfixModule
    {
        private EntityMap entityMap=null;
        private readonly List<EntityRole> entityRoles = new List<EntityRole>();

        private long EntityRecordId;

        /// <summary>
        /// 当前所有角色实体
        /// </summary>
        public List<EntityRole> EntityRoles
        {
            get
            {
                return entityRoles;
            }
        }

        public EntityRole CreateEntityRole(string name = null)
        {
            EntityRole entityRole=(EntityRole)CreateEntity<EntityRole>(name);
            entityRoles.Add(entityRole);
            return entityRole;
        }

        /// <summary>
        /// 生成一个实体
        /// </summary>
        /// <returns></returns>
        public T CreateEntity<T>(string name) where T: Entity
        {
            Entity entity = (Entity)ReferencePool.Acquire(typeof(T));
            EntityRecordId++;
            entity.Awake(EntityRecordId, name);
            return (T)entity;
        }

        /// <summary>
        /// 销毁一个实体
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            //还要对应list 移除 待定
            if (entity.IsRelease) return;
            ReferencePool.Release(entity);
        }
    }
}