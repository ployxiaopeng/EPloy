﻿namespace EPloy.Res
{
    public partial struct UpdatableVersionList
    {
        /// <summary>
        /// 资源。
        /// </summary>
        public struct Resource
        {
            private static readonly int[] EmptyIntArray = new int[] { };

            private readonly string m_Name;
            private readonly byte m_LoadType;
            private readonly int m_Length;
            private readonly int m_HashCode;
            private readonly int m_ZipLength;
            private readonly int m_ZipHashCode;
            private readonly int[] m_AssetIndexes;

            /// <summary>
            /// 初始化资源的新实例。
            /// </summary>
            /// <param name="name">资源名称。</param>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="length">资源长度。</param>
            /// <param name="hashCode">资源哈希值。</param>
            /// <param name="zipLength">资源压缩后长度。</param>
            /// <param name="zipHashCode">资源压缩后哈希值。</param>
            /// <param name="assetIndexes">资源包含的资源索引集合。</param>
            public Resource(string name, byte loadType, int length, int hashCode, int zipLength, int zipHashCode, int[] assetIndexes)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Log.Fatal("Name is invalid.");
                }

                m_Name = name;
                m_LoadType = loadType;
                m_Length = length;
                m_HashCode = hashCode;
                m_ZipLength = zipLength;
                m_ZipHashCode = zipHashCode;
                m_AssetIndexes = assetIndexes ?? EmptyIntArray;
            }

            /// <summary>
            /// 获取资源名称。
            /// </summary>
            public string Name
            {
                get
                {
                    return m_Name;
                }
            }

            /// <summary>
            /// 获取资源加载方式。
            /// </summary>
            public byte LoadType
            {
                get
                {
                    return m_LoadType;
                }
            }

            /// <summary>
            /// 获取资源长度。
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
            /// 获取资源压缩后长度。
            /// </summary>
            public int ZipLength
            {
                get
                {
                    return m_ZipLength;
                }
            }

            /// <summary>
            /// 获取资源压缩后哈希值。
            /// </summary>
            public int ZipHashCode
            {
                get
                {
                    return m_ZipHashCode;
                }
            }

            /// <summary>
            /// 获取资源包含的资源索引集合。
            /// </summary>
            /// <returns>资源包含的资源索引集合。</returns>
            public int[] GetAssetIndexes()
            {
                return m_AssetIndexes;
            }
        }
    }
}