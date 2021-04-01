namespace EPloy.Res
{
    /// <summary>
    /// 本地版本资源列表。
    /// </summary>
    public struct LocalVersionList
    {
        private static readonly Resource[] EmptyResourceArray = new Resource[] { };
        private static readonly FileSystem[] EmptyFileSystemArray = new FileSystem[] { };

        /// <summary>
        /// 获取本地版本资源列表是否有效。
        /// </summary>
        public bool IsValid
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
        /// 获取包含的文件系统集合。
        /// </summary>
        /// <returns>包含的文件系统集合。</returns>
        public FileSystem[] FileSystems
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化本地版本资源列表的新实例。
        /// </summary>
        /// <param name="resources">包含的资源集合。</param>
        /// <param name="fileSystems">包含的文件系统集合。</param>
        public LocalVersionList(Resource[] resources, FileSystem[] fileSystems)
        {
            IsValid = true;
            Resources = resources ?? EmptyResourceArray;
            FileSystems = fileSystems ?? EmptyFileSystemArray;
        }

        /// <summary>
        /// 本地版本资源信息。
        /// </summary>
        public struct Resource
        {
            /// <summary>
            /// 获取资源名称。
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源变体名称。
            /// </summary>
            public string Variant
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源扩展名称。
            /// </summary>
            public string Extension
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源加载方式。
            /// </summary>
            public byte LoadType
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源长度。
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
            /// 初始化资源的新实例。
            /// </summary>
            /// <param name="name">资源名称。</param>
            /// <param name="variant">资源变体名称。</param>
            /// <param name="extension">资源扩展名称。</param>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="length">资源长度。</param>
            /// <param name="hashCode">资源哈希值。</param>
            public Resource(string name, string variant, string extension, byte loadType, int length, int hashCode)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Log.Fatal("Name is invalid.");
                }

                Name = name;
                Variant = variant;
                Extension = extension;
                LoadType = loadType;
                Length = length;
                HashCode = hashCode;
            }

        }

        /// <summary>
        /// 本地版文件系统信息。
        /// </summary>
        public struct FileSystem
        {
            private static readonly int[] EmptyIntArray = new int[] { };

            /// <summary>
            /// 获取文件系统名称。
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取文件系统包含的资源索引集合。
            /// </summary>
            /// <returns>文件系统包含的资源索引集合。</returns>
            public int[] ResourceIndexes
            {
                get;
                private set;
            }

            /// <summary>
            /// 初始化文件系统的新实例。
            /// </summary>
            /// <param name="name">文件系统名称。</param>
            /// <param name="resourceIndexes">文件系统包含的资源索引集合。</param>
            public FileSystem(string name, int[] resourceIndexes)
            {
                if (name == null)
                {
                     Log.Fatal("Name is invalid.");
                }

                Name = name;
                ResourceIndexes = resourceIndexes ?? EmptyIntArray;
            }
        }
    }
}
