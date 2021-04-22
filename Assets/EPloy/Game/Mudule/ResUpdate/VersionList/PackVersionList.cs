namespace EPloy.Res
{
    /// <summary>
    /// 资源包版本资源列表。
    /// </summary>
    public partial struct PackVersionList
    {
        private static readonly Resource[] EmptyResourceArray = new Resource[] { };

        /// <summary>
        /// 获取资源包版本资源列表是否有效。
        /// </summary>
        public bool IsValid
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源数据偏移。
        /// </summary>
        public int Offset
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源数据长度。
        /// </summary>
        public long Length
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源数据哈希值。
        /// </summary>
        public int HashCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取包含的资源集合。
        /// </summary>
        /// <returns>包含的资源集合。</returns>
        public Resource[] Resources
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化资源包版本资源列表的新实例。
        /// </summary>
        /// <param name="offset">资源数据偏移。</param>
        /// <param name="length">资源数据长度。</param>
        /// <param name="hashCode">资源数据哈希值。</param>
        /// <param name="resources">包含的资源集合。</param>
        public PackVersionList(int offset, long length, int hashCode, Resource[] resources)
        {
            IsValid = true;
            Offset = offset;
            Length = length;
            HashCode = hashCode;
            Resources = resources ?? EmptyResourceArray;
        }
    }
}
