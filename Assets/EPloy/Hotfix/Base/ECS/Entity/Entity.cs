using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EPloy
{
    public class Entity : IEntity, IReference
    {
        protected GameScene GameScene;

        public string Name { get; private set; }

        public long Id { get; set; }

        /// <summary>
        /// 实体是否释放
        /// </summary>
        public bool IsRelease
        {
            get { return Id == -1; }
        }

        /// <summary>
        /// 实体组件
        /// </summary>
        protected readonly  Dictionary<Type, Component> ComponentDictionary = new Dictionary<Type, Component>();

        /// <summary>
        /// 实体初始化
        /// </summary>
        public void Awake(long id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public void AddComponent(Component component)
        {
            Type type = component.GetType();
            if (ComponentDictionary.ContainsKey(type))
            {
                Log.Fatal(Utility.Text.Format("Component {0} is in Entity ", type));
            }

            ComponentDictionary.Add(type, component);
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

            return (T) ComponentDictionary[type];
        }

        /// <summary>
        /// 获取所有组件
        /// </summary>
        public Component[] GetAllComponent()
        {
            return ComponentDictionary.Values.ToArray();
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
            ComponentDictionary.Remove(typeof(T));
        }

        /// <summary>
        /// 移除实体所有组件
        /// </summary>
        public void RemoveAllComponent()
        {
            ComponentDictionary.Clear();
        }

        /// <summary>
        /// 实体清理
        /// </summary>
        public void Clear()
        {
            Id = -1;
            ComponentDictionary.Clear();
        }
    }
}
