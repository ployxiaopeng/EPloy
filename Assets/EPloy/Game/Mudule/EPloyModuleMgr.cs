﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 非热更模块管理
    /// </summary>
    public static class EPloyModuleMgr
    {
        private static Dictionary<Type, EPloyModule> EPloyModules = new Dictionary<Type, EPloyModule>();

        public static bool HasModule<T>() where T : EPloyModule
        {
            return (EPloyModules.ContainsKey(typeof(T)));
        }

        public static T CreateModule<T>() where T : EPloyModule
        {
            EPloyModule mudule = (EPloyModule)Activator.CreateInstance<T>();
            mudule.Awake();
            EPloyModules.Add(typeof(T), mudule);
            return (T)mudule;
        }

        public static T GetModule<T>() where T : EPloyModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                return (T)EPloyModules[typeof(T)];
            }
            Log.Fatal(Utility.Text.Format("can not find module : ", typeof(T).ToString()));
            return null;
        }

        public static bool RemoveModule<T>() where T : EPloyModule
        {
            if (EPloyModules.ContainsKey(typeof(T)))
            {
                EPloyModule mudule = EPloyModules[typeof(T)];
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
            foreach (var module in EPloyModules)
            {
                try
                {
                    module.Value.Update();
                }
                catch (Exception e)
                {
                    Log.Info(Utility.Text.Format("module {0} update err {1} ", e.ToString()));
                }
            }
        }

        public static void ModuleDestory()
        {
            foreach (var module in EPloyModules)
            {
                try
                {
                    module.Value.OnDestroy();
                }
                catch (Exception e)
                {
                    Log.Fatal(Utility.Text.Format("module {0} destory err {1} ", module, e.ToString()));
                }
            }
        }
    }
}
