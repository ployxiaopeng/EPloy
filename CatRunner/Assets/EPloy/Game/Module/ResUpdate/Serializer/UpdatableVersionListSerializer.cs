using System;
using System.IO;
using System.Text;

namespace EPloy.Res
{
    /// <summary>
    /// 可更新模式版本资源列表序列化器。
    /// </summary>
    public sealed class UpdatableVersionListSerializer : EPloySerializer<UpdatableVersionList>
    {
        private readonly byte[] Header = new byte[] { (byte)'E', (byte)'P', (byte)'U' };

        /// <summary>
        /// 获取可更新模式版本资源列表头标识。
        /// </summary>
        /// <returns>可更新模式版本资源列表头标识。</returns>
        protected override byte[] GetHeader()
        {
            return Header;
        }

        /// <summary>
        /// 初始化可更新模式版本资源列表序列化器的新实例。
        /// </summary>
        public UpdatableVersionListSerializer()
        {
            base.RegisterSerializeCallback(0, UpdatableVersionListSerializeCallback_V0);
            base.RegisterSerializeCallback(1, UpdatableVersionListSerializeCallback_V1);

            base.RegisterDeserializeCallback(0, UpdatableVersionListDeserializeCallback_V0);
            base.RegisterDeserializeCallback(1, UpdatableVersionListDeserializeCallback_V1);
            base.RegisterTryGetValueCallback(0, UpdatableVersionListTryGetValueCallback_V0);
            base.RegisterTryGetValueCallback(1, UpdatableVersionListTryGetValueCallback_V1_V2);
        }

