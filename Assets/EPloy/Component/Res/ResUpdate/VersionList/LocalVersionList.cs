namespace EPloy.Res
{
    /// <summary>
    /// 本地版本资源列表。
    /// </summary>
    public partial struct LocalVersionList
    {
        private static readonly Resource[] EmptyResourceArray = new Resource[] { };

        private readonly bool m_IsValid;
        private readonly Resource[] m_Resources;

        /// <summary>
        /// 初始化本地版本资源列表的新实例。
        /// </summary>
        /// <param name="resources">包含的资源集合。</param>
        /// <param name="fileSystems">包含的文件系统集合。</param>
        public LocalVersionList(Resource[] resources)
        {
            m_IsValid = true;
            m_Resources = resources ?? EmptyResourceArray;
        }

        /// <summary>
        /// 获取本地版本资源列表是否有效。
        /// </summary>
        public bool IsValid
        {
            get
            {
                return m_IsValid;
            }
        }

        /// <summary>
        /// 获取包含的资源集合。
        /// </summary>
        /// <returns>包含的资源集合。</returns>
        public Resource[] GetResources()
        {
            return m_Resources;
        }
    }
}
