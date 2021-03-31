using ETModel;
using GameFramework;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    public static class ExtensionComponentSystem
    {
        public static async ETVoid HotfixAwake(this ExtensionComponent self)
        {
            Type Type = Utility.Assembly.GetType("ETHotfix.HotfixComponent");
            Activator.CreateInstance(Type);
        }

        public static void CreateHotfixComponent(this ExtensionComponent self)
        {
            HotfixComponent.Instance.CreateHotfixComponent();
        }
        public static void AwakeHotfixComponent(this ExtensionComponent self)
        {
            HotfixComponent.Instance.AwakeHotfixComponent();
        }

        public static void UpdateHotfixComponent(this ExtensionComponent self)
        {
            GameEntry.Event.Fire(HotfixUpdateEvent.EventId, ReferencePool.Acquire<HotfixUpdateEvent>());
        }

        public static void LateUpdateHotfixComponent(this ExtensionComponent self)
        {
            GameEntry.Event.Fire(HotfixLateUpdateEvent.EventId, ReferencePool.Acquire<HotfixLateUpdateEvent>());
        }

        public static void FixUpdateHotfixComponent(this ExtensionComponent self)
        {
            
        }

        /// <summary>
        /// 获取Hotfix自定义组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T GetComponent<T>(this ExtensionComponent self) where T : Component
        {
            return HotfixComponent.Instance.GetComponent<T>();
        }
    }
}