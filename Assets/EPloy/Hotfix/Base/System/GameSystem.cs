using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    ///  全局唯一游戏系统 他管理所有系统流程
    /// </summary>
    public sealed class GameSystem
    {
        private Dictionary<long, Component> allComponents
        {
            get
            {
                return GameEntry.Game.WithIdComponent.AllComponents;
            }
        }
        private readonly Dictionary<string, Type[]> assemblies = new Dictionary<string, Type[]>();
        //private readonly UnOrderMultiMap<Type, Type> types = new UnOrderMultiMap<Type, Type>();

        private readonly UnOrderMultiMap<Type, ISystem> awakeSystems = new UnOrderMultiMap<Type, ISystem>();
        private readonly UnOrderMultiMap<Type, ISystem> startSystems = new UnOrderMultiMap<Type, ISystem>();
        private readonly UnOrderMultiMap<Type, ISystem> updateSystems = new UnOrderMultiMap<Type, ISystem>();

        private List<long> updates = new List<long>();
        private readonly Queue<long> starts = new Queue<long>();

        public Type[] GetTypes(string dllType)
        {
            return this.assemblies[dllType];
        }

        /// <summary>
        /// 添加DLL 数据
        /// </summary>
        /// <param name="dllType"></param>
        /// <param name="assembly"></param>
        public void Add(string dllType, Type[] assembly)
        {

            this.assemblies[dllType] = assembly;
            this.awakeSystems.Clear();
            this.updateSystems.Clear();
            this.startSystems.Clear();
            this.updates.Clear();
            this.starts.Clear();
            foreach (Type[] value in this.assemblies.Values)
            {
                foreach (Type type in value)
                {
                    if (AddSystemObj(type)) continue;
                }
            }
        }

        private bool AddSystemObj(Type type)
        {
            object[] objects = type.GetCustomAttributes(typeof(SystemAttribute), false);
            if (objects.Length != 0)
            {
                object obj = Activator.CreateInstance(type);
                ISystem system = (ISystem)obj;
                switch (obj)
                {
                    case IAwake awakeSystem:
                        this.awakeSystems.Add(system.Type(), system);
                        break;
                    case IStart startSystem:
                        this.startSystems.Add(system.Type(), system);
                        break;
                    case IUpdate updateSystem:
                        this.updateSystems.Add(system.Type(), system);
                        break;
                }
                Log.Info(type);
                return true;
            }
            return false;
        }

        public void Awake(Component component)
        {
            Type type = component.GetType();

            TypeLinkedList<ISystem> AwakeSystems = this.awakeSystems[type];
            if (AwakeSystems != null)
            {
                foreach (ISystem awakeSystem in AwakeSystems)
                {
                    if (awakeSystem == null) continue;
                    try
                    {
                        awakeSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(e.ToString());
                    }
                }
            }

            if (this.updateSystems.ContainsKey(type))
            {
                this.updates.Add(component.InstanceId);
            }

            if (this.startSystems.ContainsKey(type))
            {
                this.starts.Enqueue(component.InstanceId);
            }
        }

        private void Start()
        {
            while (this.starts.Count > 0)
            {
                long instanceId = this.starts.Dequeue();
                Component component;
                if (!this.allComponents.TryGetValue(instanceId, out component))
                {
                    continue;
                }

                TypeLinkedList<ISystem> StartSystems = this.startSystems[component.GetType()];
                if (StartSystems == null)
                {
                    continue;
                }

                foreach (ISystem startSystem in StartSystems)
                {
                    try
                    {
                        startSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(e.ToString());
                    }
                }
            }
        }
        public void Update()
        {
            this.Start();

            for (int i = 0; i < updates.Count; i++)
            {
                long instanceId = this.updates[i];
                Component component;
                if (!this.allComponents.TryGetValue(instanceId, out component))
                {
                    updates.RemoveAt(i);
                    continue;
                }

                if (component.IsRelease)
                {
                    updates.RemoveAt(i);
                    continue;
                }

                TypeLinkedList<ISystem> UpdateSystems = this.updateSystems[component.GetType()];
                if (UpdateSystems == null)
                {
                    updates.RemoveAt(i);
                    continue;
                }

                foreach (ISystem updateSystem in UpdateSystems)
                {
                    try
                    {
                        updateSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(Utility.Text.Format("component: {0}err: {1}", component, e.ToString()));
                    }
                }
            }
        }


    }
}

