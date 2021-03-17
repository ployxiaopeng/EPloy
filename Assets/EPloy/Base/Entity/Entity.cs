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
        protected Dictionary<Type, Component> ComponentDictionary=new Dictionary<Type, Component>();
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
            id = _id;
            Debug.LogError("创建一个实体"+id);
        }
        public virtual void Awake(int _id,string name=null)
        {
            Awake(_id);
            Name = name;
            parentEntity = null;
        }
        public virtual void Awake(int _id, Entity _parentEntity, string name=null)
        {
            Awake(_id);
            Name = name;
            parentEntity = _parentEntity;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (ComponentDictionary.ContainsKey(type))
            {
                new EPloyException(Utility.Text.Format("Component {0} is in Entity ", type));
                return null;
            }
            Component component = GameEntry.Game.WithIdComponent.CreateComponent(this, type);
            ComponentDictionary.Add(type, component);
            GameEntry.GameSystem.Awake(component);
            return (T)component;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (!ComponentDictionary.ContainsKey(type))
            {
                new EPloyException(Utility.Text.Format("Component {0} is not in Entity ", type));
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
                new EPloyException(Utility.Text.Format("entityId {0} is in Entity ", entity.id));
                return;
            }
            EntityDictionary.Add(entity.id, entity);
            entity.Awake(entity.id, this);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent<T>() where T : Component
        {
            if (!ComponentDictionary.ContainsKey(typeof(T)))
            {
                new EPloyException(Utility.Text.Format("Component {0} is not in Entity ", typeof(T)));
                return;
            }
            GameEntry.Game.WithIdComponent.ReleaseComponent(ComponentDictionary[typeof(T)]);
            ComponentDictionary.Remove(typeof(T));
        }

        /// <summary>
        /// 移除实体所有组件
        /// </summary>
        public void RemoveAllComponent()
        {
            foreach (var  component in ComponentDictionary)
            {
                GameEntry.Game.WithIdComponent.ReleaseComponent(component.Value);
            }
        }

        /// <summary>
        /// 移除子实体
        /// </summary>
        public void RemoveChild(Entity entity)
        {
            if (EntityDictionary.ContainsKey(entity.id))
            {
                new EPloyException(Utility.Text.Format("entityId {0} is not in Entity ", entity.id));
                return ;
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
