using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;
using UnityEngine;

namespace EPloy.Res
{
    /// <summary>
    /// 资源更新器
    /// </summary>
    public sealed class ResUpdaterModule : EPloyModule
    {
        internal string BackupExtension = "bak";
        internal string ResPath = Application.persistentDataPath;
        internal PackVersionListSerializer PackVersionListSerializer { get; private set; }
        internal UpdatableVersionListSerializer UpdatableVersionListSerializer { get; private set; }
        internal LocalVersionListSerializer LocalVersionListSerializer { get; private set; }
        internal LocalVersionListSerializer ReadWriteVersionListSerializer { get; private set; }
        private ResChecker ResChecker;

        public MemoryStream DecompressCachedStream { get; set; }

        /// <summary>
        ///  更新资源需要的文件系统
        /// </summary>
        internal Dictionary<string, IFileSystem> ReadWriteFileSystems { get; private set; }

        /// <summary>
        ///  更新资源URL
        /// </summary>
        internal string UpdatePrefixUri;

        /// <summary>
        /// 资源读写信息  看代码就资源更新的时候用了
        /// </summary>
        internal SortedDictionary<ResName, ReadWriteResInfo> ReadWriteResInfos;
        /// <summary>
        /// 资源组写信息  看代码就资源更新的时候用了
        /// </summary>
        internal Dictionary<string, ResGroup> ResGroups;

        /// <summary>
        /// 变体 暂时不知道不知道鬼
        /// </summary>
        public string CurrentVariant
        {
            get;
            set;
        }

        public override void Awake()
        {
            // ResChecker = new ResourceChecker(this, ResStore.Instance);
            ReadWriteFileSystems = new Dictionary<string, IFileSystem>();
            ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>();
            ResGroups = new Dictionary<string, ResGroup>();
        }

        public override void Update()
        {

        }

        public override void OnDestroy()
        {
            ReadWriteFileSystems.Clear();
        }

        /// <summary>
        /// 检查资源。
        /// </summary>
        /// <param name="checkCallback">完成时结果的</param>
        public void CheckRes(Action<bool, string> checkCallback)
        {
            if (checkCallback == null)
            {
                Log.Fatal("Check resources complete callback is invalid.");
                return;
            }
#if UNITY_EDITOR
            checkCallback(true, "");
            return;
#endif
            ResChecker.CheckResources(CurrentVariant);
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
                string fullPath = Utility.Path.GetRegularPath(Path.Combine(ResPath, Utility.Text.Format("{0}.{1}", fileSystemName, MuduleConfig.DefaultExtension)));
                fileSystem = Game.FileSystem.GetFileSystem(fullPath);
                if (fileSystem == null)
                {
                    if (File.Exists(fullPath))
                    {
                        fileSystem = Game.FileSystem.LoadFileSystem(fullPath, FileSystemAccess.ReadWrite);
                    }
                    else
                    {
                        string directory = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        fileSystem = Game.FileSystem.CreateFileSystem(fullPath, FileSystemAccess.ReadWrite, MuduleConfig.FileSystemMaxFileCount, MuduleConfig.FileSystemMaxBlockCount);
                    }

                    ReadWriteFileSystems.Add(fileSystemName, fileSystem);
                }
            }
            return fileSystem;
        }
    }
}
