using System;
using System.Collections.Generic;
using System.IO;
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
    /// 资源检查器。
    /// </summary>
    internal sealed partial class ResourceChecker
    {
        private ResUpdaterModule ResUpdater;

        private readonly Dictionary<ResName, CheckInfo> CheckInfos;
        private string CurrentVariant;
        private string ResPath;
        private bool UpdatableVersionListReady;
        private bool ReadWriteVersionListReady;

        public EPloyAction<ResName, string, LoadType, int, int, int, int> ResourceNeedUpdate;
        public EPloyAction<int, int, int, long, long> ResourceCheckComplete;


        /// <summary>
        /// 初始化资源检查器的新实例。
        /// </summary>
        public ResourceChecker(ResUpdaterModule resUpdater)
        {
            ResUpdater = resUpdater;
            CheckInfos = new Dictionary<ResName, CheckInfo>();
            CurrentVariant = null;
            UpdatableVersionListReady = false;
            ReadWriteVersionListReady = false;

            ResourceNeedUpdate = null;
            ResourceCheckComplete = null;
        }

        /// <summary>
        /// 关闭并清理资源检查器。
        /// </summary>
        public void Shutdown()
        {
            CheckInfos.Clear();
        }

        public void CheckResources(string currentVariant)
        {
            CurrentVariant = currentVariant;
            TryRecoverReadWriteVersionList();
        }

        private void SetCachedFileSystemName(ResName ResName, string fileSystemName)
        {
            GetOrAddCheckInfo(ResName).SetCachedFileSystemName(fileSystemName);
        }

        private void SetVersionInfo(ResName ResName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode)
        {
            GetOrAddCheckInfo(ResName).SetVersionInfo(loadType, length, hashCode, zipLength, zipHashCode);
        }

        private void SetReadWriteInfo(ResName ResName, LoadType loadType, int length, int hashCode)
        {
            GetOrAddCheckInfo(ResName).SetReadWriteInfo(loadType, length, hashCode);
        }

        private CheckInfo GetOrAddCheckInfo(ResName ResName)
        {
            CheckInfo checkInfo = null;
            if (CheckInfos.TryGetValue(ResName, out checkInfo))
            {
                return checkInfo;
            }

            checkInfo = new CheckInfo(ResName);
            CheckInfos.Add(checkInfo.ResName, checkInfo);

            return checkInfo;
        }

        private void RefreshCheckInfoStatus()
        {
            if (!UpdatableVersionListReady || !ReadWriteVersionListReady)
            {
                return;
            }

            int movedCount = 0;
            int removedCount = 0;
            int updateCount = 0;
            long updateTotalLength = 0L;
            long updateTotalZipLength = 0L;
            foreach (KeyValuePair<ResName, CheckInfo> checkInfo in CheckInfos)
            {
                CheckInfo ci = checkInfo.Value;
                ci.RefreshStatus(CurrentVariant);
                if (ci.Status == CheckStatus.StorageInReadWrite)
                {
                    if (ci.NeedMoveToDisk || ci.NeedMoveToFileSystem)
                    {
                        movedCount++;
                        string resourceFullName = ci.ResName.FullName;
                        string resourcePath = Utility.Path.GetRegularPath(Path.Combine(ResPath, resourceFullName));
                        if (ci.NeedMoveToDisk)
                        {
                            IFileSystem fileSystem = ResUpdater.GetFileSystem(ci.ReadWriteFileSystemName, false);
                            if (!fileSystem.SaveAsFile(resourceFullName, resourcePath))
                            {
                                Log.Fatal(Utility.Text.Format("Save as file '{0}' to '{1}' from file system '{2}' error.", resourceFullName, fileSystem.FullPath));
                                return;
                            }

                            fileSystem.DeleteFile(resourceFullName);
                        }

                        if (ci.NeedMoveToFileSystem)
                        {
                            IFileSystem fileSystem = ResUpdater.GetFileSystem(ci.FileSystemName, false);
                            if (!fileSystem.WriteFile(resourceFullName, resourcePath))
                            {
                                Log.Fatal(Utility.Text.Format("Write resource '{0}' to file system '{1}' error.", resourceFullName, fileSystem.FullPath));
                                return;
                            }

                            if (File.Exists(resourcePath))
                            {
                                File.Delete(resourcePath);
                            }
                        }
                    }
                    // ResStore.ReadWriteResInfos.Add(ci.ResName, new ReadWriteResInfo(ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode));
                }
                else if (ci.Status == CheckStatus.Update)
                {
                    updateCount++;
                    updateTotalLength += ci.Length;
                    updateTotalZipLength += ci.ZipLength;
                    ResourceNeedUpdate(ci.ResName, ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode, ci.ZipLength, ci.ZipHashCode);
                }
                else if (ci.Status == CheckStatus.Unavailable || ci.Status == CheckStatus.Disuse)
                {
                    // Do nothing.
                }
                else
                {
                    Log.Fatal(Utility.Text.Format("Check resources '{0}' error with unknown status.", ci.ResName.FullName));
                }

                if (ci.NeedRemove)
                {
                    removedCount++;
                    if (ci.ReadWriteUseFileSystem)
                    {
                        IFileSystem fileSystem = ResUpdater.GetFileSystem(ci.ReadWriteFileSystemName, false);
                        fileSystem.DeleteFile(ci.ResName.FullName);
                    }
                    else
                    {
                        string resourcePath = Utility.Path.GetRegularPath(Path.Combine(ResPath, ci.ResName.FullName));
                        if (File.Exists(resourcePath))
                        {
                            File.Delete(resourcePath);
                        }
                    }
                }
            }

            if (movedCount > 0 || removedCount > 0)
            {
                RemoveEmptyFileSystems();
                Utility.Path.RemoveEmptyDirectory(ResPath);
            }

            ResourceCheckComplete(movedCount, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
        }

        /// <summary>
        /// 尝试恢复读写区版本资源列表。
        /// </summary>
        /// <returns>是否恢复成功。</returns>
        private bool TryRecoverReadWriteVersionList()
        {
            string file = Utility.Path.GetRegularPath(Path.Combine(ResPath, MuduleConfig.LocalVersionListFileName));
            string backupFile = Utility.Text.Format("{0}.{1}", file, ResUpdaterModule.BackupExtension);

            try
            {
                if (!File.Exists(backupFile))
                {
                    return false;
                }

                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                File.Move(backupFile, file);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void RemoveEmptyFileSystems()
        {
            List<string> removedFileSystemNames = null;
            foreach (KeyValuePair<string, IFileSystem> fileSystem in ResUpdater.ReadWriteFileSystems)
            {
                if (fileSystem.Value.FileCount <= 0)
                {
                    if (removedFileSystemNames == null)
                    {
                        removedFileSystemNames = new List<string>();
                    }

                    Game.FileSystem.DestroyFileSystem(fileSystem.Value, true);
                    removedFileSystemNames.Add(fileSystem.Key);
                }
            }

            if (removedFileSystemNames != null)
            {
                foreach (string removedFileSystemName in removedFileSystemNames)
                {
                    ResUpdater.ReadWriteFileSystems.Remove(removedFileSystemName);
                }
            }
        }

        private void OnLoadUpdatableVersionListSuccess(string fileUri, byte[] bytes, float duration)
        {
            if (UpdatableVersionListReady)
            {
                Log.Fatal("Updatable version list has been parsed.");
                return;
            }

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes, false);
                UpdatableVersionList versionList = ResUpdater.UpdatableVersionListSerializer.Deserialize(memoryStream);
                if (!versionList.IsValid)
                {
                    Log.Fatal("Deserialize updatable version list failure.");
                    return;
                }

                UpdatableVersionList.Asset[] assets = versionList.GetAssets();
                UpdatableVersionList.Resource[] resources = versionList.GetResources();
                UpdatableVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
                UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
                //ResStore.SetResVersion(versionList.ApplicableGameVersion, versionList.InternalResourceVersion);
                //ResStore.VersionInfos = new Dictionary<string, AssetInfo>(assets.Length, StringComparer.Ordinal);
                // ResStore.ResInfos = new Dictionary<ResName, ResInfo>(resources.Length, new ResNameComparer());
                // ResStore.ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>(new ResNameComparer());
                // ResGroup defaultResourceGroup = ResStore.GetOrAddResourceGroup(string.Empty);

                foreach (UpdatableVersionList.FileSystem fileSystem in fileSystems)
                {
                    int[] resourceIndexes = fileSystem.GetResourceIndexes();
                    foreach (int resourceIndex in resourceIndexes)
                    {
                        UpdatableVersionList.Resource resource = resources[resourceIndex];
                        if (resource.Variant != null && resource.Variant != CurrentVariant)
                        {
                            continue;
                        }

                        SetCachedFileSystemName(new ResName(resource.Name, resource.Variant, resource.Extension), fileSystem.Name);
                    }
                }

                foreach (UpdatableVersionList.Resource resource in resources)
                {
                    if (resource.Variant != null && resource.Variant != CurrentVariant)
                    {
                        continue;
                    }

                    ResName ResName = new ResName(resource.Name, resource.Variant, resource.Extension);
                    int[] assetIndexes = resource.GetAssetIndexes();
                    foreach (int assetIndex in assetIndexes)
                    {
                        UpdatableVersionList.Asset asset = assets[assetIndex];
                        int[] dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
                        int index = 0;
                        string[] dependencyAssetNames = new string[dependencyAssetIndexes.Length];
                        foreach (int dependencyAssetIndex in dependencyAssetIndexes)
                        {
                            dependencyAssetNames[index++] = assets[dependencyAssetIndex].Name;
                        }

                        //ResStore.VersionInfos.Add(asset.Name, new AssetInfo(asset.Name, ResName, dependencyAssetNames));
                    }

                    SetVersionInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode, resource.ZipLength, resource.ZipHashCode);
                    // defaultResourceGroup.AddResource(ResName, resource.Length, resource.ZipLength);
                }

                foreach (UpdatableVersionList.ResourceGroup resourceGroup in resourceGroups)
                {
                    // ResGroup group = ResStore.GetOrAddResourceGroup(resourceGroup.Name);
                    //  int[] resourceIndexes = resourceGroup.GetResourceIndexes();
                    //  foreach (int resourceIndex in resourceIndexes)
                    //  {
                    //      UpdatableVersionList.Resource resource = resources[resourceIndex];
                    //      if (resource.Variant != null && resource.Variant != CurrentVariant)
                    //       {
                    //            continue;
                    //        }

                    //       group.AddResource(new ResName(resource.Name, resource.Variant, resource.Extension), resource.Length, resource.ZipLength);
                    //   }
                }

                UpdatableVersionListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                Log.Fatal(Utility.Text.Format("Parse updatable version list exception '{0}'.", exception.ToString()));
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }
        }

        private void OnLoadUpdatableVersionListFailure(string fileUri, string errorMessage)
        {
            Log.Fatal(Utility.Text.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
        }

        private void OnLoadReadWriteVersionListSuccess(string fileUri, byte[] bytes, float duration)
        {
            if (ReadWriteVersionListReady)
            {
                Log.Fatal("Read write version list has been parsed.");
                return;
            }

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes, false);
                LocalVersionList versionList = ResUpdater.LocalVersionListSerializer.Deserialize(memoryStream);
                if (!versionList.IsValid)
                {
                    Log.Fatal("Deserialize read write version list failure.");
                }

                foreach (LocalVersionList.FileSystem fileSystem in versionList.FileSystems)
                {
                    foreach (int resourceIndex in fileSystem.ResourceIndexes)
                    {
                        LocalVersionList.Resource resource = versionList.Resources[resourceIndex];
                        SetCachedFileSystemName(new ResName(resource.Name, resource.Variant, resource.Extension), fileSystem.Name);
                    }
                }

                foreach (LocalVersionList.Resource resource in versionList.Resources)
                {
                    ResName ResName = new ResName(resource.Name, resource.Variant, resource.Extension);
                    SetReadWriteInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode);
                }

                ReadWriteVersionListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                Log.Fatal(Utility.Text.Format("Parse read write version list exception '{0}'.", exception.ToString()));
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }
        }

        private void OnLoadReadWriteVersionListFailure(string fileUri, string errorMessage)
        {
            if (ReadWriteVersionListReady)
            {
                Log.Fatal("Read write version list has been parsed.");
                return;
            }

            ReadWriteVersionListReady = true;
            RefreshCheckInfoStatus();
        }
    }
}
