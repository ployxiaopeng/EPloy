using System;
using System.Collections.Generic;


/// <summary>
/// 程序集相关的实用函数。
/// </summary>
public static class UtilAssembly
{
    private static readonly System.Reflection.Assembly[] s_Assemblies = null;
    private static readonly Dictionary<string, Type> s_CachedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);

    static UtilAssembly()
    {
        s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    /// <summary>
    /// 获取已加载的程序集。
    /// </summary>
    /// <returns>已加载的程序集。</returns>
    public static System.Reflection.Assembly[] GetAssemblies()
    {
        return s_Assemblies;
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型。
    /// </summary>
    /// <returns>已加载的程序集中的所有类型。</returns>
    public static Type[] GetTypes()
    {
        List<Type> results = new List<Type>();
        foreach (System.Reflection.Assembly assembly in s_Assemblies)
        {
            results.AddRange(assembly.GetTypes());
        }

        return results.ToArray();
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型。
    /// </summary>
    /// <param name="results">已加载的程序集中的所有类型。</param>
    public static void GetTypes(List<Type> results)
    {
        if (results == null)
        {
            Log.Fatal("Results is invalid.");
            return;
        }

        results.Clear();
        foreach (System.Reflection.Assembly assembly in s_Assemblies)
        {
            results.AddRange(assembly.GetTypes());
        }
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型。
    /// </summary>
    /// <param name="typeName">要获取的类型名。</param>
    /// <returns>已加载的程序集中的指定类型。</returns>
    public static Type GetType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            Log.Fatal("Type name is invalid.");
            return default(Type);
        }

        Type type = null;
        if (s_CachedTypes.TryGetValue(typeName, out type))
        {
            return type;
        }

        type = Type.GetType(typeName);
        if (type != null)
        {
            s_CachedTypes.Add(typeName, type);
            return type;
        }

        foreach (System.Reflection.Assembly assembly in s_Assemblies)
        {
            type = Type.GetType(UtilText.Format("{0}, {1}", typeName, assembly.FullName));
            if (type != null)
            {
                s_CachedTypes.Add(typeName, type);
                return type;
            }
        }

        return null;
    }
}
