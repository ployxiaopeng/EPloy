using System;
using System.Collections.Generic;

    public enum GameModel
    {
        Editor,//编辑器模式
        EditorHotfix,//编辑器ILRuntime模式
        EditorABPack,//编辑器AB包测试模式
        Local,//本地模式
        HotFix,//热更模式
    }

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
        IGameModule mudule = (IGameModule)Activator.CreateInstance<T>();
        mudule.Awake();
        EPloyModules.Add(typeof(T), mudule);
        return (T)mudule;
    }

    public static T GetModule<T>() where T : IGameModule
    {
        if (EPloyModules.ContainsKey(typeof(T)))
        {
            return (T)EPloyModules[typeof(T)];
        }

        Log.Fatal(UtilText.Format("can not find module : ", typeof(T).ToString()));
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

        Log.Fatal(UtilText.Format("can not find module : ", typeof(T).ToString()));
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
                Log.Fatal(UtilText.Format("module {0} update err {1} ", module, e.ToString()));
            }
        }
    }

    public static void ModuleDestory()
    {
        try
        {
            foreach (var module in EPloyModules.Values)
            {
                try
                {
                    module.OnDestroy();
                }
                catch (Exception e)
                {
                    Log.Fatal(UtilText.Format("module {0} destory err {1} ", module, e.ToString()));
                }
            }

            EPloyModules.Clear();
        }
        catch (Exception e)
        {
            Log.Fatal(e.ToString());
        }
    }

    public static void ModuleDestory<T>() where T : IGameModule
    {
        if (EPloyModules.ContainsKey(typeof(T)))
        {
            IGameModule module = EPloyModules[typeof(T)];
            module.OnDestroy();
            EPloyModules.Remove(typeof(T));
            return;
        }

        Log.Fatal(UtilText.Format("module has been destory {0} ", typeof(T).ToString()));
    }
}
