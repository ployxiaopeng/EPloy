using System;
using System.Collections.Generic;
using System.IO;

namespace EPloy.Res
{
    /// <summary>
    /// 资源检查器。
    /// </summary>
    internal sealed partial class ResourceChecker
    {
        // //private readonly ResourceManager m_ResourceManager;
        // private readonly Dictionary<ResName, CheckInfo> m_CheckInfos;
        // private string m_CurrentVariant;
        // private bool m_IgnoreOtherVariant;
        // private bool m_UpdatableVersionListReady;
        // private bool m_ReadOnlyVersionListReady;
        // private bool m_ReadWriteVersionListReady;

        // private string RemoteVersionListFileName
        // {
        //     get
        //     {
        //         return ResUpdater.RemoteVersionListFileName;
        //     }
        // }
        // private string LocalVersionListFileName
        // {
        //     get
        //     {
        //         return ResUpdater.LocalVersionListFileName;
        //     }
        // }

        // private string ResPath
        // {
        //     get
        //     {
        //         return ResUpdater.ResPath;
        //     }
        // }
        // private string BackupExtension
        // {
        //     get
        //     {
        //         return ResUpdater.BackupExtension;
        //     }
        // }


        // public EPloyAction<ResName, string, LoadType, int, int, int, int> ResourceNeedUpdate;
        // public EPloyAction<int, int, int, long, long> ResourceCheckComplete;

        // /// <summary>
        // /// 初始化资源检查器的新实例。
        // /// </summary>
        // /// <param name="resourceManager">资源管理器。</param>
        // public ResourceChecker()
        // {
        //     // m_ResourceManager = resourceManager;
        //     m_CheckInfos = new Dictionary<ResName, CheckInfo>();
        //     m_CurrentVariant = null;
        //     m_IgnoreOtherVariant = false;
        //     m_UpdatableVersionListReady = false;
        //     m_ReadOnlyVersionListReady = false;
        //     m_ReadWriteVersionListReady = false;

        //     ResourceNeedUpdate = null;
        //     ResourceCheckComplete = null;
        // }

        // /// <summary>
        // /// 关闭并清理资源检查器。
        // /// </summary>
        // public void Shutdown()
        // {
        //     m_CheckInfos.Clear();
        // }

        // public void CheckResources(string currentVariant, bool ignoreOtherVariant)
        // {
        //     m_CurrentVariant = currentVariant;
        //     m_IgnoreOtherVariant = ignoreOtherVariant;

        //     TryRecoverReadWriteVersionList();
        //     // m_ResourceManager.m_ResourceHelper.LoadBytes(Utility.Path.GetRemotePath(Path.Combine(m_ResourceManager.m_ReadWritePath, RemoteVersionListFileName)), new LoadBytesCallbacks(OnLoadUpdatableVersionListSuccess, OnLoadUpdatableVersionListFailure), null);
        //     // m_ResourceManager.m_ResourceHelper.LoadBytes(Utility.Path.GetRemotePath(Path.Combine(m_ResourceManager.m_ReadOnlyPath, LocalVersionListFileName)), new LoadBytesCallbacks(OnLoadReadOnlyVersionListSuccess, OnLoadReadOnlyVersionListFailure), null);
        //     // m_ResourceManager.m_ResourceHelper.LoadBytes(Utility.Path.GetRemotePath(Path.Combine(m_ResourceManager.m_ReadWritePath, LocalVersionListFileName)), new LoadBytesCallbacks(OnLoadReadWriteVersionListSuccess, OnLoadReadWriteVersionListFailure), null);
        // }

        // private void SetCachedFileSystemName(ResName ResName, string fileSystemName)
        // {
        //     GetOrAddCheckInfo(ResName).SetCachedFileSystemName(fileSystemName);
        // }

        // private void SetVersionInfo(ResName ResName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode)
        // {
        //     GetOrAddCheckInfo(ResName).SetVersionInfo(loadType, length, hashCode, zipLength, zipHashCode);
        // }

        // private void SetReadOnlyInfo(ResName ResName, LoadType loadType, int length, int hashCode)
        // {
        //     GetOrAddCheckInfo(ResName).SetReadOnlyInfo(loadType, length, hashCode);
        // }

