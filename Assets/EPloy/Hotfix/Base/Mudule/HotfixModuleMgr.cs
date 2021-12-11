using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 非热更游戏模块管理
    /// </summary>
    public static class HotfixModuleMgr
    {
        private static readonly Dictionary<Type, IHotfixModule> EPloyModules = new Dictionary<Type, IHotfixModule>();
        private static readonly List<Type> EPloyModuleTypes = new List<Type>();

        public static bool HasModule<T>() where T : IHotfixModule
        {
            return (EPloyModules.ContainsKey(typeof(T)));
        }

        public static T CreateModule<T>() where T : IHotfixModule
        {
            IHotfixModule mudule = (IHotfixModule) Activator.CreateInstance<T>();
            mudule.Awake();
            EPloyModules.Add(typeof(T), mudule);
            EPloyModuleTypes.Add(typeof(T));
            return (T) mudule;
        }

        public static T GetModule<T>() where T : IHotfixModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                return (T) EPloyModules[typeof(T)];
            }

            Log.Fatal(Utility.Text.Format("can not find module : ", typeof(T).ToString()));
            return default(T);
        }

        public static bool RemoveModule<T>() where T : IHotfixModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                IHotfixModule mudule = EPloyModules[typeof(T)];
                EPloyModules.Remove(typeof(T));
                mudule.OnDestroy();
                mudule = null;
                return true;
            }

            Log.Fatal(Utility.Text.Format("can not find module : ", typeof(T).ToString()));
            return false;
        }

        public static void ModuleUpdate()
        {
            for (int i = 0; i < EPloyModuleTypes.Count; i++)
            {
                try
                {
                    EPloyModules[EPloyModuleTypes[i]].Update();
                }
                catch (Exception e)
                {
                    Log.Fatal(Utility.Text.Format("module {0} update err {1} ", EPloyModules[EPloyModuleTypes[i]],
                        e.ToString()));
                }

            }
        }

        public static void ModuleDestory()
        {
            for (int i = 0; i < EPloyModuleTypes.Count; i++)
            {
                try
                {
                    EPloyModules[EPloyModuleTypes[i]].OnDestroy();
                }
                catch (Exception e)
                {
                    Log.Fatal(Utility.Text.Format("module {0} destory err {1} ", EPloyModules[EPloyModuleTypes[i]],
                        e.ToString()));
                }

            }
            EPloyModules.Clear();
            EPloyModuleTypes.Clear();
        }
    }
}
