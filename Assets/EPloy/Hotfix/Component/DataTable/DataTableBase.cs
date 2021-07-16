using System;

namespace EPloy.Table
{
    /// <summary>
    /// 数据表基类。
    /// </summary>
    public abstract class DataTableBase
    {
        protected string name;
        /// <summary>
        /// 把解析逻辑分出去
        /// </summary>
        protected IDataTableHelper dataTableHelper;

        /// <summary>
        /// 获取数据表名称 考虑到多表的情况 这个不一定时表的名字
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// 获取数据表完整名称。
        /// </summary>
        public string FullName
        {
            get
            {
                return new TypeNamePair(Type, name).ToString();
            }
        }

        /// <summary>
        /// 获取数据表行的类型。
        /// </summary>
        public abstract Type Type
        {
            get;
        }

        /// <summary>
        /// 获取数据表行数。
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// 读取数据表。
        /// </summary>
        /// <param name="dataTableAssetName">数据表资源名称。</param>
        public void ReadData(string dataTableAssetName)
        {
            dataTableHelper.ReadData(dataTableAssetName);
        }

        /// <summary>
        /// 解析数据表。
        /// </summary>
        /// <param name="dataTableBytes">要解析的数据表二进制流。</param>
        /// <returns>是否解析数据表成功。</returns>
        public bool ParseData(byte[] dataTableBytes)
        {
            return dataTableHelper.ParseData(dataTableBytes);
        }

        /// <summary>
        /// 检查是否存在数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>是否存在数据表行。</returns>
        public abstract bool HasDataRow(int id);

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowBytes">要解析的数据表行二进制流。</param>
        /// <param name="startIndex">数据表行二进制流的起始位置。</param>
        /// <param name="length">数据表行二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否增加数据表行成功。</returns>
        public abstract bool AddDataRow(byte[] dataRowBytes, int startIndex, int length);

        /// <summary>
        /// 移除指定数据表行。
        /// </summary>
        /// <param name="id">要移除数据表行的编号。</param>
        /// <returns>是否移除数据表行成功。</returns>
        public abstract bool RemoveDataRow(int id);

        /// <summary>
        /// 清空所有数据表行。
        /// </summary>
        public abstract void RemoveAllDataRows();

        /// <summary>
        /// 关闭并清理数据表。
        /// </summary>
        internal abstract void OnDestroy();
    }
}