        // private void SetReadWriteInfo(ResName ResName, LoadType loadType, int length, int hashCode)
        // {
        //     GetOrAddCheckInfo(ResName).SetReadWriteInfo(loadType, length, hashCode);
        // }

        // private CheckInfo GetOrAddCheckInfo(ResName ResName)
        // {
        //     CheckInfo checkInfo = null;
        //     if (m_CheckInfos.TryGetValue(ResName, out checkInfo))
        //     {
        //         return checkInfo;
        //     }

        //     checkInfo = new CheckInfo(ResName);
        //     m_CheckInfos.Add(checkInfo.ResName, checkInfo);

        //     return checkInfo;
        // }

        // private void RefreshCheckInfoStatus()
        // {
        //     if (!m_UpdatableVersionListReady || !m_ReadOnlyVersionListReady || !m_ReadWriteVersionListReady)
        //     {
        //         return;
        //     }

        //     int movedCount = 0;
        //     int removedCount = 0;
        //     int updateCount = 0;
        //     long updateTotalLength = 0L;
        //     long updateTotalZipLength = 0L;
        //     foreach (KeyValuePair<ResName, CheckInfo> checkInfo in m_CheckInfos)
        //     {
        //         CheckInfo ci = checkInfo.Value;
        //         ci.RefreshStatus(m_CurrentVariant, m_IgnoreOtherVariant);
        //         if (ci.Status == CheckStatus.StorageInReadOnly)
        //         {
        //             m_ResourceManager.m_ResInfos.Add(ci.ResName, new ResInfo(ci.ResName, ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode, true, true));
        //         }
        //         else if (ci.Status == CheckStatus.StorageInReadWrite)
        //         {
        //             if (ci.NeedMoveToDisk || ci.NeedMoveToFileSystem)
        //             {
        //                 movedCount++;
        //                 string resourceFullName = ci.ResName.FullName;
        //                 string resourcePath = Utility.Path.GetRegularPath(Path.Combine(m_ResourceManager.m_ReadWritePath, resourceFullName));
        //                 if (ci.NeedMoveToDisk)
        //                 {
        //                     IFileSystem fileSystem = m_ResourceManager.GetFileSystem(ci.ReadWriteFileSystemName, false);
        //                     if (!fileSystem.SaveAsFile(resourceFullName, resourcePath))
        //                     {
        //                         throw new EPloyException(Utility.Text.Format("Save as file '{0}' to '{1}' from file system '{2}' error.", resourceFullName, fileSystem.FullPath));
        //                     }

        //                     fileSystem.DeleteFile(resourceFullName);
        //                 }

        //                 if (ci.NeedMoveToFileSystem)
        //                 {
        //                     IFileSystem fileSystem = m_ResourceManager.GetFileSystem(ci.FileSystemName, false);
        //                     if (!fileSystem.WriteFile(resourceFullName, resourcePath))
        //                     {
        //                         throw new EPloyException(Utility.Text.Format("Write resource '{0}' to file system '{1}' error.", resourceFullName, fileSystem.FullPath));
        //                     }

        //                     if (File.Exists(resourcePath))
        //                     {
        //                         File.Delete(resourcePath);
        //                     }
        //                 }
        //             }

        //             m_ResourceManager.m_ResInfos.Add(ci.ResName, new ResInfo(ci.ResName, ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode, false, true));
        //             m_ResourceManager.m_ReadWriteResInfos.Add(ci.ResName, new ReadWriteResInfo(ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode));
        //         }
        //         else if (ci.Status == CheckStatus.Update)
        //         {
        //             m_ResourceManager.m_ResInfos.Add(ci.ResName, new ResInfo(ci.ResName, ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode, false, false));
        //             updateCount++;
        //             updateTotalLength += ci.Length;
        //             updateTotalZipLength += ci.ZipLength;
        //             ResourceNeedUpdate(ci.ResName, ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode, ci.ZipLength, ci.ZipHashCode);
        //         }
        //         else if (ci.Status == CheckStatus.Unavailable || ci.Status == CheckStatus.Disuse)
        //         {
        //             // Do nothing.
        //         }
        //         else
        //         {
        //             throw new EPloyException(Utility.Text.Format("Check resources '{0}' error with unknown status.", ci.ResName.FullName));
        //         }

