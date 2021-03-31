using GameFramework;
using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class DataTableComponentAwakeSystem : AwakeSystem<DataTableComponent>
    {
        public override void Awake(DataTableComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class DataTableComponentStartSystem : StartSystem<DataTableComponent>
    {
        public override void Start(DataTableComponent self)
        {
            self.Start();
        }
    }
    public static class DataTableComponentSystem
    {
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void Awake(this DataTableComponent self)
        {
            self.DataTableManager = GameFrameworkEntry.GetModule<IDataTableManager>();
            if (self.DataTableManager == null)
            {
                Log.Fatal("Data table manager is invalid.");
                return;
            }
        }

        public static void Start(this DataTableComponent self)
        {
            self.DataTableManager.SetResourceManager(Init.Resource.ResourceManager);
            self.DataTableManager.SetDataTableHelper(new DataTableHelper());
        }
    }
}
