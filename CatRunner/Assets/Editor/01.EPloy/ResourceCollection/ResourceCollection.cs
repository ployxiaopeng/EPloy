
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace EPloy.Editor.ResourceTools
{
    /// <summary>
    /// 资源集合。
    /// </summary>
    public sealed class ResourceCollection
    {
        private const string SceneExtension = ".unity";
        private static readonly Regex ResourceNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");
        private static readonly Regex ResourceVariantRegex = new Regex(@"^[a-z0-9_-]+$");

        private readonly string m_ConfigurationPath;
        private readonly SortedDictionary<string, Resource> m_Resources;
        private readonly SortedDictionary<string, Asset> m_Assets;

        public ResourceCollection()
        {
            m_ConfigurationPath =UtilPath.GetRegularPath(Path.Combine(Application.dataPath, EPloyEditorPath.ResCollection));
            m_Resources = new SortedDictionary<string, Resource>(StringComparer.Ordinal);
            m_Assets = new SortedDictionary<string, Asset>(StringComparer.Ordinal);
        }

        public int ResourceCount
        {
            get
            {
                return m_Resources.Count;
            }
        }

        public int AssetCount
        {
            get
            {
                return m_Assets.Count;
            }
        }

        public event Action<int, int> OnLoadingResource = null;

        public event Action<int, int> OnLoadingAsset = null;

        public event Action OnLoadCompleted = null;

        public void Clear()
        {
            m_Resources.Clear();
            m_Assets.Clear();
        }

        public bool Load()
        {
            Clear();

            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("EPloy");
                XmlNode xmlCollection = xmlRoot.SelectSingleNode("ResCollection");
                XmlNode xmlResources = xmlCollection.SelectSingleNode("Res");
                XmlNode xmlAssets = xmlCollection.SelectSingleNode("Assets");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;
                int count = 0;

                xmlNodeList = xmlResources.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingResource != null)
                    {
                        OnLoadingResource(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Res")
                    {
                        continue;
                    }

                    string name = xmlNode.Attributes.GetNamedItem("Name").Value;
                    string extension = xmlNode.Attributes.GetNamedItem("Extension").Value;
                    byte loadType = 0;
                    if (xmlNode.Attributes.GetNamedItem("LoadType") != null)
                    {
                        byte.TryParse(xmlNode.Attributes.GetNamedItem("LoadType").Value, out loadType);
                    }

                    string[] resourceGroups = xmlNode.Attributes.GetNamedItem("ResGroups") != null ? xmlNode.Attributes.GetNamedItem("ResGroups").Value.Split(',') : null;
                    if (!AddResource(string.Format("{0}.{1}", name, extension) , (LoadType)loadType, resourceGroups))
                    {
                       Debug.LogWarning(UtilText.Format("Can not add resource '{0}'.", name));
                        continue;
                    }
                }

                xmlNodeList = xmlAssets.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingAsset != null)
                    {
                        OnLoadingAsset(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Asset")
                    {
                        continue;
                    }

                    string guid = xmlNode.Attributes.GetNamedItem("Guid").Value;
                    string name = xmlNode.Attributes.GetNamedItem("ResName").Value;
                    if (!AssignAsset(guid, name))
                    {
                        Debug.LogWarning(UtilText.Format("Can not assign asset '{0}' to resource '{1}'.", guid, name));
                        continue;
                    }
                }

                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return true;
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return false;
            }
        }

        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("EPloy");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlCollection = xmlDocument.CreateElement("ResCollection");
                xmlRoot.AppendChild(xmlCollection);

                XmlElement xmlResources = xmlDocument.CreateElement("Res");
                xmlCollection.AppendChild(xmlResources);

                XmlElement xmlAssets = xmlDocument.CreateElement("Assets");
                xmlCollection.AppendChild(xmlAssets);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                foreach (Resource resource in m_Resources.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Res");
                    xmlAttribute = xmlDocument.CreateAttribute("Name");
                    xmlAttribute.Value = resource.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("Extension");
                    xmlAttribute.Value = resource.Extension;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);

                    xmlAttribute = xmlDocument.CreateAttribute("LoadType");
                    xmlAttribute.Value = ((byte)resource.LoadType).ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    string[] resourceGroups = resource.GetResourceGroups();
                    if (resourceGroups.Length > 0)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("ResGroups");
                        xmlAttribute.Value = string.Join(",", resourceGroups);
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlResources.AppendChild(xmlElement);
                }

                foreach (Asset asset in m_Assets.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Asset");
                    xmlAttribute = xmlDocument.CreateAttribute("Guid");
                    xmlAttribute.Value = asset.Guid;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("ResName");
                    xmlAttribute.Value = asset.Resource.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);


                    xmlAssets.AppendChild(xmlElement);
                }

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }
        }

        public Resource[] GetResources()
        {
            return m_Resources.Values.ToArray();
        }

        public Resource GetResource(string name)
        {
            if (!IsValidResourceName(name))
            {
                return null;
            }

            Resource resource = null;
            if (m_Resources.TryGetValue(name.ToLowerInvariant(), out resource))
            {
                return resource;
            }

            return null;
        }

        public bool HasResource(string name)
        {
            if (!IsValidResourceName(name))
            {
                return false;
            }

            return m_Resources.ContainsKey(name.ToLowerInvariant());
        }

        public bool AddResource(string fullName, LoadType loadType)
        {
            return AddResource(fullName, loadType,null);
        }

        public bool AddResource(string fullName, LoadType loadType, string[] resourceGroups)
        {
            Resource resource = Resource.Create(fullName, loadType, resourceGroups);
            if (!IsValidResourceName(resource.Name))
            {
                resource = null;
                return false;
            }

            if (!IsAvailableResourceName(resource.Name, null))
            {
                resource = null;
                return false;
            }
            m_Resources.Add(resource.Name.ToLowerInvariant(), resource);

            return true;
        }

        public bool RenameResource(string oldName,string newName)
        {
            if (!IsValidResourceName(oldName) || !IsValidResourceName(newName))
            {
                return false;
            }

            Resource resource = GetResource(oldName);
            if (resource == null)
            {
                return false;
            }

            if (oldName == newName)
            {
                return true;
            }

            if (!IsAvailableResourceName(newName, resource))
            {
                return false;
            }

            m_Resources.Remove(resource.Name.ToLowerInvariant());
            resource.Rename(newName);
            m_Resources.Add(resource.Name.ToLowerInvariant(), resource);

            return true;
        }

        public bool RemoveResource(string name)
        {
            if (!IsValidResourceName(name))
            {
                return false;
            }

            Resource resource = GetResource(name);
            if (resource == null)
            {
                return false;
            }

            Asset[] assets = resource.GetAssets();
            resource.Clear();
            m_Resources.Remove(resource.Name.ToLowerInvariant());
            foreach (Asset asset in assets)
            {
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }

        public bool SetResourceLoadType(string name, LoadType loadType)
        {
            if (!IsValidResourceName(name))
            {
                return false;
            }

            Resource resource = GetResource(name);
            if (resource == null)
            {
                return false;
            }

            if ((loadType == LoadType.LoadFromBinary) && resource.GetAssets().Length > 1)
            {
                return false;
            }

            resource.LoadType = loadType;
            return true;
        }

        public Asset[] GetAssets()
        {
            return m_Assets.Values.ToArray();
        }

        public Asset[] GetAssets(string name)
        {
            if (!IsValidResourceName(name))
            {
                return new Asset[0];
            }

            Resource resource = GetResource(name);
            if (resource == null)
            {
                return new Asset[0];
            }

            return resource.GetAssets();
        }

        public Asset GetAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            Asset asset = null;
            if (m_Assets.TryGetValue(guid, out asset))
            {
                return asset;
            }

            return null;
        }

        public bool HasAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            return m_Assets.ContainsKey(guid);
        }

        public bool AssignAsset(string guid, string name)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            if (!IsValidResourceName(name))
            {
                return false;
            }

            Resource resource = GetResource(name);
            if (resource == null)
            {
                return false;
            }

            string assetName = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            Asset[] assetsInResource = resource.GetAssets();
            foreach (Asset assetInResource in assetsInResource)
            {
                if (assetInResource.FullName.ToLowerInvariant() == assetName.ToLowerInvariant())
                {
                    return false;
                }
            }

            bool isScene = assetName.EndsWith(SceneExtension, StringComparison.Ordinal);
            if (isScene && resource.AssetType == AssetType.Asset || !isScene && resource.AssetType == AssetType.Scene)
            {
                return false;
            }

            Asset asset = GetAsset(guid);
            if (resource.IsLoadFromBinary && assetsInResource.Length > 0 && asset != assetsInResource[0])
            {
                return false;
            }

            if (asset == null)
            {
                asset = Asset.Create(guid);
                m_Assets.Add(asset.Guid, asset);
            }

            resource.AssignAsset(asset, isScene);

            return true;
        }

        public bool UnassignAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            Asset asset = GetAsset(guid);
            if (asset != null)
            {
                asset.Resource.UnassignAsset(asset);
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }

        private bool IsValidResourceName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!ResourceNameRegex.IsMatch(name))
            {
                return false;
            }
            return true;
        }

        private bool IsAvailableResourceName(string name, Resource current)
        {
            Resource found = GetResource(name);
            if (found != null && found != current)
            {
                return false;
            }

            foreach (Resource resource in m_Resources.Values)
            {
                if (current != null && resource == current)
                {
                    continue;
                }

                if (resource.Name == name)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
