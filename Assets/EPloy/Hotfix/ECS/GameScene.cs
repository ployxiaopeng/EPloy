using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// EcsGame场景
    /// </summary>
    public class GameScene : IHotfixModule
    {
        /// <summary>
        /// 所有System
        /// </summary>
        private readonly List<ISystem> systems = new List<ISystem>();

        /// <summary>
        /// 所有Entity，Int32是Entity.Id
        /// </summary>
        private readonly Dictionary<Int64, Entity> entitys = new Dictionary<Int64, Entity>();

        /// <summary>
        /// 所有Component，Component.Id
        /// </summary>
        private readonly Dictionary<Int64, Component> entityCpts = new Dictionary<Int64, Component>();
        /// <summary>
        /// 所有单例Component，Component Type
        /// </summary>
        private readonly Dictionary<Type, Component> singleCpts = new Dictionary<Type, Component>();
        

        private long recordId;

        public override void Awake()
        {
            recordId = 0;
        }

        public override void Update()
        {
            for (int i = 0; i < systems.Count; i++)
            {
                try
                {
                    if (systems[i].IsPause) continue;
                    systems[i].Update();
                }
                catch (Exception e)
                {
                    Log.Error(Utility.Text.Format("system: {0}  err: {1}", systems[i], e.ToString()));
                }
            }
        }

        public override void OnDestroy()
        {
            systems.Clear();
            entitys.Clear();
            entityCpts.Clear();
            recordId = -1;
            singleCpts.Clear();
        }
        #region 系统管理

        /// <summary>
        /// 创建系统
        /// </summary>
        /// <returns></returns>
        public void CreateSystem<T>() where T : ISystem, new()
        {
            ISystem system = new T();
            system.Start();
            system.IsPause = false;
            systems.Add(system);
        }

        /// <summary>
        /// 暂停系统
        /// </summary>
        /// <returns></returns>
        public void PauseAllSystem()
        {
            foreach (var system in systems)
            {
                system.IsPause = true;
            }
        }

        #endregion

        #region 组件管理

        /// <summary>
        /// 获取单例的组件
        /// </summary>
        public T GetSingleCpt<T>() where T : Component, new()
        {
            Type type = typeof(T);
            if (singleCpts.ContainsKey(type))
            {
                return (T) singleCpts[type];
            }

            Component component = ReferencePool.Acquire<T>();
            singleCpts.Add(type, component);
            return (T) component;
        }

        /// <summary>
        /// 给实体添加组件
        /// </summary>
        /// <returns></returns>
        public T AddCpt<T>(Entity entity) where T : Component, new()
        {
            Component component = ReferencePool.Acquire<T>();
            component.Awake(entity, recordId++);
            entityCpts.Add(recordId, component);
            entity.AddComponent(component);
            return (T) component;
        }

        /// <summary>
        /// 给实体添加组件
        /// </summary>
        /// <returns></returns>
        public T AddCpt<T>(long id) where T : Component, new()
        {
            Entity entity = GetEntity(id);
            if (entity == null)
            {
                Log.Error("can not find entity");
                return null;
            }

            recordId++;
            Component component = ReferencePool.Acquire<T>();
            component.Awake(entity, recordId);
            entityCpts.Add(recordId, component);
            entity.AddComponent(component);
            return (T) component;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <returns></returns>
        public Component GetCpt(long id)
        {
            if (entityCpts.ContainsKey(id))
            {
                return entityCpts[id];
            }

            return null;
        }

        /// <summary>
        /// 销毁实体组件
        /// </summary>
        public void ReleaseCpt<T>(Entity entity) where T : Component
        {
            Component component = entity.GetComponent<T>();
            if (component.IsRelease) return;
            entityCpts.Remove(component.Id);
            entity.RemoveComponent<T>();
            ReferencePool.Release(component);
        }

        #endregion

        #region 实体管理

        /// <summary>
        /// 生成一个实体
        /// </summary>
        /// <returns></returns>
        public T CreateEntity<T>(string name = null) where T : Entity
        {
            Entity entity = (Entity) ReferencePool.Acquire(typeof(T));
            recordId++;
            entity.Awake(recordId, name);
            entitys.Add(recordId, entity);
            return (T) entity;
        }

        /// <summary>
        /// 生成一个实体
        /// </summary>
        /// <returns></returns>
        public Entity CreateEntity(string name = null)
        {
            return CreateEntity<Entity>(name);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        public Entity GetEntity(long id)
        {
            if (entitys.ContainsKey(id))
            {
                return entitys[id];
            }

            return null;
        }

        /// <summary>
        /// 销毁一个实体
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            if (entity.IsRelease) return;
            Component[] components = entity.GetAllComponent();
            foreach (var component in components)
            {
                entityCpts.Remove(component.Id);
            }

            entity.RemoveAllComponent();
            ReferencePool.Release(entity);
        }

        #endregion
    }
}