using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy
{
    /// <summary>
    /// 全局唯一游戏实体 他具备实体的特性  但是他管理所有实体
    /// </summary>
    public class GameEntity : Entity
    {
        private int RecordId;

        public WithIdComponent WithIdComponent
        {
            get; private set;
        }

        public override void Awake(int _id, string name)
        {
            base.Awake(_id, name);
            ResetEntityId();
            //默认添加WithIdComponent
            WithIdComponent = ReferencePool.Acquire<WithIdComponent>();
            WithIdComponent.SetEntity(this, 0);
            ComponentDictionary.Add(typeof(WithIdComponent), WithIdComponent);
        }

        /// <summary>
        /// 生成一个实体
        /// </summary>
        /// <returns></returns>
        public Entity CreateEntity(string name = null)
        {
            Entity entity = ReferencePool.Acquire<Entity>();
            int id = GetNextEntityId();
            entity.Awake(id, name);
            EntityDictionary.Add(id, entity);
            return entity;
        }

        /// <summary>
        /// 销毁一个实体
        /// </summary>
        public void ReleaseEntity(Entity entity)
        {
            if (entity.IsRelease) return;
            if (entity.ParentEntity != null)
                entity.ParentEntity.RemoveChild(entity);
            ReferencePool.Release(entity);
        }

        /// <summary>
        /// 获取一个不存在的实体Id
        /// </summary>
        /// <returns></returns>
        public int GetNextEntityId()
        {
            while (EntityDictionary.ContainsKey(RecordId))
            {
                RecordId++;
            }
            return RecordId;
        }

        public void ResetEntityId()
        {
            //0是自己  其他所有的从1开始
            RecordId = 1;
        }

    }
}
