using System;
using System.Collections.Generic;
using System.IO;
using EPloy.SystemFile;

namespace EPloy.Res
{
    /// <summary>
    /// 资源储存器
    /// </summary>
    internal sealed class ResStore
    {
        public string ApplicableGameVersion { get; private set; }
        public int InternalResVersion { get; private set; }
        /// <summary>
        /// 资产信息 版本检验用
        /// </summary>
        internal Dictionary<string, AssetInfo> VersionInfos;
        /// <summary>
        /// 资源信息 local资源加载用
        /// </summary>
        internal Dictionary<ResName, ResInfo> ResInfos;
        /// <summary>
        /// 资源读写信息  
        /// </summary>
        internal SortedDictionary<ResName, ReadWriteResInfo> ReadWriteResInfos;
        /// <summary>
        /// 资源组写信息  
        /// </summary>
        internal Dictionary<string, ResGroup> ResGroups;

        internal ResStore()
        {
            VersionInfos = new Dictionary<string, AssetInfo>();
            ResInfos = new Dictionary<ResName, ResInfo>();
            ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>();
            ResGroups = new Dictionary<string, ResGroup>();
        }

        internal void SetResVersion(string applicableGameVersion, int internalResVersion)
        {
            ApplicableGameVersion = applicableGameVersion;
            InternalResVersion = internalResVersion;
        }

        internal ResGroup GetOrAddResourceGroup(string resGroupName)
        {
            if (resGroupName == null)
            {
                resGroupName = string.Empty;
            }

            ResGroup resourceGroup = null;
            if (!ResGroups.TryGetValue(resGroupName, out resourceGroup))
            {
                resourceGroup = new ResGroup(resGroupName, ResInfos);
                ResGroups.Add(resGroupName, resourceGroup);
            }

            return resourceGroup;
        }

        /// <summary>
        /// 获取资源组。
        /// </summary>
        /// <param name="resourceGroupName">要获取的资源组名称。</param>
        /// <returns>要获取的资源组。</returns>
        internal ResGroup GetResGroup(string resourceGroupName)
        {
            ResGroup resourceGroup = null;
            if (ResGroups.TryGetValue(resourceGroupName ?? string.Empty, out resourceGroup))
            {
                return resourceGroup;
            }

            return null;
        }

        internal AssetInfo GetAssetInfo(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.Fatal("Asset name is invalid.");
                return null;
            }

            if (VersionInfos == null)
            {
                return null;
            }

            AssetInfo assetInfo = null;
            if (VersionInfos.TryGetValue(assetName, out assetInfo))
            {
                return assetInfo;
            }

            return null;
        }
        internal ResInfo GetResInfo(ResName resName)
        {
            if (ResInfos == null)
            {
                return null;
            }

            ResInfo resInfo = null;
            if (ResInfos.TryGetValue(resName, out resInfo))
            {
                return resInfo;
            }

            return null;
        }
        internal ResInfo GetResInfo(string assetName)
        {
            if (ResInfos == null)
            {
                return null;
            }

            AssetInfo assetInfo = GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return null;
            }

            ResInfo resInfo = null;
            if (ResInfos.TryGetValue(resInfo.ResName, out resInfo))
            {
                return resInfo;
            }

            return null;
        }
    }
}
