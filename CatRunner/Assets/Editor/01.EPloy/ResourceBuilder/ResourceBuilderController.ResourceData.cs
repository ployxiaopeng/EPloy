
using System.Collections.Generic;

namespace EPloy.Editor.ResourceTools
{
    public sealed partial class ResourceBuilderController
    {
        private sealed class ResourceData
        {
            private readonly string m_Name;
            private readonly string m_Extension;
            private readonly LoadType m_LoadType;
            private readonly string[] m_ResourceGroups;
            private readonly List<AssetData> m_AssetDatas;
            private readonly List<ResourceCode> m_Codes;

            public ResourceData(string name,string extension, LoadType loadType, string[] resourceGroups)
            {
                m_Name = name;
                m_Extension = extension;
                m_LoadType = loadType;
                m_ResourceGroups = resourceGroups;
                m_AssetDatas = new List<AssetData>();
                m_Codes = new List<ResourceCode>();
            }

            public string Name
            {
                get
                {
                    return m_Name;
                }
            }

            public string Extension
            {
                get
                {
                    return m_Extension;
                }
            }
            public string FullName
            {
                get
                {
                    return string.Format("{0}.{1}", Name, Extension); ;
                }
            }

            public bool IsLoadFromBinary
            {
                get
                {
                    return m_LoadType == LoadType.LoadFromBinary;
                }
            }

            public LoadType LoadType
            {
                get
                {
                    return m_LoadType;
                }
            }

            public int AssetCount
            {
                get
                {
                    return m_AssetDatas.Count;
                }
            }

            public string[] GetResourceGroups()
            {
                return m_ResourceGroups;
            }

            public string[] GetAssetGuids()
            {
                string[] assetGuids = new string[m_AssetDatas.Count];
                for (int i = 0; i < m_AssetDatas.Count; i++)
                {
                    assetGuids[i] = m_AssetDatas[i].Guid;
                }

                return assetGuids;
            }

            public string[] GetAssetNames()
            {
                string[] assetNames = new string[m_AssetDatas.Count];
                for (int i = 0; i < m_AssetDatas.Count; i++)
                {
                    assetNames[i] = m_AssetDatas[i].Name;
                }

                return assetNames;
            }

            public AssetData[] GetAssetDatas()
            {
                return m_AssetDatas.ToArray();
            }

            public AssetData GetAssetData(string assetName)
            {
                foreach (AssetData assetData in m_AssetDatas)
                {
                    if (assetData.Name == assetName)
                    {
                        return assetData;
                    }
                }

                return null;
            }

            public void AddAssetData(string guid, string name, int length, int hashCode, string[] dependencyAssetNames)
            {
                m_AssetDatas.Add(new AssetData(guid, name, length, hashCode, dependencyAssetNames));
            }

            public ResourceCode GetCode(Platform platform)
            {
                foreach (ResourceCode code in m_Codes)
                {
                    if (code.Platform == platform)
                    {
                        return code;
                    }
                }

                return null;
            }

            public ResourceCode[] GetCodes()
            {
                return m_Codes.ToArray();
            }

            public void AddCode(Platform platform, int length, int hashCode, int compressedLength, int compressedHashCode)
            {
                m_Codes.Add(new ResourceCode(platform, length, hashCode, compressedLength, compressedHashCode));
            }
        }
    }
}
