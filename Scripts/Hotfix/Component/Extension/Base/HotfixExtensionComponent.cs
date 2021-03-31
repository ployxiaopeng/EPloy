using ETModel;
using GameFramework;
using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 热更层组件管理
    /// </summary>
    public class HotfixComponent
    {
        public static HotfixComponent Instance = null;
        public HotfixComponent()
        {
            if (Instance == null) Instance = this;
            else
            {
                Log.Error("HotfixComponent is Exist");
                return;
            }
            HotfixComponentDic = new Dictionary<Type, Component>();

            GameEntry.Event.Subscribe(HotfixUpdateEvent.EventId, UpdateHotfixComponent);
            GameEntry.Event.Subscribe(HotfixLateUpdateEvent.EventId, LateUpdateHotfixComponent);
        }

        public void CreateHotfixComponent()
        {
            foreach (Type type in GameEntry.ILRuntime.GetHotfixTypes)
            {
                object[] attrs = type.GetCustomAttributes(typeof(HotfixExtensionAttribute), false);
                if (attrs.Length == 0) continue;
                Type tType = Utility.Assembly.GetType(type.FullName);
                SetComponent((Component)Activator.CreateInstance(tType));
            }
        }

        private Dictionary<Type, Component> HotfixComponentDic;
        private void SetComponent(Component component)
        {
            Type type = component.GetType();
            if (HotfixComponentDic.ContainsKey(type)) return;
            HotfixComponentDic.Add(type, component);
        }
        public void AwakeHotfixComponent()
        {
            foreach (var component in HotfixComponentDic)
                component.Value.Awake();
            //开启轮转
            GameEntry.ILRuntime.OpenHotfixUpdate = true;
        }

        public void UpdateHotfixComponent(object sender, GameEventArgs args )
        {
            foreach (var component in HotfixComponentDic)
                component.Value.Update();
        }

        public void LateUpdateHotfixComponent(object sender, GameEventArgs args)
        {
            foreach (var component in HotfixComponentDic)
                component.Value.LateUpdate();
        }

        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (!HotfixComponentDic.ContainsKey(type)) return default(T);
            return (T)HotfixComponentDic[type];
        }
    }
}