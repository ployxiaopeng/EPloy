using EPloy.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 非热更游戏模块管理
    /// </summary>
    public static class HotfixModuleMgr
    {
        private static readonly Dictionary<Type, IHotfixModule> EPloyModules = new Dictionary<Type, IHotfixModule>();

        public static bool HasModule<T>() where T : IHotfixModule
        {
            return (EPloyModules.ContainsKey(typeof(T)));
        }

        public static T CreateModule<T>() where T : IHotfixModule
        {
            IHotfixModule mudule = (IHotfixModule)Activator.CreateInstance<T>();
            mudule.Awake();
            EPloyModules.Add(typeof(T), mudule);
            return (T)mudule;
        }

        public static T GetModule<T>() where T : IHotfixModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                return (T)EPloyModules[typeof(T)];
            }

            Log.Fatal(UtilText.Format("can not find module : ", typeof(T).ToString()));
            return default(T);
        }

        public static bool RemoveModule<T>() where T : IHotfixModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                IHotfixModule mudule = EPloyModules[typeof(T)];
                EPloyModules.Remove(typeof(T));
                mudule.OnDestroy();
                return true;
            }

            Log.Fatal(UtilText.Format("can not find module : ", typeof(T).ToString()));
            return false;
        }

        public static void ModuleDestory()
        {
            foreach (var module in EPloyModules.Values)
            {
                try
                {
                    module.OnDestroy();
                }
                catch (Exception e)
                {
                    Log.Fatal(UtilText.Format("module {0} destory err {1} ", module,
                        e.ToString()));
                }
            }
            EPloyModules.Clear();
        }
    }
}
