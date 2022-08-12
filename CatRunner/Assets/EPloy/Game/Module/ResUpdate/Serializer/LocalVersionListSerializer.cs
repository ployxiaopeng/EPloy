using System;
using System.IO;
using System.Text;

namespace EPloy.Res
{
    /// <summary>
    /// 本地只读区版本资源列表序列化器。
    /// </summary>
    public sealed class LocalVersionListSerializer : EPloySerializer<LocalVersionList>
    {
        private readonly byte[] Header = new byte[] { (byte)'E', (byte)'P', (byte)'L' };

        /// <summary>
        /// 获取本地只读区版本资源列表头标识。
        /// </summary>
        /// <returns>本地只读区版本资源列表头标识。</returns>
        protected override byte[] GetHeader()
        {
            return Header;
        }

        /// <summary>
        /// 初始化本地只读区版本资源列表序列化器的新实例。
        /// </summary>
        public LocalVersionListSerializer()
        {
#if UNITY_EDITOR
            base.RegisterSerializeCallback(0, LocalVersionListSerializeCallback_V0);
            base.RegisterSerializeCallback(1, LocalVersionListSerializeCallback_V1);
#endif
            base.RegisterDeserializeCallback(0, LocalVersionListDeserializeCallback_V0);
            base.RegisterDeserializeCallback(1, LocalVersionListDeserializeCallback_V1);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 序列化本地版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的本地版本资源列表（版本 0）。</param>
        /// <returns>是否序列化本地版本资源列表（版本 0）成功。</returns>
        private bool LocalVersionListSerializeCallback_V0(Stream stream, LocalVersionList versionList)
        {
            if (!versionList.IsValid)
            {
                return false;
            }

            UtilRandom.GetRandomBytes(s_CachedHashBytes);
            using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                LocalVersionList.Resource[] resources = versionList.Resources;
                binaryWriter.Write(resources.Length);
                foreach (LocalVersionList.Resource resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        /// 序列化本地版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的本地版本资源列表（版本 1）。</param>
        /// <returns>是否序列化本地版本资源列表（版本 1）成功。</returns>
        private bool LocalVersionListSerializeCallback_V1(Stream stream, LocalVersionList versionList)
        {
            if (!versionList.IsValid)
            {
                return false;
            }

            UtilRandom.GetRandomBytes(s_CachedHashBytes);
            using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                LocalVersionList.Resource[] resources = versionList.Resources;
                binaryWriter.Write7BitEncodedInt32(resources.Length);
                foreach (LocalVersionList.Resource resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write7BitEncodedInt32(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

#endif

        /// <summary>
        /// 反序列化本地版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的本地版本资源列表（版本 0）。</returns>
        private LocalVersionList LocalVersionListDeserializeCallback_V0(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                int resourceCount = binaryReader.ReadInt32();
                LocalVersionList.Resource[] resources = resourceCount > 0 ? new LocalVersionList.Resource[resourceCount] : null;
                for (int i = 0; i < resourceCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    string variant = binaryReader.ReadEncryptedString(encryptBytes);
                    byte loadType = binaryReader.ReadByte();
                    int length = binaryReader.ReadInt32();
                    int hashCode = binaryReader.ReadInt32();
                    resources[i] = new LocalVersionList.Resource(name, loadType, length, hashCode);
                }

                return new LocalVersionList(resources,null);
            }
        }

        /// <summary>
        /// 反序列化本地版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的本地版本资源列表（版本 1）。</returns>
        private LocalVersionList LocalVersionListDeserializeCallback_V1(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                int resourceCount = binaryReader.Read7BitEncodedInt32();
                LocalVersionList.Resource[] resources = resourceCount > 0 ? new LocalVersionList.Resource[resourceCount] : null;
                for (int i = 0; i < resourceCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    byte loadType = binaryReader.ReadByte();
                    int length = binaryReader.Read7BitEncodedInt32();
                    int hashCode = binaryReader.ReadInt32();
                    resources[i] = new LocalVersionList.Resource(name, loadType, length, hashCode);
                }

                int fileSystemCount = binaryReader.Read7BitEncodedInt32();
                LocalVersionList.FileSystem[] fileSystems = fileSystemCount > 0 ? new LocalVersionList.FileSystem[fileSystemCount] : null;
                for (int i = 0; i < fileSystemCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    int resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                    int[] resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (int j = 0; j < resourceIndexCount; j++)
                    {
                        resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();
                    }

                    fileSystems[i] = new LocalVersionList.FileSystem(name, resourceIndexes);
                }

                return new LocalVersionList(resources, fileSystems);
            }
        }
    }
}
