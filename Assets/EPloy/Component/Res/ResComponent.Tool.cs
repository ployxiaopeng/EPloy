
using EPloy.Res;
using System;
using System.Collections.Generic;


namespace EPloy
{
    public partial class ResComponent : Component
    {
        public PackVersionListSerializer PackVersionListSerializer { get; private set; }
        public UpdatableVersionListSerializer UpdatableVersionListSerializer { get; private set; }
        public LocalVersionListSerializer LocalVersionListSerializer { get; private set; }
        // 资源文件名
        private Dictionary<ResName, string> m_CachedFileSystemNames;


        /// <summary>
        /// 资源工具
        /// </summary>
        private void InitTool()
        {
            LocalVersionListSerializer = new LocalVersionListSerializer();
            PackVersionListSerializer = new PackVersionListSerializer();
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            m_CachedFileSystemNames = new Dictionary<ResName, string>();
        }

        // /// <summary>
        // /// 初始化资源列表
        // /// </summary>
        // public void InitResList(string versionListFileName)
        // {
        //     m_CurrentVariant = currentVariant;

        //     if (m_ResourceManager.m_ResourceHelper == null)
        //     {
        //         throw new GameFrameworkException("Resource helper is invalid.");
        //     }

        //     if (string.IsNullOrEmpty(m_ResourceManager.m_ReadOnlyPath))
        //     {
        //         throw new GameFrameworkException("Readonly path is invalid.");
        //     }

        //     LoadBinary(Utility.Path.GetRemotePath(Path.Combine(m_ResourceManager.m_ReadOnlyPath, RemoteVersionListFileName)), new LoadBytesCallbacks(OnLoadPackageVersionListSuccess, OnLoadPackageVersionListFailure), null);
        // }

        // private void OnLoadPackageVersionListSuccess(string fileUri, byte[] bytes, float duration, object userData)
        // {
        //     MemoryStream memoryStream = null;
        //     try
        //     {
        //         memoryStream = new MemoryStream(bytes, false);
        //         PackageVersionList versionList = m_ResourceManager.m_PackageVersionListSerializer.Deserialize(memoryStream);
        //         if (!versionList.IsValid)
        //         {
        //             throw new GameFrameworkException("Deserialize package version list failure.");
        //         }

        //         PackageVersionList.Asset[] assets = versionList.GetAssets();
        //         PackageVersionList.Resource[] resources = versionList.GetResources();
        //         PackageVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
        //         PackageVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
        //         m_ResourceManager.m_ApplicableGameVersion = versionList.ApplicableGameVersion;
        //         m_ResourceManager.m_InternalResourceVersion = versionList.InternalResourceVersion;
        //         m_ResourceManager.m_AssetInfos = new Dictionary<string, AssetInfo>(assets.Length, StringComparer.Ordinal);
        //         m_ResourceManager.m_ResourceInfos = new Dictionary<ResourceName, ResourceInfo>(resources.Length, new ResourceNameComparer());
        //         ResourceGroup defaultResourceGroup = m_ResourceManager.GetOrAddResourceGroup(string.Empty);

        //         foreach (PackageVersionList.FileSystem fileSystem in fileSystems)
        //         {
        //             int[] resourceIndexes = fileSystem.GetResourceIndexes();
        //             foreach (int resourceIndex in resourceIndexes)
        //             {
        //                 PackageVersionList.Resource resource = resources[resourceIndex];
        //                 if (resource.Variant != null && resource.Variant != m_CurrentVariant)
        //                 {
        //                     continue;
        //                 }

        //                 m_CachedFileSystemNames.Add(new ResourceName(resource.Name, resource.Variant, resource.Extension), fileSystem.Name);
        //             }
        //         }

        //         foreach (PackageVersionList.Resource resource in resources)
        //         {
        //             if (resource.Variant != null && resource.Variant != m_CurrentVariant)
        //             {
        //                 continue;
        //             }

        //             ResourceName resourceName = new ResourceName(resource.Name, resource.Variant, resource.Extension);
        //             int[] assetIndexes = resource.GetAssetIndexes();
        //             foreach (int assetIndex in assetIndexes)
        //             {
        //                 PackageVersionList.Asset asset = assets[assetIndex];
        //                 int[] dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
        //                 int index = 0;
        //                 string[] dependencyAssetNames = new string[dependencyAssetIndexes.Length];
        //                 foreach (int dependencyAssetIndex in dependencyAssetIndexes)
        //                 {
        //                     dependencyAssetNames[index++] = assets[dependencyAssetIndex].Name;
        //                 }

        //                 m_ResourceManager.m_AssetInfos.Add(asset.Name, new AssetInfo(asset.Name, resourceName, dependencyAssetNames));
        //             }

        //             string fileSystemName = null;
        //             if (!m_CachedFileSystemNames.TryGetValue(resourceName, out fileSystemName))
        //             {
        //                 fileSystemName = null;
        //             }

        //             m_ResourceManager.m_ResourceInfos.Add(resourceName, new ResourceInfo(resourceName, fileSystemName, (LoadType)resource.LoadType, resource.Length, resource.HashCode, true, true));
        //             defaultResourceGroup.AddResource(resourceName, resource.Length, resource.Length);
        //         }

        //         foreach (PackageVersionList.ResourceGroup resourceGroup in resourceGroups)
        //         {
        //             ResourceGroup group = m_ResourceManager.GetOrAddResourceGroup(resourceGroup.Name);
        //             int[] resourceIndexes = resourceGroup.GetResourceIndexes();
        //             foreach (int resourceIndex in resourceIndexes)
        //             {
        //                 PackageVersionList.Resource resource = resources[resourceIndex];
        //                 if (resource.Variant != null && resource.Variant != m_CurrentVariant)
        //                 {
        //                     continue;
        //                 }

        //                 group.AddResource(new ResourceName(resource.Name, resource.Variant, resource.Extension), resource.Length, resource.Length);
        //             }
        //         }

        //         ResourceInitComplete();
        //     }
        //     catch (Exception exception)
        //     {
        //         if (exception is EPloyException)
        //         {
        //             throw;
        //         }

        //         throw new EPloyException(Utility.Text.Format("Parse package version list exception '{0}'.", exception.ToString()), exception);
        //     }
        //     finally
        //     {
        //         m_CachedFileSystemNames.Clear();
        //         if (memoryStream != null)
        //         {
        //             memoryStream.Dispose();
        //             memoryStream = null;
        //         }
        //     }
        // }

        private void OnLoadPackageVersionListFailure(string fileUri, string errorMessage, object userData)
        {
            throw new EPloyException(Utility.Text.Format("Package version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
        }

        /// <summary>
        /// 使用可更新模式并检查版本资源列表。
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号。</param>
        /// <returns>检查版本资源列表结果。</returns>
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {
            return CheckVersionListResult.Updated;
        }

    }
}
