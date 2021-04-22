
namespace EPloy.Res
{
    /// <summary>
    /// 具体要加载资源的详细信息
    /// </summary>
    public sealed class ResInfo
    {
        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public ResName ResName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源是否使用文件系统。
        /// </summary>
        public bool UseFileSystem
        {
            get
            {
                return !string.IsNullOrEmpty(FileSystemName);
            }
        }

        /// <summary>
        /// 获取文件系统名称。
        /// </summary>
        public string FileSystemName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源是否通过二进制方式加载。
        /// </summary>
        public bool IsLoadFromBinary
        {
            get
            {
                return LoadType == LoadType.LoadFromBinary;
            }
        }

        /// <summary>
        /// 获取资源加载方式。
        /// </summary>
        public LoadType LoadType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源大小。
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源哈希值。
        /// </summary>
        public int HashCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源是否准备完毕。
        /// </summary>
        public bool Ready
        {
            get;
            private set;
        }

        /// <summary>
        /// 标记资源准备完毕。
        /// </summary>
        public void MarkReady()
        {
            Ready = true;
        }
        /// <summary>
        /// 初始化资源信息的新实例。
        /// </summary>
        /// <param name="resourceName">资源名称。</param>
        /// <param name="fileSystemName">文件系统名称。</param>
        /// <param name="loadType">资源加载方式。</param>
        /// <param name="length">资源大小。</param>
        /// <param name="hashCode">资源哈希值。</param>
        /// <param name="ready">资源是否准备完毕。</param>
        public ResInfo(ResName resName, string fileSystemName, LoadType loadType, int length, int hashCode, bool ready)
        {
            ResName = resName;
            FileSystemName = fileSystemName;
            LoadType = loadType;
            Length = length;
            HashCode = hashCode;
            Ready = ready;
        }
    }
}