        //         if (ci.NeedRemove)
        //         {
        //             removedCount++;
        //             if (ci.ReadWriteUseFileSystem)
        //             {
        //                 IFileSystem fileSystem = m_ResourceManager.GetFileSystem(ci.ReadWriteFileSystemName, false);
        //                 fileSystem.DeleteFile(ci.ResName.FullName);
        //             }
        //             else
        //             {
        //                 string resourcePath = Utility.Path.GetRegularPath(Path.Combine(m_ResourceManager.m_ReadWritePath, ci.ResName.FullName));
        //                 if (File.Exists(resourcePath))
        //                 {
        //                     File.Delete(resourcePath);
        //                 }
        //             }
        //         }
        //     }

        //     if (movedCount > 0 || removedCount > 0)
        //     {
        //         RemoveEmptyFileSystems();
        //         Utility.Path.RemoveEmptyDirectory(m_ResourceManager.m_ReadWritePath);
        //     }

        //     ResourceCheckComplete(movedCount, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
        // }

        // /// <summary>
        // /// 尝试恢复读写区版本资源列表。
        // /// </summary>
        // /// <returns>是否恢复成功。</returns>
        // private bool TryRecoverReadWriteVersionList()
        // {
        //     string file = Utility.Path.GetRegularPath(Path.Combine(ResPath, LocalVersionListFileName));
        //     string backupFile = Utility.Text.Format("{0}.{1}", file, BackupExtension);

        //     try
        //     {
        //         if (!File.Exists(backupFile))
        //         {
        //             return false;
        //         }

        //         if (File.Exists(file))
        //         {
        //             File.Delete(file);
        //         }

        //         File.Move(backupFile, file);
        //     }
        //     catch
        //     {
        //         return false;
        //     }

        //     return true;
        // }

        // private void RemoveEmptyFileSystems()
        // {
        //     List<string> removedFileSystemNames = null;
        //     foreach (KeyValuePair<string, IFileSystem> fileSystem in ResPath)
        //     {
        //         if (fileSystem.Value.FileCount <= 0)
        //         {
        //             if (removedFileSystemNames == null)
        //             {
        //                 removedFileSystemNames = new List<string>();
        //             }

        //             m_ResourceManager.m_FileSystemManager.DestroyFileSystem(fileSystem.Value, true);
        //             removedFileSystemNames.Add(fileSystem.Key);
        //         }
        //     }

        //     if (removedFileSystemNames != null)
        //     {
        //         foreach (string removedFileSystemName in removedFileSystemNames)
        //         {
        //             m_ResourceManager.m_ReadWriteFileSystems.Remove(removedFileSystemName);
        //         }
        //     }
        // }

        // private void OnLoadUpdatableVersionListSuccess(string fileUri, byte[] bytes, float duration, object userData)
        // {
        //     if (m_UpdatableVersionListReady)
        //     {
        //         throw new EPloyException("Updatable version list has been parsed.");
        //     }

        //     MemoryStream memoryStream = null;
        //     try
        //     {
        //         memoryStream = new MemoryStream(bytes, false);
        //         UpdatableVersionList versionList = m_ResourceManager.m_UpdatableVersionListSerializer.Deserialize(memoryStream);
        //         if (!versionList.IsValid)
        //         {
        //             throw new EPloyException("Deserialize updatable version list failure.");
        //         }

        //         UpdatableVersionList.Asset[] assets = versionList.GetAssets();
        //         UpdatableVersionList.Resource[] resources = versionList.GetResources();
        //         UpdatableVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
        //         UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
        //         m_ResourceManager.m_ApplicableGameVersion = versionList.ApplicableGameVersion;
        //         m_ResourceManager.m_InternalResourceVersion = versionList.InternalResourceVersion;
        //         m_ResourceManager.m_AssetInfos = new Dictionary<string, AssetInfo>(assets.Length, StringComparer.Ordinal);
        //         m_ResourceManager.m_ResInfos = new Dictionary<ResName, ResInfo>(resources.Length, new ResNameComparer());
        //         m_ResourceManager.m_ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>(new ResNameComparer());
        //         ResourceGroup defaultResourceGroup = m_ResourceManager.GetOrAddResourceGroup(string.Empty);

