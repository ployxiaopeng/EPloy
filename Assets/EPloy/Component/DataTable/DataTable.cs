
using System;
using System.Collections;
using System.Collections.Generic;


namespace EPloy
{

    /// <summary>
    /// 数据表。
    /// </summary>
    /// <typeparam name="T">数据表行的类型。</typeparam>
    public sealed class DataTable<T> : DataTableBase, IDataTable<T> where T : class, IDataRow, new()
    {
        private readonly Dictionary<int, T> DataSet;

        /// <summary>
        /// 初始化数据表的新实例。
        /// </summary>
        /// <param name="name">数据表名称。</param>
        public DataTable(string name)
            : base(name)
        {
            DataSet = new Dictionary<int, T>();
        }

        /// <summary>
        /// 获取数据表行的类型。
        /// </summary>
        public override Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 获取数据表行数。
        /// </summary>
        public override int Count
        {
            get
            {
                return DataSet.Count;
            }
        }

        /// <summary>
        /// 获取数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>数据表行。</returns>
        public T this[int id]
        {
            get
            {
                return GetDataRow(id);
            }
        }

        /// <summary>
        /// 检查是否存在数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>是否存在数据表行。</returns>
        public override bool HasDataRow(int id)
        {
            return DataSet.ContainsKey(id);
        }

        /// <summary>
        /// 检查是否存在数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <returns>是否存在数据表行。</returns>
        public bool HasDataRow(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new EPloyException("Condition is invalid.");
            }

            foreach (KeyValuePair<int, T> dataRow in DataSet)
            {
                if (condition(dataRow.Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>数据表行。</returns>
        public T GetDataRow(int id)
        {
            T dataRow = null;
            if (DataSet.TryGetValue(id, out dataRow))
            {
                return dataRow;
            }

            return dataRow;
        }

        /// <summary>
        /// 获取符合条件的数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <returns>符合条件的数据表行。</returns>
        /// <remarks>当存在多个符合条件的数据表行时，仅返回第一个符合条件的数据表行。</remarks>
        public T GetDataRow(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new EPloyException("Condition is invalid.");
            }

            foreach (KeyValuePair<int, T> dataRow in DataSet)
            {
                if (condition(dataRow.Value))
                {
                    return dataRow.Value;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 获取符合条件的数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <returns>符合条件的数据表行。</returns>
        public T[] GetDataRows(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new EPloyException("Condition is invalid.");
            }

            List<T> results = new List<T>();
            foreach (KeyValuePair<int, T> dataRow in DataSet)
            {
                if (condition(dataRow.Value))
                {
                    results.Add(dataRow.Value);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取所有数据表行。
        /// </summary>
        /// <returns>所有数据表行。</returns>
        public T[] GetAllDataRows()
        {
            int index = 0;
            T[] results = new T[DataSet.Count];
            foreach (KeyValuePair<int, T> dataRow in DataSet)
            {
                results[index++] = dataRow.Value;
            }

            return results;
        }

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowBytes">要解析的数据表行二进制流。</param>
        /// <param name="startIndex">数据表行二进制流的起始位置。</param>
        /// <param name="length">数据表行二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否增加数据表行成功。</returns>
        public override bool AddDataRow(byte[] dataRowBytes, int startIndex, int length)
        {
            try
            {
                T dataRow = new T();
                if (!dataRow.ParseDataRow(dataRowBytes, startIndex, length))
                {
                    return false;
                }

                InternalAddDataRow(dataRow);
                return true;
            }
            catch (Exception exception)
            {
                if (exception is EPloyException)
                {
                    throw;
                }

                throw new EPloyException(string.Format("Can not parse data row bytes for data table '{0}' with exception '{1}'.", new TypeNamePair(typeof(T), Name).ToString(), exception.ToString()), exception);
            }
        }

        /// <summary>
        /// 移除指定数据表行。
        /// </summary>
        /// <param name="id">要移除数据表行的编号。</param>
        /// <returns>是否移除数据表行成功。</returns>
        public override bool RemoveDataRow(int id)
        {
            if (!HasDataRow(id))
            {
                return false;
            }

            if (!DataSet.Remove(id))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清空所有数据表行。
        /// </summary>
        public override void RemoveAllDataRows()
        {
            DataSet.Clear();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return DataSet.Values.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return DataSet.Values.GetEnumerator();
        }
        internal override void OnDestroy()
        {
            DataSet.Clear();
        }

        private void InternalAddDataRow(T dataRow)
        {
            if (HasDataRow(dataRow.Id))
            {
                throw new EPloyException(string.Format("Already exist '{0}' in data table '{1}'.", dataRow.Id.ToString(), new TypeNamePair(typeof(T), Name).ToString()));
            }

            DataSet.Add(dataRow.Id, dataRow);
        }
    }
}