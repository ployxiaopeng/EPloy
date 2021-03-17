
namespace EPloy.Res
{
    /// <summary>
    /// 具体要加载资源的详细信息
    /// </summary>
    internal sealed class ResInfo
    {
        private readonly ResName m_ResName;
        private readonly LoadType m_LoadType;
        private readonly int m_Length;
        private readonly int m_HashCode;
        private bool m_Ready;

        /// <summary>
        /// 初始化资源信息的新实例。
        /// </summary>
        /// <param name="resourceName">资源名称。</param>
        /// <param name="fileSystemName">文件系统名称。</param>
        /// <param name="loadType">资源加载方式。</param>
        /// <param name="length">资源大小。</param>
        /// <param name="hashCode">资源哈希值。</param>
        /// <param name="ready">资源是否准备完毕。</param>
        public ResInfo(ResName resName, LoadType loadType, int length, int hashCode, bool ready)
        {
            m_ResName = resName;
            m_LoadType = loadType;
            m_Length = length;
            m_HashCode = hashCode;
            m_Ready = ready;
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public ResName ResName
        {
            get
            {
                return m_ResName;
            }
        }

        /// <summary>
        /// 获取资源是否通过二进制方式加载。
        /// </summary>
        public bool IsLoadFromBinary
        {
            get
            {
                return m_LoadType == LoadType.LoadFromBinary;
            }
        }

        /// <summary>
        /// 获取资源加载方式。
        /// </summary>
        public LoadType LoadType
        {
            get
            {
                return m_LoadType;
            }
        }

        /// <summary>
        /// 获取资源大小。
        /// </summary>
        public int Length
        {
            get
            {
                return m_Length;
            }
        }

        /// <summary>
        /// 获取资源哈希值。
        /// </summary>
        public int HashCode
        {
            get
            {
                return m_HashCode;
            }
        }

        /// <summary>
        /// 获取资源是否准备完毕。
        /// </summary>
        public bool Ready
        {
            get
            {
                return m_Ready;
            }
        }

        /// <summary>
        /// 标记资源准备完毕。
        /// </summary>
        public void MarkReady()
        {
            m_Ready = true;
        }
    }
}

