using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EPloy.Game.SystemFile;
using UnityEngine.Networking;

namespace EPloy.Game.Res
{
    
    /// <summary>
    /// 资源检查器。
    /// </summary>
    internal sealed partial class ResChecker
    {
        private ResUpdaterModule ResUpdater;
        private ResStore ResStore;
        private UpdatableVersionListSerializer UpdatableVersionListSerializer;
        private LocalVersionListSerializer LocalVersionListSerializer;
        private readonly Dictionary<ResName, CheckInfo> CheckInfos;
        private string VersionListPath
        {
            get
            {
                return GameModule.ResPath;
            }
        }
        private bool RemoteVersionListReady;
        private bool LocalVersionListReady;
        public ResCheckerCallBack ResCheckerCallBack;

        public ResChecker(ResUpdaterModule resUpdater, ResStore resStore)
        {
            ResUpdater = resUpdater; ResStore = resStore;
            CheckInfos = new Dictionary<ResName, CheckInfo>();
            ResCheckerCallBack = null;
            RemoteVersionListReady = false;
            LocalVersionListReady = false;
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            LocalVersionListSerializer = new LocalVersionListSerializer();
        }

        public void OnDestroy()
        {
            CheckInfos.Clear();
        }

        public void CheckResources()
        {
            TryRecoverReadWriteVersionList();
            string versionListUri = UtilPath.GetRemotePath(Path.Combine(VersionListPath, ConfigVersion.LocalVersionListFileName));
            GameStart.Instance.StartCoroutine(LocalVersionList(versionListUri));

            string RemoteVersionListUrl = UtilPath.GetRemotePath(Path.Combine(VersionListPath, ConfigVersion.RemoteVersionListFileName));
            GameStart.Instance.StartCoroutine(RemoteVersionList(RemoteVersionListUrl));

        }

        /// <summary>
        /// 暂时给单机用
        /// </summary>
        public void CheckPackageResources()
        {
            TryRecoverReadWriteVersionList();
            string versionListUri = UtilPath.GetRemotePath(Path.Combine(VersionListPath, ConfigVersion.RemoteVersionListFileName));
            GameStart.Instance.StartCoroutine(PackageVersionList(versionListUri));
        }

        private bool TryRecoverReadWriteVersionList()
        {
            string file = UtilPath.GetRegularPath(Path.Combine(VersionListPath, ConfigVersion.LocalVersionListFileName));
            string backupFile = UtilText.Format("{0}.{1}", file, ResUpdater.BackupExtension);

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

        private IEnumerator LocalVersionList(string fileUri)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
            if (!isError)
            {
                bytes = UtilZip.Decompress(bytes);
                OnLocalVersionListSuccess(fileUri, bytes);
            }
            else
            {
                OnLocalVersionListFailure(fileUri, errorMessage);
            }
        }

        private IEnumerator PackageVersionList(string fileUri)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
            if (!isError)
            {
                bytes = UtilZip.Decompress(bytes);
                OnPackageVersionListSuccess(fileUri, bytes);
            }
            else
            {
                OnPackageVersionListFailure(fileUri, errorMessage);
            }
        }

