﻿
using System.Collections.Generic;
using System.Reflection;
using EPloy;

namespace EPloy.Editor
{
    /// <summary>
    /// 类型相关的实用函数。
    /// </summary>
    internal static class Type
    {
        private static readonly string[] EPloyNames =
        {
            "EPloy",
        };

        private static readonly string[] EPloyOrEditorAssemblyNames =
        {
            "EPloy",
            "EPloy.Editor",
        };

        /// <summary>
        /// 获取配置路径。
        /// </summary>
        /// <typeparam name="T">配置类型。</typeparam>
        /// <returns>配置路径。</returns>
        internal static string GetConfigurationPath<T>() where T : ConfigPathAttribute
        {
            // foreach (System.Type type in Utility.Assembly.GetTypes())
            // {
            //     if (!type.IsAbstract || !type.IsSealed)
            //     {
            //         continue;
            //     }

            //     foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            //     {
            //         if (fieldInfo.FieldType == typeof(string) && fieldInfo.IsDefined(typeof(T), false))
            //         {
            //             return (string)fieldInfo.GetValue(null);
            //         }
            //     }

            //     foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            //     {
            //         if (propertyInfo.PropertyType == typeof(string) && propertyInfo.IsDefined(typeof(T), false))
            //         {
            //             return (string)propertyInfo.GetValue(null, null);
            //         }
            //     }
            // }

            return null;
        }

        /// <summary>
        /// 在运行时程序集中获取指定基类的所有子类的名称。
        /// </summary>
        /// <param name="typeBase">基类类型。</param>
        /// <returns>指定基类的所有子类的名称。</returns>
        internal static string[] GetRuntimeTypeNames(System.Type typeBase)
        {
            return GetTypeNames(typeBase, EPloyNames);
        }

        /// <summary>
        /// 在运行时或编辑器程序集中获取指定基类的所有子类的名称。
        /// </summary>
        /// <param name="typeBase">基类类型。</param>
        /// <returns>指定基类的所有子类的名称。</returns>
        internal static string[] GetRuntimeOrEditorTypeNames(System.Type typeBase)
        {
            return GetTypeNames(typeBase, EPloyOrEditorAssemblyNames);
        }

        private static string[] GetTypeNames(System.Type typeBase, string[] assemblyNames)
        {
            List<string> typeNames = new List<string>();
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    continue;
                }

                if (assembly == null)
                {
                    continue;
                }

                System.Type[] types = assembly.GetTypes();
                foreach (System.Type type in types)
                {
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }
    }
}