        //         foreach (UpdatableVersionList.FileSystem fileSystem in fileSystems)
        //         {
        //             int[] resourceIndexes = fileSystem.GetResourceIndexes();
        //             foreach (int resourceIndex in resourceIndexes)
        //             {
        //                 UpdatableVersionList.Resource resource = resources[resourceIndex];
        //                 if (resource.Variant != null && resource.Variant != m_CurrentVariant)
        //                 {
        //                     continue;
        //                 }

        //                 SetCachedFileSystemName(new ResName(resource.Name, resource.Variant, resource.Extension), fileSystem.Name);
        //             }
        //         }

        //         foreach (UpdatableVersionList.Resource resource in resources)
        //         {
        //             if (resource.Variant != null && resource.Variant != m_CurrentVariant)
        //             {
        //                 continue;
        //             }

        //             ResName ResName = new ResName(resource.Name, resource.Variant, resource.Extension);
        //             int[] assetIndexes = resource.GetAssetIndexes();
        //             foreach (int assetIndex in assetIndexes)
        //             {
        //                 UpdatableVersionList.Asset asset = assets[assetIndex];
        //                 int[] dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
        //                 int index = 0;
        //                 string[] dependencyAssetNames = new string[dependencyAssetIndexes.Length];
        //                 foreach (int dependencyAssetIndex in dependencyAssetIndexes)
        //                 {
        //                     dependencyAssetNames[index++] = assets[dependencyAssetIndex].Name;
        //                 }

        //                 m_ResourceManager.m_AssetInfos.Add(asset.Name, new AssetInfo(asset.Name, ResName, dependencyAssetNames));
        //             }

        //             SetVersionInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode, resource.ZipLength, resource.ZipHashCode);
        //             defaultResourceGroup.AddResource(ResName, resource.Length, resource.ZipLength);
        //         }

        //         foreach (UpdatableVersionList.ResourceGroup resourceGroup in resourceGroups)
        //         {
        //             ResourceGroup group = m_ResourceManager.GetOrAddResourceGroup(resourceGroup.Name);
        //             int[] resourceIndexes = resourceGroup.GetResourceIndexes();
        //             foreach (int resourceIndex in resourceIndexes)
        //             {
        //                 UpdatableVersionList.Resource resource = resources[resourceIndex];
        //                 if (resource.Variant != null && resource.Variant != m_CurrentVariant)
        //                 {
        //                     continue;
        //                 }

        //                 group.AddResource(new ResName(resource.Name, resource.Variant, resource.Extension), resource.Length, resource.ZipLength);
        //             }
        //         }

        //         m_UpdatableVersionListReady = true;
        //         RefreshCheckInfoStatus();
        //     }
        //     catch (Exception exception)
        //     {
        //         if (exception is EPloyException)
        //         {
        //             throw;
        //         }

        //         throw new EPloyException(Utility.Text.Format("Parse updatable version list exception '{0}'.", exception.ToString()), exception);
        //     }
        //     finally
        //     {
        //         if (memoryStream != null)
        //         {
        //             memoryStream.Dispose();
        //             memoryStream = null;
        //         }
        //     }
        // }

        // private void OnLoadUpdatableVersionListFailure(string fileUri, string errorMessage, object userData)
        // {
        //     throw new EPloyException(Utility.Text.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
        // }

        // private void OnLoadReadOnlyVersionListSuccess(string fileUri, byte[] bytes, float duration, object userData)
        // {
        //     if (m_ReadOnlyVersionListReady)
        //     {
        //         throw new EPloyException("Read only version list has been parsed.");
        //     }

        //     MemoryStream memoryStream = null;
        //     try
        //     {
        //         memoryStream = new MemoryStream(bytes, false);
        //         LocalVersionList versionList = m_ResourceManager.m_ReadOnlyVersionListSerializer.Deserialize(memoryStream);
        //         if (!versionList.IsValid)
        //         {
        //             throw new EPloyException("Deserialize read only version list failure.");
        //         }

