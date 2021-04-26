namespace EPloy.Res
{
    public sealed partial class UpdaterHandler
    {
        /// <summary>
        /// 更新信息。
        /// </summary>
        private sealed class UpdateInfo
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


            /// <summary>
            /// 获取或设置已重试次数。
            /// </summary>
            public int RetryCount
            {
                get;
                set;
            }

            /// <summary>
            /// 初始化更新信息的新实例。
            /// </summary>
            /// <param name="resName">资源名称。</param>
            /// <param name="fileSystemName">资源所在的文件系统名称。</param>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="length">资源大小。</param>
            /// <param name="hashCode">资源哈希值。</param>
            /// <param name="zipLength">压缩后大小。</param>
            /// <param name="zipHashCode">压缩后哈希值。</param>
            /// <param name="resPath">资源路径。</param>
            public UpdateInfo(ResName resName, string fileSystemName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode, string resPath)
            {
                ResName = resName;
                FileSystemName = fileSystemName;
                LoadType = loadType;
                Length = length;
                HashCode = hashCode;
                ZipLength = zipLength;
                ZipHashCode = zipHashCode;
                ResPath = resPath;
                RetryCount = 0;
            }

        }
    }
}
