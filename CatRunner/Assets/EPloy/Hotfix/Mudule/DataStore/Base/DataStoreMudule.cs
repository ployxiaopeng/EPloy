using EPloy.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 数据仓管组件
    /// </summary>
    public class DataStoreMudule : IHotfixModule
    {
        private Dictionary<Type, DataStoreBase> dataStores;

        public override void Awake()
        {
            dataStores = new Dictionary<Type, DataStoreBase>();
        }

        public override void OnDestroy()
        {
            Type[] keys = dataStores.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                ReferencePool.Release(dataStores[keys[i]]);
            }

            dataStores.Clear();
        }

        /// <summary>
        /// 获取数据存储DataStore
        /// </summary>
        /// <typeparam name=类型></typeparam>
        /// <returns></returns>
        public T GetDataStore<T>() where T : DataStoreBase
        {
            Type type = typeof(T);
            if (!dataStores.ContainsKey(type))
            {
                DataStoreBase dataStore = (DataStoreBase) ReferencePool.Acquire(type);
                dataStore.Create();
                dataStores.Add(type, dataStore);
                return (T) dataStore;
            }
            else
            {
                return (T) dataStores[type];
            }
        }

        /// <summary>
        /// 重置DataStore
        /// </summary>
        /// <typeparam name=类型></typeparam>
        /// <returns></returns>
        public bool ResetDataStore<T>() where T : DataStoreBase
        {
            Type type = typeof(T);
            if (!dataStores.ContainsKey(type))
            {
                Log.Error(UtilText.Format("{0} class not DataStore", type.ToString()));
                return false;
            }

            DataStoreBase dataStore = dataStores[type];
            dataStore.Reset();
            return true;
        }

        /// <summary>
        /// 移除DataStore
        /// </summary>
        /// <typeparam name=类型></typeparam>
        /// <returns></returns>
        public bool RemoveDataStore<T>() where T : DataStoreBase
        {
            Type type = typeof(T);
            if (!dataStores.ContainsKey(type))
            {
                Log.Error(UtilText.Format("{0} class not DataStore", type.ToString()));
                return false;
            }

            DataStoreBase dataStore = dataStores[type];
            dataStores.Remove(type);
            ReferencePool.Release(dataStore);
            return true;
        }
    }
}