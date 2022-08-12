using System.Collections.Generic;

namespace EPloy.Editor.ResourceTools
{
    /// <summary>
    /// 资源。
    /// </summary>
    public sealed class Resource
    {
        private readonly List<Asset> m_Assets;
        private readonly List<string> m_ResourceGroups;

        private Resource(string fullName, LoadType loadType, string[] resourceGroups)
        {
            m_Assets = new List<Asset>();
            m_ResourceGroups = new List<string>();
            int dotIndex = fullName.IndexOf('.');
            Name = dotIndex > 0 ? fullName.Substring(0, dotIndex) : fullName;
            Extension= dotIndex > 0 ? fullName.Substring(dotIndex+1, fullName.Length- dotIndex-1) : "";
            AssetType = AssetType.Unknown;
            LoadType = loadType;

            foreach (string resourceGroup in resourceGroups)
            {
                AddResourceGroup(resourceGroup);
            }
        }

        public string Name
        {
            get;
            private set;
        }
        public string Extension
        {
            get;
            private set;
        }

        public string FullName
        {
            get
            {
                return string.Format("{0}.{1}", Name, Extension);
            }
        }

        public AssetType AssetType
        {
            get;
            private set;
        }

        public bool IsLoadFromBinary
        {
            get
            {
                return LoadType == LoadType.LoadFromBinary;
            }
        }

        public LoadType LoadType
        {
            get;
            set;
        }


        public static Resource Create(string fullName, LoadType loadType, string[] resourceGroups)
        {
            return new Resource(fullName, loadType, resourceGroups ?? new string[0]);
        }

        public Asset[] GetAssets()
        {
            return m_Assets.ToArray();
        }

        public Asset GetFirstAsset()
        {
            return m_Assets.Count > 0 ? m_Assets[0] : null;
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void AssignAsset(Asset asset, bool isScene)
        {
            if (asset.Resource != null)
            {
                asset.Resource.UnassignAsset(asset);
            }

            AssetType = isScene ? AssetType.Scene : AssetType.Asset;
            asset.Resource = this;
            m_Assets.Add(asset);
            m_Assets.Sort(AssetComparer);
        }

        public void UnassignAsset(Asset asset)
        {
            asset.Resource = null;
            m_Assets.Remove(asset);
            if (m_Assets.Count <= 0)
            {
                AssetType = AssetType.Unknown;
            }
        }

        public string[] GetResourceGroups()
        {
            return m_ResourceGroups.ToArray();
        }

        public bool HasResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return false;
            }

            return m_ResourceGroups.Contains(resourceGroup);
        }

        public void AddResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return;
            }

            if (m_ResourceGroups.Contains(resourceGroup))
            {
                return;
            }

            m_ResourceGroups.Add(resourceGroup);
            m_ResourceGroups.Sort();
        }

        public bool RemoveResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return false;
            }

            return m_ResourceGroups.Remove(resourceGroup);
        }

        public void Clear()
        {
            foreach (Asset asset in m_Assets)
            {
                asset.Resource = null;
            }

            m_Assets.Clear();
            m_ResourceGroups.Clear();
        }

        private int AssetComparer(Asset a, Asset b)
        {
            return a.Guid.CompareTo(b.Guid);
        }
    }
}
