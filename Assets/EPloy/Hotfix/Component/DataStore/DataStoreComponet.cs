using System;
using System.Collections.Generic;
using EPloy.Res;
using EPloy.Table;

namespace EPloy
{
    /// <summary>
    /// 数据仓管组件
    /// </summary>
    public class DataStoreComponet : Component
    {
        private Entity DataStoreEntity;
        //存储所有数据类 后面bool 是否AddComponent到DataStoreEntity 
        private Dictionary<Type, bool> DataStoreTypes;

        public override void Awake()
        {
            DataStoreEntity = GameEntry.Game.CreateEntity("DataStore");
            GetUIFormTypes();
        }

        /// <summary>
        /// 获取数据存储Componet
        /// </summary>
        /// <typeparam name=类型></typeparam>
        /// <returns></returns>
        public T GetDataStore<T>() where T : Component
        {
            Type type = typeof(T);
            if (!DataStoreTypes.ContainsKey(type))
            {
                Log.Error(Utility.Text.Format("{0} class not DataStore", type.ToString()));
                return null;
            }
            bool isInit = DataStoreTypes[type];
            T dataStore;
            if (!isInit)
            {
                dataStore = DataStoreEntity.AddComponent<T>();
                DataStoreTypes[type] = true;
            }
            else
            {
                dataStore = DataStoreEntity.GetComponent<T>();
            }
            return dataStore;
        }

        /// <summary>
        /// 获取所有 DataStoreType
        /// </summary>
        private void GetUIFormTypes()
        {
            DataStoreTypes = new Dictionary<Type, bool>();
            Type[] Types = GameEntry.GameSystem.GetTypes(MuduleConfig.HotFixDllName);
            foreach (Type type in Types)
            {
                object[] objects = type.GetCustomAttributes(typeof(DataStoreAttribute), false);
                if (objects.Length != 0)
                {
                    DataStoreTypes.Add(type, false);
                }
            }
        }

    }
}