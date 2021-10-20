using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    ///  游戏系统
    /// </summary>
    public sealed class GameSystem
    {
        private Dictionary<long, Component> allComponents
        {
            get { return HotFixMudule.GameEntity.WithIdComponent.AllComponents; }
        }

        private readonly Dictionary<string, Type[]> assemblies = new Dictionary<string, Type[]>();
        private readonly Queue<EPloyAction> starts = new Queue<EPloyAction>();
        private readonly List<long> updates = new List<long>();

        public Type[] GetTypes(string dllType)
        {
            return this.assemblies[dllType];
        }
        public void Add(string dllType, Type[] assembly)
        {
            this.assemblies[dllType] = assembly;
            this.updates.Clear();
            this.starts.Clear();
        }

        public void Awake(Component component)
        {
            try
            {
                component.Awake();
            }
            catch (Exception e)
            {
                Log.Fatal(e.ToString());
            }


            // if (lifeCycleCheck.isStart)
            // {
            //     // this.starts.Enqueue(component.Start);
            // }
            //
            // if (lifeCycleCheck.isUpdate)
            // {
            //     this.updates.Add(component.InstanceId);
            // }
        }
        
        public void Update()
        {
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
                   // component.Update();
                }
                catch (Exception e)
                {
                    Log.Error(Utility.Text.Format("component: {0}  err: {1}", component, e.ToString()));
                }
            }
        }
        
    }
}