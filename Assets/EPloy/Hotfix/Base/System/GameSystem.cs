using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    ///  全局唯一游戏系统
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
        private readonly Queue<EPloyAction> starts = new Queue<EPloyAction>();
        private readonly List<long> updates = new List<long>();

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
            this.updates.Clear();
            this.starts.Clear();
        }

        public void Awake(Component component)
        {
            LifeCycleCheck lifeCycleCheck = CheckLifeCycle(component);
            if (lifeCycleCheck.isAwake)
            {
                try
                {
                    component.Awake();
                }
                catch (Exception e)
                {
                    Log.Fatal(e.ToString());
                }
            }

            if (lifeCycleCheck.isStart)
            {
                this.starts.Enqueue(component.Start);
            }

            if (lifeCycleCheck.isUpdate)
            {
                this.updates.Add(component.InstanceId);
            }
        }

        private void Start()
        {
            while (this.starts.Count > 0)
            {
                EPloyAction start = this.starts.Dequeue();
                try
                {
                    start();
                }
                catch (Exception e)
                {
                    Log.Fatal(e.ToString());
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
                try
                {
                    component.Update();
                }
                catch (Exception e)
                {
                    Log.Error(Utility.Text.Format("component: {0}err: {1}", component, e.ToString()));
                }
            }
        }

        /// <summary>
        /// 判断组件中生命周期
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private LifeCycleCheck CheckLifeCycle(Component component)
        {
            LifeCycleCheck lifeCycleCheck = ReferencePool.Acquire<LifeCycleCheck>();
            Type type = component.GetType();
            foreach (MemberInfo m in type.GetMembers())
            {
                if (!lifeCycleCheck.isAwake && m.Name == LifeCycle.Awake.ToString())
                {
                    lifeCycleCheck.isAwake = true;
                    continue;
                }
                if (!lifeCycleCheck.isStart && m.Name == LifeCycle.Start.ToString())
                {
                    lifeCycleCheck.isStart = true;
                    continue;
                }
                if (!lifeCycleCheck.isUpdate && m.Name == LifeCycle.Update.ToString())
                {
                    lifeCycleCheck.isUpdate = true;
                    break;
                }
            }
            return lifeCycleCheck;
        }
    }
}