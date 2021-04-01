
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EPloy.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private static class DataProcessorUtility
        {
            private static readonly IDictionary<string, DataProcessor> s_DataProcessors = new SortedDictionary<string, DataProcessor>();

            static DataProcessorUtility()
            {
                System.Type dataProcessorBaseType = typeof(DataProcessor);
                Assembly assembly = Assembly.GetExecutingAssembly();
                System.Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (!types[i].IsClass || types[i].IsAbstract)
                    {
                        continue;
                    }

                    if (dataProcessorBaseType.IsAssignableFrom(types[i]))
                    {
                        DataProcessor dataProcessor = (DataProcessor)Activator.CreateInstance(types[i]);
                        foreach (string typeString in dataProcessor.GetTypeStrings())
                        {
                            s_DataProcessors.Add(typeString.ToLower(), dataProcessor);
                        }
                    }
                }
            }

            public static DataProcessor GetDataProcessor(string type)
            {
                if (type == null)
                {
                    type = string.Empty;
                }

                DataProcessor dataProcessor = null;
                if (s_DataProcessors.TryGetValue(type.ToLower(), out dataProcessor))
                {
                    return dataProcessor;
                }

                Log.Fatal(Utility.Text.Format("Not supported data processor type '{0}'.", type));
                return null;
            }
        }
    }
}