        /// <summary>
        /// 序列化可更新模式版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的可更新模式版本资源列表（版本 0）。</param>
        /// <returns>是否序列化可更新模式版本资源列表（版本 0）成功。</returns>
        private bool UpdatableVersionListSerializeCallback_V0(Stream stream, UpdatableVersionList versionList)
        {
            if (!versionList.IsValid)
            {
                return false;
            }

            UtilRandom.GetRandomBytes(s_CachedHashBytes);
            using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                binaryWriter.WriteEncryptedString(versionList.ApplicableGameVersion, s_CachedHashBytes);
                binaryWriter.Write(versionList.InternalResourceVersion);
                UpdatableVersionList.Asset[] assets = versionList.GetAssets();
                binaryWriter.Write(assets.Length);
                UpdatableVersionList.Resource[] resources = versionList.GetResources();
                binaryWriter.Write(resources.Length);
                foreach (UpdatableVersionList.Resource resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                    binaryWriter.Write(resource.ZipLength);
                    binaryWriter.Write(resource.ZipHashCode);
                    int[] assetIndexes = resource.GetAssetIndexes();
                    binaryWriter.Write(assetIndexes.Length);
                    byte[] hashBytes = new byte[CachedHashBytesLength];
                    foreach (int assetIndex in assetIndexes)
                    {
                        UtilConverter.GetBytes(resource.HashCode, hashBytes);
                        UpdatableVersionList.Asset asset = assets[assetIndex];
                        binaryWriter.WriteEncryptedString(asset.Name, hashBytes);
                        int[] dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
                        binaryWriter.Write(dependencyAssetIndexes.Length);
                        foreach (int dependencyAssetIndex in dependencyAssetIndexes)
                        {
                            binaryWriter.WriteEncryptedString(assets[dependencyAssetIndex].Name, hashBytes);
                        }
                    }
                }

                UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
                binaryWriter.Write(resourceGroups.Length);
                foreach (UpdatableVersionList.ResourceGroup resourceGroup in resourceGroups)
                {
                    binaryWriter.WriteEncryptedString(resourceGroup.Name, s_CachedHashBytes);
                    int[] resourceIndexes = resourceGroup.GetResourceIndexes();
                    binaryWriter.Write(resourceIndexes.Length);
                    foreach (ushort resourceIndex in resourceIndexes)
                    {
                        binaryWriter.Write(resourceIndex);
                    }
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        /// 序列化可更新模式版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的可更新模式版本资源列表（版本 1）。</param>
        /// <returns>是否序列化可更新模式版本资源列表（版本 1）成功。</returns>
        private bool UpdatableVersionListSerializeCallback_V1(Stream stream, UpdatableVersionList versionList)
        {
            if (!versionList.IsValid)
            {
                return false;
            }

            UtilRandom.GetRandomBytes(s_CachedHashBytes);
            using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                binaryWriter.WriteEncryptedString(versionList.ApplicableGameVersion, s_CachedHashBytes);
                binaryWriter.Write7BitEncodedInt32(versionList.InternalResourceVersion);
                UpdatableVersionList.Asset[] assets = versionList.GetAssets();
                binaryWriter.Write7BitEncodedInt32(assets.Length);
                foreach (UpdatableVersionList.Asset asset in assets)
                {
                    binaryWriter.WriteEncryptedString(asset.Name, s_CachedHashBytes);
                    int[] dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
                    binaryWriter.Write7BitEncodedInt32(dependencyAssetIndexes.Length);
                    foreach (int dependencyAssetIndex in dependencyAssetIndexes)
                    {
                        binaryWriter.Write7BitEncodedInt32(dependencyAssetIndex);
                    }
                }

                UpdatableVersionList.Resource[] resources = versionList.GetResources();
                binaryWriter.Write7BitEncodedInt32(resources.Length);
                foreach (UpdatableVersionList.Resource resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write7BitEncodedInt32(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                    binaryWriter.Write7BitEncodedInt32(resource.ZipLength);
                    binaryWriter.Write(resource.ZipHashCode);
                    int[] assetIndexes = resource.GetAssetIndexes();
                    binaryWriter.Write7BitEncodedInt32(assetIndexes.Length);
                    foreach (int assetIndex in assetIndexes)
                    {
                        binaryWriter.Write7BitEncodedInt32(assetIndex);
                    }
                }

                UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();
                binaryWriter.Write7BitEncodedInt32(resourceGroups.Length);
                foreach (UpdatableVersionList.ResourceGroup resourceGroup in resourceGroups)
                {
                    binaryWriter.WriteEncryptedString(resourceGroup.Name, s_CachedHashBytes);
                    int[] resourceIndexes = resourceGroup.GetResourceIndexes();
                    binaryWriter.Write7BitEncodedInt32(resourceIndexes.Length);
                    foreach (int resourceIndex in resourceIndexes)
                    {
                        binaryWriter.Write7BitEncodedInt32(resourceIndex);
                    }
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        /// 反序列化可更新模式版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的可更新模式版本资源列表（版本 0）。</returns>
        private UpdatableVersionList UpdatableVersionListDeserializeCallback_V0(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                string applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                int internalResourceVersion = binaryReader.Read7BitEncodedInt32();
                int assetCount = binaryReader.Read7BitEncodedInt32();
                UpdatableVersionList.Asset[] assets = assetCount > 0 ? new UpdatableVersionList.Asset[assetCount] : null;
                for (int i = 0; i < assetCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    int dependencyAssetCount = binaryReader.Read7BitEncodedInt32();
                    int[] dependencyAssetIndexes = dependencyAssetCount > 0 ? new int[dependencyAssetCount] : null;
                    for (int j = 0; j < dependencyAssetCount; j++)
                    {
                        dependencyAssetIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    assets[i] = new UpdatableVersionList.Asset(name, dependencyAssetIndexes);
                }

                int resourceCount = binaryReader.Read7BitEncodedInt32();
                UpdatableVersionList.Resource[] resources = resourceCount > 0 ? new UpdatableVersionList.Resource[resourceCount] : null;
                for (int i = 0; i < resourceCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    byte loadType = binaryReader.ReadByte();
                    int length = binaryReader.Read7BitEncodedInt32();
                    int hashCode = binaryReader.ReadInt32();
                    int compressedLength = binaryReader.Read7BitEncodedInt32();
                    int compressedHashCode = binaryReader.ReadInt32();
                    int assetIndexCount = binaryReader.Read7BitEncodedInt32();
                    int[] assetIndexes = assetIndexCount > 0 ? new int[assetIndexCount] : null;
                    for (int j = 0; j < assetIndexCount; j++)
                    {
                        assetIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    resources[i] = new UpdatableVersionList.Resource(name, loadType, length, hashCode, compressedLength, compressedHashCode, assetIndexes);
                }

                int resourceGroupCount = binaryReader.Read7BitEncodedInt32();
                UpdatableVersionList.ResourceGroup[] resourceGroups = resourceGroupCount > 0 ? new UpdatableVersionList.ResourceGroup[resourceGroupCount] : null;
                for (int i = 0; i < resourceGroupCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    int resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                    int[] resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (int j = 0; j < resourceIndexCount; j++)
                    {
                        resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    resourceGroups[i] = new UpdatableVersionList.ResourceGroup(name, resourceIndexes);
                }

                return new UpdatableVersionList(applicableGameVersion, internalResourceVersion, assets, resources, resourceGroups);
            }
        }


        /// <summary>
        /// 反序列化可更新模式版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的可更新模式版本资源列表（版本 1）。</returns>
        private UpdatableVersionList UpdatableVersionListDeserializeCallback_V1(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                string applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                int internalResourceVersion = binaryReader.Read7BitEncodedInt32();
                int assetCount = binaryReader.Read7BitEncodedInt32();
                UpdatableVersionList.Asset[] assets = assetCount > 0 ? new UpdatableVersionList.Asset[assetCount] : null;
                for (int i = 0; i < assetCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    int dependencyAssetCount = binaryReader.Read7BitEncodedInt32();
                    int[] dependencyAssetIndexes = dependencyAssetCount > 0 ? new int[dependencyAssetCount] : null;
                    for (int j = 0; j < dependencyAssetCount; j++)
                    {
                        dependencyAssetIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    assets[i] = new UpdatableVersionList.Asset(name, dependencyAssetIndexes);
                }

                int resourceCount = binaryReader.Read7BitEncodedInt32();
                UpdatableVersionList.Resource[] resources = resourceCount > 0 ? new UpdatableVersionList.Resource[resourceCount] : null;
                for (int i = 0; i < resourceCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    byte loadType = binaryReader.ReadByte();
                    int length = binaryReader.Read7BitEncodedInt32();
                    int hashCode = binaryReader.ReadInt32();
                    int compressedLength = binaryReader.Read7BitEncodedInt32();
                    int compressedHashCode = binaryReader.ReadInt32();
                    int assetIndexCount = binaryReader.Read7BitEncodedInt32();
                    int[] assetIndexes = assetIndexCount > 0 ? new int[assetIndexCount] : null;
                    for (int j = 0; j < assetIndexCount; j++)
                    {
                        assetIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    resources[i] = new UpdatableVersionList.Resource(name, loadType, length, hashCode, compressedLength, compressedHashCode, assetIndexes);
                }

                int fileSystemCount = binaryReader.Read7BitEncodedInt32();
                UpdatableVersionList.FileSystem[] fileSystems = fileSystemCount > 0 ? new UpdatableVersionList.FileSystem[fileSystemCount] : null;
                for (int i = 0; i < fileSystemCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    int resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                    int[] resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (int j = 0; j < resourceIndexCount; j++)
                    {
                        resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    fileSystems[i] = new UpdatableVersionList.FileSystem(name, resourceIndexes);
                }
                UpdatableVersionList.ResourceGroup[] resourceGroups = null;
                if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    int resourceGroupCount = binaryReader.Read7BitEncodedInt32();
                    resourceGroups = new UpdatableVersionList.ResourceGroup[resourceGroupCount];
                    for (int i = 0; i < resourceGroupCount; i++)
                    {
                        string name = binaryReader.ReadEncryptedString(encryptBytes);
                        int resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                        int[] resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                        for (int j = 0; j < resourceIndexCount; j++)
                        {
                            resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();
                        }

                        resourceGroups[i] = new UpdatableVersionList.ResourceGroup(name, resourceIndexes);
                    }
                } 
                

                return new UpdatableVersionList(applicableGameVersion, internalResourceVersion, assets, resources, resourceGroups);
            }
        }

        /// <summary>
        /// 尝试从可更新模式版本资源列表（版本 0）获取指定键的值回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <param name="key">指定键。</param>
        /// <param name="value">指定键的值。</param>
        /// <returns></returns>
        private bool UpdatableVersionListTryGetValueCallback_V0(Stream stream, string key, out object value)
        {
            value = null;
            if (key != "InternalResourceVersion")
            {
                return false;
            }

            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                binaryReader.BaseStream.Position += CachedHashBytesLength;
                byte stringLength = binaryReader.ReadByte();
                binaryReader.BaseStream.Position += stringLength;
                value = binaryReader.ReadInt32();
            }

            return true;
        }

        /// <summary>
        /// 尝试从可更新模式版本资源列表（版本 1 或版本 2）获取指定键的值回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <param name="key">指定键。</param>
        /// <param name="value">指定键的值。</param>
        /// <returns></returns>
        private bool UpdatableVersionListTryGetValueCallback_V1_V2(Stream stream, string key, out object value)
        {
            value = null;
            if (key != "InternalResourceVersion")
            {
                return false;
            }

            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                binaryReader.BaseStream.Position += CachedHashBytesLength;
                byte stringLength = binaryReader.ReadByte();
                binaryReader.BaseStream.Position += stringLength;
                value = binaryReader.Read7BitEncodedInt32();
            }

            return true;
        }
    }
}
