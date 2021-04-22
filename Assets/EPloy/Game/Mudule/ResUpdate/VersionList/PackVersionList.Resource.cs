namespace EPloy.Res
{
    public partial struct PackVersionList
    {
        /// <summary>
        /// 资源。
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
            /// 获取资源偏移。
            /// </summary>
            public long Offset
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
            /// 获取资源压缩后长度。
            /// </summary>
            public int ZipLength
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源压缩后哈希值。
            /// </summary>
            public int ZipHashCode
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
            /// <param name="offset">资源偏移。</param>
            /// <param name="length">资源长度。</param>
            /// <param name="hashCode">资源哈希值。</param>
            /// <param name="zipLength">资源压缩后长度。</param>
            /// <param name="zipHashCode">资源压缩后哈希值。</param>
            public Resource(string name, string variant, string extension, byte loadType, long offset, int length, int hashCode, int zipLength, int zipHashCode)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Log.Fatal("Name is invalid.");
                }

                Name = name;
                Variant = variant;
                Extension = extension;
                LoadType = loadType;
                Offset = offset;
                Length = length;
                HashCode = hashCode;
                ZipLength = zipLength;
                ZipHashCode = zipHashCode;
            }

        }
    }
}
