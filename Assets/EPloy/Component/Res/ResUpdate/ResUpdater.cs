using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;
using EPloy.SystemFile;

namespace EPloy.Res
{
    /// <summary>
    /// 检查资源回调。
    /// </summary>
    /// <param name="movedCount">已移动的资源数量。</param>
    /// <param name="removedCount">已移除的资源数量。</param>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    /// <param name="updateTotalZipLength">可更新的压缩后总大小。</param>
    public delegate void CheckResCompleteCallback(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength);

    /// <summary>
    /// 资源更新器
    /// </summary>
    public sealed class ResUpdater
    {
        internal const string BackupExtension = "bak";
        internal const string ResPath = "bak";

        private static ResUpdater instance = null;
        public static ResUpdater CreateResUpdater()
        {
            if (instance == null) instance = new ResUpdater();
            return instance;
        }
        internal static ResUpdater Instance
        {
            get
            {
                return CreateResUpdater();
            }
        }

        internal FileSystemComponent FileSystem
        {
            get
            {
                return GameEntry.FileSystem;
            }
        }

        private ResourceChecker ResChecker;
        /// <summary>
        ///  更新资源需要的文件系统
        /// </summary>
        internal Dictionary<string, IFileSystem> ReadWriteFileSystems { get; private set; }

        /// <summary>
        /// 变体 暂时不知道不知道鬼
        /// </summary>
        public string CurrentVariant
        {
            get;
            set;
        }

        private ResUpdater()
        {
            ResChecker = new ResourceChecker(this, ResStore.Instance);
        }

        /// <summary>
        /// 检查资源。
        /// </summary>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成时的回调函数。</param>
        public void CheckResources(bool ignoreOtherVariant, CheckResCompleteCallback checkResCompleteCallback)
        {
            if (checkResCompleteCallback == null)
            {
                Log.Fatal("Check resources complete callback is invalid.");
                return;
            }
            ResChecker.CheckResources(CurrentVariant);
            GameEntry.Res.LoadBytes(Utility.Path.GetRemotePath(Path.Combine(ResPath, Config.RemoteVersionListFileName)), ResChecker.UpdatableVersionCallbacks);
            GameEntry.Res.LoadBytes(Utility.Path.GetRemotePath(Path.Combine(ResPath, Config.LocalVersionListFileName)), ResChecker.ReadWriteVersionCallbacks);
        }

        internal IFileSystem GetFileSystem(string fileSystemName, bool storageInReadOnly)
        {
            if (string.IsNullOrEmpty(fileSystemName))
            {
                Log.Fatal("File system name is invalid.");
                return null;
            }

            IFileSystem fileSystem = null;
            if (!ReadWriteFileSystems.TryGetValue(fileSystemName, out fileSystem))
            {
                string fullPath = Utility.Path.GetRegularPath(Path.Combine(ResPath, Utility.Text.Format("{0}.{1}", fileSystemName, Config.DefaultExtension)));
                fileSystem = FileSystem.GetFileSystem(fullPath);
                if (fileSystem == null)
                {
                    if (File.Exists(fullPath))
                    {
                        fileSystem = FileSystem.LoadFileSystem(fullPath, FileSystemAccess.ReadWrite);
                    }
                    else
                    {
                        string directory = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        fileSystem = FileSystem.CreateFileSystem(fullPath, FileSystemAccess.ReadWrite, Config.FileSystemMaxFileCount, Config.FileSystemMaxBlockCount);
                    }

                    ReadWriteFileSystems.Add(fileSystemName, fileSystem);
                }
            }
            return fileSystem;
        }
    }
}
