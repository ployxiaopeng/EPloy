using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EPloy
{
    public class Entity : IEntity, IReference
    {
        public string Name;

        public int id { get; set; }

        /// <summary>
        /// 实体是否释放
        /// </summary>
        public bool IsRelease
        {
            get
            {
                return id == -1;
            }
        }

        private Entity parentEntity;

        /// <summary>
        /// 实体组件
        /// </summary>
        protected Dictionary<Type, Component> ComponentDictionary = new Dictionary<Type, Component>();
        /// <summary>
        /// 实体还有实体
        /// </summary>
        protected Dictionary<int, Entity> EntityDictionary = new Dictionary<int, Entity>();
        /// <summary>
        /// 父级实体 并且唯一
        /// </summary>
        public Entity ParentEntity
        {
            get
            {
                return parentEntity;
            }
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="_id"></param>
        public virtual void Awake(int _id)
        {
            Awake(_id, null, null);
        }
        public virtual void Awake(int _id, string name)
        {
            Awake(_id, null, name);
        }
        public virtual void Awake(int _id, Entity _parentEntity, string name)
        {
            id = _id;
            Name = name;
            parentEntity = _parentEntity;
           // Log.Info(Utility.Text.Format("创建一个实体 id: {0} name: {1}", id, Name));
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (ComponentDictionary.ContainsKey(type))
            {
                Log.Fatal(Utility.Text.Format("Component {0} is in Entity ", type));
                return null;
            }
            Component component = GameEntry.GameEntity.WithIdComponent.CreateComponent(this, type);
            ComponentDictionary.Add(type, component);
            GameEntry.GameSystem.Awake(component);
            return (T)component;
        }

        /// <summary>
        /// 是否存在组件
        /// </summary>
        public bool HasComponent<T>() where T : Component
        {
            Type type = typeof(T);
            return ComponentDictionary.ContainsKey(type);
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (!ComponentDictionary.ContainsKey(type))
            {
                Log.Fatal(Utility.Text.Format("Component {0} is not in Entity ", type));
                return null;
            }
            return (T)ComponentDictionary[type];
        }

        /// <summary>
        /// 添加子实体
        /// </summary>
        public void AddChild(Entity entity)
        {
            if (EntityDictionary.ContainsKey(entity.id))
            {
                Log.Fatal(Utility.Text.Format("entityId {0} is in Entity ", entity.id));
                return;
            }
            EntityDictionary.Add(entity.id, entity);
            entity.Awake(entity.id, this, entity.Name);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent<T>() where T : Component
        {
            if (!ComponentDictionary.ContainsKey(typeof(T)))
            {
                Log.Fatal(Utility.Text.Format("Component {0} is not in Entity ", typeof(T)));
                return;
            }
            GameEntry.GameEntity.WithIdComponent.ReleaseComponent(ComponentDictionary[typeof(T)]);
            ComponentDictionary.Remove(typeof(T));
        }

        /// <summary>
        /// 移除实体所有组件
        /// </summary>
        public void RemoveAllComponent()
        {
            foreach (var component in ComponentDictionary)
            {
                GameEntry.GameEntity.WithIdComponent.ReleaseComponent(component.Value);
            }
        }

        /// <summary>
        /// 移除子实体
        /// </summary>
        public void RemoveChild(Entity entity)
        {
            if (EntityDictionary.ContainsKey(entity.id))
            {
                Log.Fatal(Utility.Text.Format("entityId {0} is not in Entity ", entity.id));
                return;
            }
            entity.RemoveAllComponent();
            EntityDictionary.Remove(entity.id);
        }

        /// <summary>
        /// 实体清理
        /// </summary>
        public void Clear()
        {
            id = -1;
            ComponentDictionary.Clear();
            EntityDictionary.Clear();
        }
    }
}
