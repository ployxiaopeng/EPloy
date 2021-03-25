using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;
using EPloy.SystemFile;

namespace EPloy.Res
{
    /// <summary>
    /// 资源储存器
    /// </summary>
    public sealed class ResStore
    {
        private static ResStore instance = null;
        public static ResStore CreateResStore()
        {
            if (instance == null) instance = new ResStore();
            return instance;
        }
        internal static ResStore Instance
        {
            get
            {
                return CreateResStore();
            }
        }


        internal PackVersionListSerializer PackVersionListSerializer { get; private set; }
        internal UpdatableVersionListSerializer UpdatableVersionListSerializer { get; private set; }
        internal LocalVersionListSerializer LocalVersionListSerializer { get; private set; }

        public ObjectPoolBase AssetPool { get; private set; }
        public ObjectPoolBase ResourcePool { get; private set; }

        /// <summary>
        /// 资产信息 版本检验用
        /// </summary>
        internal Dictionary<string, AssetInfo> VersionInfos;
        /// <summary>
        /// 资源信息 local资源加载用
        /// </summary>
        internal Dictionary<ResName, ResInfo> ResInfos;
        /// <summary>
        /// 资源读写信息  看代码就资源更新的时候用了
        /// </summary>
        internal SortedDictionary<ResName, ReadWriteResInfo> ReadWriteResInfos;
        /// <summary>
        /// 资源组写信息  看代码就资源更新的时候用了
        /// </summary>
        internal Dictionary<string, ResGroup> ResGroups;


        private ResStore()
        {
            VersionInfos = new Dictionary<string, AssetInfo>();
            ResInfos = new Dictionary<ResName, ResInfo>();
            ReadWriteResInfos = new SortedDictionary<ResName, ReadWriteResInfo>();
            ResGroups = new Dictionary<string, ResGroup>();
            LocalVersionListSerializer = new LocalVersionListSerializer();
            PackVersionListSerializer = new PackVersionListSerializer();
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            SetObjectPoolManager();
        }

        /// <summary>
        /// 设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        private void SetObjectPoolManager()
        {
            AssetPool = GameEntry.ObjectPool.CreateObjectPool(typeof(AssetObject), "AssetPool");
            ResourcePool = GameEntry.ObjectPool.CreateObjectPool(typeof(AssetObject), "ResPool");
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

        internal AssetInfo GetAssetInfo(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new EPloyException("Asset name is invalid.");
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
