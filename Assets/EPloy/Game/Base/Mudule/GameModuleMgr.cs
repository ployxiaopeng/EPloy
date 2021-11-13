using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 非热更游戏模块管理
    /// </summary>
    public static class GameModuleMgr
    {
        private static Dictionary<Type, IGameModule> EPloyModules = new Dictionary<Type, IGameModule>();

        public static bool HasModule<T>() where T : IGameModule
        {
            return (EPloyModules.ContainsKey(typeof(T)));
        }

        public static T CreateModule<T>() where T : IGameModule
        {
            IGameModule mudule = (IGameModule) Activator.CreateInstance<T>();
            mudule.Awake();
            EPloyModules.Add(typeof(T), mudule);
            return (T) mudule;
        }

        public static T GetModule<T>() where T : IGameModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                return (T) EPloyModules[typeof(T)];
            }

            Log.Fatal(Utility.Text.Format("can not find module : ", typeof(T).ToString()));
            return default(T);
        }

        public static bool RemoveModule<T>() where T : IGameModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                IGameModule mudule = EPloyModules[typeof(T)];
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
            foreach (var module in EPloyModules.Values)
            {
                try
                {
                    module.Update();
                }
                catch (Exception e)
                {
                    Log.Info(Utility.Text.Format("module {0} update err {1} ", module, e.ToString()));
                }
            }
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
                    Log.Fatal(Utility.Text.Format("module {0} destory err {1} ", module, e.ToString()));
                }
            }

            EPloyModules.Clear();
        }
    }
}