        private IEnumerator RemoteVersionList(string fileUri)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
            if (!isError)
            {
                bytes = UtilZip.Decompress(bytes);
                OnRemoteVersionListSuccess(fileUri, bytes);
            }
            else
            {
                OnRemoteVersionListFailure(fileUri, errorMessage);
            }
        }

        private void OnLocalVersionListSuccess(string fileUri, byte[] bytes)
        {
            if (LocalVersionListReady)
            {
                Log.Fatal("Read write version list has been parsed.");
                return;
            }
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes, false);
                LocalVersionList versionList = LocalVersionListSerializer.Deserialize(memoryStream);
                if (!versionList.IsValid)
                {
                    Log.Fatal("Deserialize read write version list failure.");
                }

                foreach (LocalVersionList.FileSystem fileSystem in versionList.FileSystems)
                {
                    foreach (int resourceIndex in fileSystem.ResourceIndexes)
                    {
                        LocalVersionList.Resource resource = versionList.Resources[resourceIndex];
                        SetCachedFileSystemName(new ResName(resource.Name), fileSystem.Name);
                    }
                }

                foreach (LocalVersionList.Resource resource in versionList.Resources)
                {
                    ResName ResName = new ResName(resource.Name);
                    SetReadWriteInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode);
                }

                LocalVersionListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                Log.Fatal(UtilText.Format("Parse read write version list exception '{0}'.", exception.ToString()));
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

        private void OnLocalVersionListFailure(string fileUri, string errorMessage)
        {
            if (LocalVersionListReady)
            {
                Log.Fatal("Read write version list has been parsed.");
                return;
            }

            LocalVersionListReady = true;
            RefreshCheckInfoStatus();
        }

        private void OnPackageVersionListSuccess(string fileUri, byte[] bytes)
        {
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes, false);
                UpdatableVersionList versionList = UpdatableVersionListSerializer.Deserialize(memoryStream);
                if (!versionList.IsValid)
                {
                    Log.Fatal("Deserialize package version list failure.");
                    return;
                }

                UpdatableVersionList.Asset[] assets = versionList.GetAssets();
                UpdatableVersionList.Resource[] resources = versionList.GetResources();
                UpdatableVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
                UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
                ResStore.SetResVersion(versionList.ApplicableGameVersion, versionList.InternalResourceVersion);
                ResStore.VersionInfos = new Dictionary<string, AssetInfo>(assets.Length, StringComparer.Ordinal);
                ResStore.ResInfos = new Dictionary<ResName, ResInfo>(resources.Length, new ResNameComparer());
                ResStore.ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>(new ResNameComparer());

                foreach (UpdatableVersionList.FileSystem fileSystem in fileSystems)
                {
                    int[] resourceIndexes = fileSystem.GetResourceIndexes();
                    foreach (int resourceIndex in resourceIndexes)
                    {
                        UpdatableVersionList.Resource resource = resources[resourceIndex];
                        SetCachedFileSystemName(new ResName(resource.Name), fileSystem.Name);
                    }
                }

                foreach (UpdatableVersionList.Resource resource in resources)
                {
                    //Log.Error("资源检测： " + resource.Name);
                    ResName ResName = new ResName(resource.Name);
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

                        ResStore.VersionInfos.Add(asset.Name, new AssetInfo(asset.Name, ResName, dependencyAssetNames));
                    }
                    ResStore.ResInfos.Add(ResName, new ResInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode, true));
                    SetVersionInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode, resource.ZipLength, resource.ZipHashCode);
                }

                foreach (UpdatableVersionList.ResourceGroup resourceGroup in resourceGroups)
                {
                    ResGroup group = ResStore.GetOrAddResourceGroup(resourceGroup.Name);
                    int[] resourceIndexes = resourceGroup.GetResourceIndexes();
                    foreach (int resourceIndex in resourceIndexes)
                    {
                        UpdatableVersionList.Resource resource = resources[resourceIndex];
                        group.AddResource(new ResName(resource.Name), resource.Length, resource.ZipLength);
                    }
                }
                ResCheckerCallBack.CheckComplete(0, 0, 0, 0, 0);
            }
            catch (Exception exception)
            {
                Log.Fatal(UtilText.Format("Parse package version list exception '{0}'.", exception.ToString()));
            }
            finally
            {
                //CachedFileSystemNames.Clear();
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }
        }

        private void OnPackageVersionListFailure(string fileUri, string errorMessage)
        {
            Log.Fatal(UtilText.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
        }

        private void OnRemoteVersionListSuccess(string fileUri, byte[] bytes)
        {
            if (RemoteVersionListReady)
            {
                Log.Fatal("Remote version list has been parsed.");
                return;
            }

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes, false);
                UpdatableVersionList versionList = UpdatableVersionListSerializer.Deserialize(memoryStream);
                if (!versionList.IsValid)
                {
                    Log.Fatal("Deserialize updatable version list failure.");
                    return;
                }

                UpdatableVersionList.Asset[] assets = versionList.GetAssets();
                UpdatableVersionList.Resource[] resources = versionList.GetResources();
                UpdatableVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
                UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
                ResStore.SetResVersion(versionList.ApplicableGameVersion, versionList.InternalResourceVersion);
                ResStore.VersionInfos = new Dictionary<string, AssetInfo>(assets.Length, StringComparer.Ordinal);
                ResStore.ResInfos = new Dictionary<ResName, ResInfo>(resources.Length, new ResNameComparer());
                ResStore.ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>(new ResNameComparer());
                ResGroup defaultResourceGroup = ResStore.GetOrAddResourceGroup(string.Empty);

                foreach (UpdatableVersionList.FileSystem fileSystem in fileSystems)
                {
                    int[] resourceIndexes = fileSystem.GetResourceIndexes();
                    foreach (int resourceIndex in resourceIndexes)
                    {
                        UpdatableVersionList.Resource resource = resources[resourceIndex];
                        SetCachedFileSystemName(new ResName(resource.Name ), fileSystem.Name);
                    }
                }

                foreach (UpdatableVersionList.Resource resource in resources)
                {
                    ResName ResName = new ResName(resource.Name);
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

                        ResStore.VersionInfos.Add(asset.Name, new AssetInfo(asset.Name, ResName, dependencyAssetNames));
                    }

                    SetVersionInfo(ResName, (LoadType)resource.LoadType, resource.Length, resource.HashCode, resource.ZipLength, resource.ZipHashCode);
                    defaultResourceGroup.AddResource(ResName, resource.Length, resource.ZipLength);
                }

                foreach (UpdatableVersionList.ResourceGroup resourceGroup in resourceGroups)
                {
                    ResGroup group = ResStore.GetOrAddResourceGroup(resourceGroup.Name);
                    int[] resourceIndexes = resourceGroup.GetResourceIndexes();
                    foreach (int resourceIndex in resourceIndexes)
                    {
                        UpdatableVersionList.Resource resource = resources[resourceIndex];
                        group.AddResource(new ResName(resource.Name), resource.Length, resource.ZipLength);
                    }
                }

                RemoteVersionListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                Log.Fatal(UtilText.Format("Parse updatable version list exception '{0}'.", exception.ToString()));
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

        private void OnRemoteVersionListFailure(string fileUri, string errorMessage)
        {
            Log.Fatal(UtilText.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
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
            if (!RemoteVersionListReady || !LocalVersionListReady)
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
                ci.RefreshStatus();
                if (ci.Status == CheckStatus.StorageInReadWrite)
                {
                    if (ci.NeedMoveToDisk || ci.NeedMoveToFileSystem)
                    {
                        movedCount++;
                        string name = ci.ResName.Name;
                        string resourcePath = UtilPath.GetRegularPath(Path.Combine(VersionListPath, name));
                        if (ci.NeedMoveToDisk)
                        {
                            IFileSystem fileSystem = ResUpdater.GetFileSystem(ci.ReadWriteFileSystemName, false);
                            if (!fileSystem.SaveAsFile(name, resourcePath))
                            {
                                Log.Fatal(UtilText.Format("Save as file '{0}' to '{1}' from file system '{2}' error.", resourcePath, fileSystem.FullPath));
                                return;
                            }

                            fileSystem.DeleteFile(resourcePath);
                        }

                        if (ci.NeedMoveToFileSystem)
                        {
                            IFileSystem fileSystem = ResUpdater.GetFileSystem(ci.FileSystemName, false);
                            if (!fileSystem.WriteFile(name, resourcePath))
                            {
                                Log.Fatal(UtilText.Format("Write resource '{0}' to file system '{1}' error.", resourcePath, fileSystem.FullPath));
                                return;
                            }

                            if (File.Exists(resourcePath))
                            {
                                File.Delete(resourcePath);
                            }
                        }
                    }
                    ResStore.ReadWriteResInfos.Add(ci.ResName, new ReadWriteResInfo(ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode));
                }
                else if (ci.Status == CheckStatus.Update)
                {
                    updateCount++;
                    updateTotalLength += ci.Length;
                    updateTotalZipLength += ci.ZipLength;
                    if (ResCheckerCallBack.NeedUpdate != null)
                    {
                        ResCheckerCallBack.NeedUpdate(ci.ResName, ci.FileSystemName, ci.LoadType, ci.Length, ci.HashCode, ci.ZipLength, ci.ZipHashCode);
                    }
                }
                else if (ci.Status == CheckStatus.Unavailable || ci.Status == CheckStatus.Disuse)
                {
                    // Do nothing.
                }
                else
                {
                    Log.Fatal(UtilText.Format("Check resources '{0}' error with unknown status.", ci.ResName.Name));
                }

                if (ci.NeedRemove)
                {
                    removedCount++;
                    if (ci.ReadWriteUseFileSystem)
                    {
                        IFileSystem fileSystem = ResUpdater.GetFileSystem(ci.ReadWriteFileSystemName, false);
                        fileSystem.DeleteFile(ci.ResName.Name);
                    }
                    else
                    {
                        string resourcePath = UtilPath.GetRegularPath(Path.Combine(VersionListPath, ci.ResName.Name));
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
                UtilPath.RemoveEmptyDirectory(VersionListPath);
            }
            if (ResCheckerCallBack.CheckComplete != null)
            {
                ResCheckerCallBack.CheckComplete(movedCount, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
            }
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

                    GameModule.FileSystem.DestroyFileSystem(fileSystem.Value, true);
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

    }
}
