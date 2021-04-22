namespace EPloy.Res
{

    public sealed partial class ResUpdater
    {
        /// <summary>
        /// 应用信息。
        /// </summary>
        private sealed class ApplyInfo
        {

            /// <summary>
            /// 初始化应用信息的新实例。
            /// </summary>
            /// <param name="resourceName">资源名称。</param>
            /// <param name="fileSystemName">资源所在的文件系统名称。</param>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="offset">资源偏移。</param>
            /// <param name="length">资源大小。</param>
            /// <param name="hashCode">资源哈希值。</param>
            /// <param name="zipLength">压缩后大小。</param>
            /// <param name="zipHashCode">压缩后哈希值。</param>
            /// <param name="resourcePath">资源路径。</param>
            public ApplyInfo(ResName resName, string fileSystemName, LoadType loadType, long offset, int length, int hashCode, int zipLength, int zipHashCode, string resPath)
            {
                ResName = resName;
                FileSystemName = fileSystemName;
                LoadType = loadType;
                Offset = offset;
                Length = length;
                HashCode = hashCode;
                ZipLength = zipLength;
                ZipHashCode = zipHashCode;
                ResPath = resPath;
            }

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
            /// 获取资源所在的文件系统名称。
            /// </summary>
            public string FileSystemName
            {
                get;
                private set;
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
            /// 获取资源偏移。
            /// </summary>
            public long Offset
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
            /// 获取压缩后大小。
            /// </summary>
            public int ZipLength
            {
                get;
                private set;
            }


            /// <summary>
            /// 获取压缩后哈希值。
            /// </summary>
            public int ZipHashCode
            {
                get;
                private set;
            }


            /// <summary>
            /// 获取资源路径。
            /// </summary>
            public string ResPath
            {
                get;
                private set;
            }
        }
    }
}
