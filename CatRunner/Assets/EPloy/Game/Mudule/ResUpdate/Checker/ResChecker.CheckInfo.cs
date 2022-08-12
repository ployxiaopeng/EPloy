namespace EPloy.Game.Res
{
    internal sealed partial class ResChecker
    {
        /// <summary>
        /// 资源检查信息。
        /// </summary>
        public sealed partial class CheckInfo
        {
            private RemoteVersionInfo VersionInfo;
            private LocalVersionInfo ReadWriteInfo;

            /// <summary>
            /// 获取资源名称。
            /// </summary>
            public ResName ResName
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源检查状态。
            /// </summary>
            public CheckStatus Status
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取是否需要移除读写区的资源。
            /// </summary>
            public bool NeedRemove
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取是否需要将读写区的资源移动到磁盘。
            /// </summary>
            public bool NeedMoveToDisk
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取是否需要将读写区的资源移动到文件系统。
            /// </summary>
            public bool NeedMoveToFileSystem
            {
                get;
                private set;
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
            /// 获取资源是否使用文件系统。
            /// </summary>
            public bool ReadWriteUseFileSystem
            {
                get
                {
                    return ReadWriteInfo.UseFileSystem;
                }
            }

            /// <summary>
            /// 获取读写资源所在的文件系统名称。
            /// </summary>
            public string ReadWriteFileSystemName
            {
                get
                {
                    return ReadWriteInfo.FileSystemName;
                }
            }

            /// <summary>
            /// 获取资源加载方式。
            /// </summary>
            public LoadType LoadType
            {
                get
                {
                    return VersionInfo.LoadType;
                }
            }

            /// <summary>
            /// 获取资源大小。
            /// </summary>
            public int Length
            {
                get
                {
                    return VersionInfo.Length;
                }
            }

            /// <summary>
            /// 获取资源哈希值。
            /// </summary>
            public int HashCode
            {
                get
                {
                    return VersionInfo.HashCode;
                }
            }

            /// <summary>
            /// 获取压缩后大小。
            /// </summary>
            public int ZipLength
            {
                get
                {
                    return VersionInfo.ZipLength;
                }
            }

            /// <summary>
            /// 获取压缩后哈希值。
            /// </summary>
            public int ZipHashCode
            {
                get
                {
                    return VersionInfo.ZipHashCode;
                }
            }

            /// <summary>
            /// 临时缓存资源所在的文件系统名称。
            /// </summary>
            public string CachedFileSystemName
            {
                get;
                private set;
            }
            /// <summary>
            /// 初始化资源检查信息的新实例。
            /// </summary>
            /// <param name="resourceName">资源名称。</param>
            public CheckInfo(ResName resName)
            {
                ResName = resName;
                Status = CheckStatus.Unknown;
                NeedRemove = false;
                NeedMoveToDisk = false;
                VersionInfo = default(RemoteVersionInfo);
                ReadWriteInfo = default(LocalVersionInfo);
                CachedFileSystemName = null;
            }

            /// <summary>
            /// 临时缓存资源所在的文件系统名称。
            /// </summary>
            /// <param name="fileSystemName">资源所在的文件系统名称。</param>
            public void SetCachedFileSystemName(string fileSystemName)
            {
                CachedFileSystemName = fileSystemName;
            }

            /// <summary>
            /// 设置资源在版本中的信息。
            /// </summary>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="length">资源大小。</param>
            /// <param name="hashCode">资源哈希值。</param>
            /// <param name="zipLength">压缩后大小。</param>
            /// <param name="zipHashCode">压缩后哈希值。</param>
            public void SetVersionInfo(LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode)
            {
                if (VersionInfo.Exist)
                {
                    Log.Fatal(UtilText.Format("You must set version info of '{0}' only once.", ResName.Name));
                    return;
                }

                VersionInfo = new RemoteVersionInfo(CachedFileSystemName, loadType, length, hashCode, zipLength, zipHashCode);
                CachedFileSystemName = null;
            }

            /// <summary>
            /// 设置资源在读写区中的信息。
            /// </summary>
            /// <param name="loadType">资源加载方式。</param>
            /// <param name="length">资源大小。</param>
            /// <param name="hashCode">资源哈希值。</param>
            public void SetReadWriteInfo(LoadType loadType, int length, int hashCode)
            {
                if (ReadWriteInfo.Exist)
                {
                    Log.Fatal(UtilText.Format("You must set read-write info of '{0}' only once.", ResName.Name));
                    return;
                }

                ReadWriteInfo = new LocalVersionInfo(CachedFileSystemName, loadType, length, hashCode);
                CachedFileSystemName = null;
            }

            /// <summary>
            /// 刷新资源信息状态。
            /// </summary>
            public void RefreshStatus()
            {
                if (!VersionInfo.Exist)
                {
                    Status = CheckStatus.Disuse;
                    NeedRemove = ReadWriteInfo.Exist;
                    return;
                }

                if (ResName.Name == null)
                {
                    if (ReadWriteInfo.Exist && ReadWriteInfo.LoadType == VersionInfo.LoadType && ReadWriteInfo.Length == VersionInfo.Length && ReadWriteInfo.HashCode == VersionInfo.HashCode)
                    {
                        bool differentFileSystem = ReadWriteInfo.FileSystemName != VersionInfo.FileSystemName;
                        Status = CheckStatus.StorageInReadWrite;
                        NeedMoveToDisk = ReadWriteInfo.UseFileSystem && differentFileSystem;
                        NeedMoveToFileSystem = VersionInfo.UseFileSystem && differentFileSystem;
                    }
                    else
                    {
                        Status = CheckStatus.Update;
                        NeedRemove = ReadWriteInfo.Exist;
                    }
                }
                else
                {
                    Status = CheckStatus.Unavailable;
                    NeedRemove = ReadWriteInfo.Exist;
                }
            }
        }

        /// <summary>
        /// 资源检查状态。
        /// </summary>
        public enum CheckStatus : byte
        {
            /// <summary>
            /// 资源状态未知。
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 资源存在且已存放于只读区中。
            /// </summary>
            StorageInReadOnly,

            /// <summary>
            /// 资源存在且已存放于读写区中。
            /// </summary>
            StorageInReadWrite,

            /// <summary>
            /// 资源不适用于当前变体。
            /// </summary>
            Unavailable,

            /// <summary>
            /// 资源需要更新。
            /// </summary>
            Update,

            /// <summary>
            /// 资源已废弃。
            /// </summary>
            Disuse
        }
    }
}
