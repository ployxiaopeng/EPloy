using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;
using UnityEngine;

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

        private CheckResCompleteCallback CheckResCompleteCallback;
        private UpdateResCompleteCallback UpdateResCompleteCallback;

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
            ResChecker.ResCheckerCallBack = new ResCheckerCallBack(OnResNeedUpdate, OnResCheckComplete);
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
        /// 检查资源 如需更新  检查并装备好更新列表
        /// </summary>
        /// <param name="checkCallback">完成时结果的</param>
        public void CheckRes(CheckResCompleteCallback checkResCompleteCallback)
        {
            CheckResCompleteCallback = checkResCompleteCallback;
            if (CheckResCompleteCallback == null)
            {
                Log.Fatal("Check resources complete callback is invalid.");
                return;
            }
            ResChecker.CheckResources(CurrentVariant);
        }

        /// <summary>
        /// 更新指定资源组的资源。
        /// </summary>
        /// <param name="resourceGroupName">要更新的资源组名称 默认组为   string.Empty</param>
        /// <param name="updateResCompleteCallback">更新指定资源组完成时的回调函数。</param>
        public void UpdateRes(UpdateResCompleteCallback updateResCompleteCallback, string resGroupName = null)
        {
            if (updateResCompleteCallback == null)
            {
                Log.Fatal("Update resources complete callback is invalid.");
            }

            if (resGroupName == null)
            {
                resGroupName = string.Empty;
            }
            ResGroup resGroup = ResStore.GetResGroup(resGroupName);
            if (resGroup == null)
            {
                Log.Fatal(Utility.Text.Format("Can not find resource group '{0}'.", resGroupName));
                return;
            }

            UpdateResCompleteCallback = updateResCompleteCallback;
            UpdaterHandler.UpdateRess(resGroup);
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

        private void OnResNeedUpdate(ResName resourceName, string fileSystemName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode)
        {
            UpdaterHandler.AddResUpdate(resourceName, fileSystemName, loadType, length, hashCode, zipLength, zipHashCode, Utility.Path.GetRegularPath(Path.Combine(ResPath, resourceName.FullName)));
        }

        private void OnResCheckComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
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

            CheckResCompleteCallback(movedCount, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
            CheckResCompleteCallback = null;
        }

        private void OnResApplySuccess(ResName resName, string applyPath, string resPackPath, int length, int zipLength)
        {

        }

        private void OnResApplyFailure(ResName resName, string resPackPath, string errorMessage)
        {

        }

        private void OnResApplyComplete(string resPackPath, bool result, bool isAllDone)
        {
            if (isAllDone)
            {
                // m_ResourceUpdater.ResourceApplySuccess -= OnResourceApplySuccess;
                // m_ResourceUpdater.ResourceApplyFailure -= OnResourceApplyFailure;
                // m_ResourceUpdater.ResourceApplyComplete -= OnResourceApplyComplete;
                // m_ResourceUpdater.ResourceUpdateStart -= OnourceUpdateStart;
                // m_ResourceUpdater.ResourceUpdateChanged -= OnourceUpdateChanged;
                // m_ResourceUpdater.ResourceUpdateSuccess -= OnourceUpdateSuccess;
                // m_ResourceUpdater.ResourceUpdateFailure -= OnourceUpdateFailure;
                // m_ResourceUpdater.ResourceUpdateComplete -= OnourceUpdateComplete;
                UpdaterHandler.OnDestroy();
                UpdaterHandler = null;

                ResStore.ReadWriteResInfos.Clear();
                ResStore.ReadWriteResInfos = null;

                if (DecompressCachedStream != null)
                {
                    DecompressCachedStream.Dispose();
                    DecompressCachedStream = null;
                }

                Utility.Path.RemoveEmptyDirectory(ResPath);
            }

            // ApplyResourcesCompleteCallback applyResourcesCompleteCallback = m_ApplyResourcesCompleteCallback;
            // m_ApplyResourcesCompleteCallback = null;
            // applyResourcesCompleteCallback(resourcePackPath, result);
        }

        private void OnUpdateStart(ResName resName, string downloadPath, string downloadUri, int currentLength, int zipLength, int retryCount)
        {

        }

        private void OnUpdateChanged(ResName resName, string downloadPath, string downloadUri, int currentLength, int zipLength)
        {

        }

        private void OnUpdateSuccess(ResName resName, string downloadPath, string downloadUri, int length, int zipLength)
        {

        }

        private void OnUpdateFailure(ResName resName, string downloadUri, int retryCount, int totalRetryCount, string errorMessage)
        {

        }

        private void OnUpdateComplete(ResGroup resGroup, bool result, bool isAllDone)
        {
            if (isAllDone)
            {
                // m_ResourceUpdater.ResourceApplySuccess -= OnResourceApplySuccess;
                // m_ResourceUpdater.ResourceApplyFailure -= OnResourceApplyFailure;
                // m_ResourceUpdater.ResourceApplyComplete -= OnResourceApplyComplete;
                // m_ResourceUpdater.ResourceUpdateStart -= OnourceUpdateStart;
                // m_ResourceUpdater.ResourceUpdateChanged -= OnourceUpdateChanged;
                // m_ResourceUpdater.ResourceUpdateSuccess -= OnourceUpdateSuccess;
                // m_ResourceUpdater.ResourceUpdateFailure -= OnourceUpdateFailure;
                // m_ResourceUpdater.ResourceUpdateComplete -= OnourceUpdateComplete;
                UpdaterHandler.OnDestroy();
                UpdaterHandler = null;

                ResStore.ReadWriteResInfos.Clear();
                ResStore.ReadWriteResInfos = null;

                if (DecompressCachedStream != null)
                {
                    DecompressCachedStream.Dispose();
                    DecompressCachedStream = null;
                }

                Utility.Path.RemoveEmptyDirectory(ResPath);
            }

            // UpdateResourcesCompleteCallback updateResourcesCompleteCallback = m_UpdateResourcesCompleteCallback;
            // m_UpdateResourcesCompleteCallback = null;
            // updateResourcesCompleteCallback(resourceGroup, result);
        }
    }
}
