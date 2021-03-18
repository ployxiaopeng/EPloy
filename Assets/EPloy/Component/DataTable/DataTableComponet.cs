
using System;
using System.Collections.Generic;
using EPloy.Res;
using EPloy.Table;

namespace EPloy
{
    /// <summary>
    /// 数据表管理器。 TODO 分表处理
    /// </summary>
    public partial class DataTableComponet : Component
    {
        private readonly Dictionary<TypeNamePair, DataTableBase> DataTables;
        private ResComponent Res
        {
            get
            {
                return GameEntry.Res;
            }
        }
    
        /// <summary>
        /// 初始化数据表管理器的新实例。
        /// </summary>
        public DataTableComponet()
        {
            DataTables = new Dictionary<TypeNamePair, DataTableBase>();
        }

        /// <summary>
        /// 获取数据表数量。
        /// </summary>
        public int Count
        {
            get
            {
                return DataTables.Count;
            }
        }

        /// <summary>
        /// 数据表管理器轮询。
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// 关闭并清理数据表管理器。
        /// </summary>
        public void OnDestroy()
        {
            foreach (KeyValuePair<TypeNamePair, DataTableBase> dataTable in DataTables)
            {
                dataTable.Value.OnDestroy();
            }

            DataTables.Clear();
        }

        /// <summary>
        /// 确保二进制流缓存分配足够大小的内存并缓存。
        /// </summary>
        /// <param name="ensureSize">要确保二进制流缓存分配内存的大小。</param>
        public void EnsureCachedBytesSize(int ensureSize)
        {
            
        }

        /// <summary>
        /// 释放缓存的二进制流。
        /// </summary>
        public void FreeCachedBytes()
        {

        }

        /// <summary>
        /// 是否存在数据表。
        /// </summary>
        /// <typeparam name="T">数据表行的类型。</typeparam>
        /// <returns>是否存在数据表。</returns>
        public bool HasDataTable<T>() where T : IDataRow
        {
            return InternalHasDataTable(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 获取数据表。
        /// </summary>
        /// <typeparam name="T">数据表行的类型。</typeparam>
        /// <returns>要获取的数据表。</returns>
        public IDataTable<T> GetDataTable<T>() where T : IDataRow
        {
            return (IDataTable<T>)InternalGetDataTable(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 获取数据表。
        /// </summary>
        /// <param name="dataRowType">数据表行的类型。</param>
        /// <returns>要获取的数据表。</returns>
        public DataTableBase GetDataTable(Type dataRowType)
        {
            if (dataRowType == null)
            {
                throw new EPloyException("Data row type is invalid.");
            }

            if (!typeof(IDataRow).IsAssignableFrom(dataRowType))
            {
                throw new EPloyException(Utility.Text.Format("Data row type '{0}' is invalid.", dataRowType.FullName));
            }

            return InternalGetDataTable(new TypeNamePair(dataRowType));
        }

        /// <summary>
        /// 获取所有数据表。
        /// </summary>
        /// <returns>所有数据表。</returns>
        public DataTableBase[] GetAllDataTables()
        {
            int index = 0;
            DataTableBase[] results = new DataTableBase[DataTables.Count];
            foreach (KeyValuePair<TypeNamePair, DataTableBase> dataTable in DataTables)
            {
                results[index++] = dataTable.Value;
            }

            return results;
        }

        /// <summary>
        /// 创建数据表。
        /// </summary>
        /// <param name="dataRowType">数据表行的类型。</param>
        /// <param name="name">数据表名称。</param>
        /// <returns>要创建的数据表。</returns>
        public DataTableBase CreateDataTable(Type dataRowType, string name)
        {
            if (dataRowType == null)
            {
                throw new EPloyException("Data row type is invalid.");
            }

            if (!typeof(IDataRow).IsAssignableFrom(dataRowType))
            {
                throw new EPloyException(Utility.Text.Format("Data row type '{0}' is invalid.", dataRowType.FullName));
            }

            TypeNamePair typeNamePair = new TypeNamePair(dataRowType, name);
            if (InternalHasDataTable(typeNamePair))
            {
                throw new EPloyException(Utility.Text.Format("Already exist data table '{0}'.", typeNamePair.ToString()));
            }

            Type dataTableType = typeof(DataTable<>).MakeGenericType(dataRowType);
            DataTableBase dataTable = (DataTableBase)Activator.CreateInstance(dataTableType, name);
            DataTables.Add(typeNamePair, dataTable);
            return dataTable;
        }

        /// <summary>
        /// 加载表格数据
        /// </summary>
        /// <param name="dataTable"></param>
        public void LoadDataTable(Type dataRowType, string assetName)
        {
            TypeNamePair typeNamePair = new TypeNamePair(dataRowType);
            if (InternalHasDataTable(typeNamePair))
            {
                throw new EPloyException(Utility.Text.Format("Data row type '{0}' is invalid.", dataRowType.FullName));
            }
            HasResult result = Res.HasAsset(assetName);
            if (result != HasResult.BinaryOnDisk)
            {
                throw new EPloyException(Utility.Text.Format("{0}  type must be BinaryOnDisk  but is {1}.", assetName, result.ToString()));
            }
            DataTableBase dataTableBase = GetDataTable(dataRowType);
            dataTableBase.ReadData(assetName);
        }

        /// <summary>
        /// 销毁数据表。
        /// </summary>
        /// <typeparam name="T">数据表行的类型。</typeparam>
        public bool DestroyDataTable<T>() where T : IDataRow
        {
            return InternalDestroyDataTable(new TypeNamePair(typeof(T)));
        }

        private bool InternalHasDataTable(TypeNamePair typeNamePair)
        {
            return DataTables.ContainsKey(typeNamePair);
        }

        private DataTableBase InternalGetDataTable(TypeNamePair typeNamePair)
        {
            DataTableBase dataTable = null;
            if (DataTables.TryGetValue(typeNamePair, out dataTable))
            {
                return dataTable;
            }

            return null;
        }

        private bool InternalDestroyDataTable(TypeNamePair typeNamePair)
        {
            DataTableBase dataTable = null;
            if (DataTables.TryGetValue(typeNamePair, out dataTable))
            {
                dataTable.OnDestroy();
                return DataTables.Remove(typeNamePair);
            }

            return false;
        }

        
    }
}