        //         LocalVersionList.Resource[] resources = versionList.GetResources();
        //         LocalVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();

        //         foreach (LocalVersionList.FileSystem fileSystem in fileSystems)
        //         {
        //             int[] resourceIndexes = fileSystem.GetResourceIndexes();
        //             foreach (int resourceIndex in resourceIndexes)
        //             {
        //                 LocalVersionList.Resource resource = resources[resourceIndex];
        //                 SetCachedFileSystemName(new ResName(resource.Name, resource.Variant, resource.Extension), fileSystem.Name);
        //             }
        //         }

        //         foreach (LocalVersionList.Resource resource in resources)
        //         {
        //             SetReadOnlyInfo(new ResName(resource.Name, resource.Variant, resource.Extension), (LoadType)resource.LoadType, resource.Length, resource.HashCode);
        //         }

        //         m_ReadOnlyVersionListReady = true;
        //         RefreshCheckInfoStatus();
        //     }
        //     catch (Exception exception)
        //     {
        //         if (exception is EPloyException)
        //         {
        //             throw;
        //         }

        //         throw new EPloyException(Utility.Text.Format("Parse read only version list exception '{0}'.", exception.ToString()), exception);
        //     }
        //     finally
        //     {
        //         if (memoryStream != null)
        //         {
        //             memoryStream.Dispose();
        //             memoryStream = null;
        //         }
        //     }
        // }

        // private void OnLoadReadOnlyVersionListFailure(string fileUri, string errorMessage, object userData)
        // {
        //     if (m_ReadOnlyVersionListReady)
        //     {
        //         throw new EPloyException("Read only version list has been parsed.");
        //     }

        //     m_ReadOnlyVersionListReady = true;
        //     RefreshCheckInfoStatus();
        // }

        // private void OnLoadReadWriteVersionListSuccess(string fileUri, byte[] bytes, float duration, object userData)
        // {
        //     if (m_ReadWriteVersionListReady)
        //     {
        //         throw new EPloyException("Read write version list has been parsed.");
        //     }

        //     MemoryStream memoryStream = null;
        //     try
        //     {
        //         memoryStream = new MemoryStream(bytes, false);
        //         LocalVersionList versionList = m_ResourceManager.m_ReadWriteVersionListSerializer.Deserialize(memoryStream);
        //         if (!versionList.IsValid)
        //         {
        //             throw new EPloyException("Deserialize read write version list failure.");
        //         }

        //         LocalVersionList.Resource[] resources = versionList.GetResources();
        //         LocalVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();

        //         foreach (LocalVersionList.FileSystem fileSystem in fileSystems)
        //         {
        //             int[] resourceIndexes = fileSystem.GetResourceIndexes();
        //             foreach (int resourceIndex in resourceIndexes)
        //             {
        //                 LocalVersionList.Resource resource = resources[resourceIndex];
        //                 SetCachedFileSystemName(new ResName(resource.Name, resource.Variant, resource.Extension), fileSystem.Name);
        //             }
        //         }

        //         foreach (LocalVersionList.Resource resource in resources)
        //         {
        //             ResName ResName = new ResName(resource.Name, resource.Variant, resource.Extension);
        //             SetReadWriteInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode);
        //         }

        //         m_ReadWriteVersionListReady = true;
        //         RefreshCheckInfoStatus();
        //     }
        //     catch (Exception exception)
        //     {
        //         if (exception is EPloyException)
        //         {
        //             throw;
        //         }

        //         throw new EPloyException(Utility.Text.Format("Parse read write version list exception '{0}'.", exception.ToString()), exception);
        //     }
        //     finally
        //     {
        //         if (memoryStream != null)
        //         {
        //             memoryStream.Dispose();
        //             memoryStream = null;
        //         }
        //     }
        // }

        // private void OnLoadReadWriteVersionListFailure(string fileUri, string errorMessage, object userData)
        // {
        //     if (m_ReadWriteVersionListReady)
        //     {
        //         throw new EPloyException("Read write version list has been parsed.");
        //     }

        //     m_ReadWriteVersionListReady = true;
        //     RefreshCheckInfoStatus();
        // }
    }
}

