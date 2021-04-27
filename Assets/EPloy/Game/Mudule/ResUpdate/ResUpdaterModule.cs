using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;
using UnityEngine;

namespace EPloy.Res
{
    /// <summary>
    /// 资源更新
    /// </summary>
    public sealed class ResUpdaterModule : EPloyModule
    {
        internal string BackupExtension = "bak";
        internal string ResPath = Application.persistentDataPath;

        private ResStore ResStore;
        private ResChecker ResChecker;
        private UpdaterHandler UpdaterHandler;

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
        /// 变体 暂时不知道不知道鬼
        /// </summary>
        public string CurrentVariant
        {
            get;
            set;
        }

        public override void Awake()
        {
            ResStore = new ResStore();
            ResChecker = new ResChecker(this, ResStore);
            ResChecker.ResCheckerCallBack = new ResCheckerCallBack(OnCheckerResNeedUpdate, OnCheckerResCheckComplete);
            UpdaterHandler = new UpdaterHandler(this, ResStore);
            ReadWriteFileSystems = new Dictionary<string, IFileSystem>();
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

        private void OnCheckerResNeedUpdate(ResName resourceName, string fileSystemName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode)
        {
            UpdaterHandler.AddResUpdate(resourceName, fileSystemName, loadType, length, hashCode, zipLength, zipHashCode, Utility.Path.GetRegularPath(Path.Combine(ResPath, resourceName.FullName)));
        }

        private void OnCheckerResCheckComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            Game.VersionChecker.OnDestroy();
            ResChecker.OnDestroy();
            UpdaterHandler.CheckResComplete(movedCount > 0 || removedCount > 0);

            if (updateCount <= 0)
            {
                UpdaterHandler.OnDestroy();

                ResStore.ReadWriteResInfos.Clear();
                ResStore.ReadWriteResInfos = null;

                if (DecompressCachedStream != null)
                {
                    DecompressCachedStream.Dispose();
                    DecompressCachedStream = null;
                }
            }

            // m_CheckResourcesCompleteCallback(movedCount, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
            // m_CheckResourcesCompleteCallback = null;
        }

    }
}
