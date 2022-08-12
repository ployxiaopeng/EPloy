namespace EPloy.Table
{
    /// <summary>
    /// 数据表行接口。
    /// </summary>
    public abstract class IDataRow : IReference
    {
        /// <summary>
        /// 获取数据表行的编号。
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// 解析数据表行。
        /// </summary>
        /// <param name="dataRowBytes">要解析的数据表行二进制流。</param>
        /// <param name="startIndex">数据表行二进制流的起始位置。</param>
        /// <param name="length">数据表行二进制流的长度。</param>
        /// <returns>是否解析数据表行成功。</returns>
        public abstract bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length);

        public void Clear()
        {
        }
    }
}