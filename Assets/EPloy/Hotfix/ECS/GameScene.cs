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
        /// Component，Component.Id
        /// </summary>
        private readonly Dictionary<Int64, Component> allCpts = new Dictionary<Int64, Component>();

        private readonly Dictionary<Type, Component> singleCpts = new Dictionary<Type, Component>();
        

        private long recordId;

        public void Awake()
        {
            recordId = 0;
        }

        public void Update()
        {
            foreach (var system in systems)
            {
                try
                {
                    if (system.IsPause) continue;
                    system.Update();
                }
                catch (Exception e)
                {
                    Log.Error(Utility.Text.Format("system: {0}  err: {1}", system, e.ToString()));
                }
            }
        }

        public void OnDestroy()
        {
            systems.Clear();
            entitys.Clear();
            allCpts.Clear();
            recordId = -1;
            singleCpts.Clear();
        }
        #region 系统管理

        /// <summary>
        /// 创建系统
        /// </summary>
        /// <returns></returns>
        public T CreateSystem<T>() where T : ISystem, new()
        {
            ISystem system = new T();
            system.Start();
            system.IsPause = false;
            systems.Add(system);
            return (T) system;
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

            Component component = AddCpt<T>(null);
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
            allCpts.Add(recordId, component);
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
            allCpts.Add(recordId, component);
            entity.AddComponent(component);
            return (T) component;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <returns></returns>
        public Component GetCpt(long id)
        {
            if (allCpts.ContainsKey(id))
            {
                return allCpts[id];
            }

            return null;
        }

        /// <summary>
        /// 销毁实体上一个组件
        /// </summary>
        public void ReleaseCpt<T>(Entity entity) where T : Component
        {
            Component component = entity.GetComponent<T>();
            if (component.IsRelease) return;
            allCpts.Remove(component.Id);
            if (singleCpts.ContainsValue(component))
            {
                singleCpts.Remove(component.GetType());
            }

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
                allCpts.Remove(component.Id);
            }

            entity.RemoveAllComponent();
            ReferencePool.Release(entity);
        }

        #endregion
    }
}