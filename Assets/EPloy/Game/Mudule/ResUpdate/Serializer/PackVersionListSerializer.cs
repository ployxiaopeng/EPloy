using System;
using System.IO;
using System.Text;

namespace EPloy.Res
{
    /// <summary>
    /// 资源包版本资源列表序列化器。
    /// </summary>
    public sealed class PackVersionListSerializer : EPloySerializer<PackVersionList>
    {
        private readonly byte[] Header = new byte[] { (byte)'E', (byte)'P', (byte)'K' };

        /// <summary>
        /// 获取资源包版本资源列表头标识。
        /// </summary>
        /// <returns>资源包版本资源列表头标识。</returns>
        protected override byte[] GetHeader()
        {
            return Header;
        }
        /// <summary>
        /// 初始化资源包版本资源列表序列化器的新实例。
        /// </summary>
        public PackVersionListSerializer()
        {
#if UNITY_EDITOR
            base.RegisterSerializeCallback(0, PackVersionListSerializeCallback_V0);
#endif
            base.RegisterDeserializeCallback(0, PackVersionListDeserializeCallback_V0);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 序列化资源包版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的资源包版本资源列表（版本 0）。</param>
        /// <returns>是否序列化资源包版本资源列表（版本 0）成功。</returns>
        public static bool PackVersionListSerializeCallback_V0(Stream stream, PackVersionList versionList)
        {
            if (!versionList.IsValid)
            {
                return false;
            }

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                binaryWriter.Write(versionList.Offset);
                binaryWriter.Write(versionList.Length);
                binaryWriter.Write(versionList.HashCode);
                PackVersionList.Resource[] resources = versionList.Resources;
                binaryWriter.Write7BitEncodedInt32(resources.Length);
                foreach (PackVersionList.Resource resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(resource.Variant, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(resource.Extension != DefaultExtension ? resource.Extension : null, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write7BitEncodedInt64(resource.Offset);
                    binaryWriter.Write7BitEncodedInt32(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                    binaryWriter.Write7BitEncodedInt32(resource.ZipLength);
                    binaryWriter.Write(resource.ZipHashCode);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

#endif

        /// <summary>
        /// 反序列化资源包版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的资源包版本资源列表（版本 0）。</returns>
        private PackVersionList PackVersionListDeserializeCallback_V0(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                byte[] encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                int dataOffset = binaryReader.ReadInt32();
                long dataLength = binaryReader.ReadInt64();
                int dataHashCode = binaryReader.ReadInt32();
                int resourceCount = binaryReader.Read7BitEncodedInt32();
                PackVersionList.Resource[] resources = resourceCount > 0 ? new PackVersionList.Resource[resourceCount] : null;
                for (int i = 0; i < resourceCount; i++)
                {
                    string name = binaryReader.ReadEncryptedString(encryptBytes);
                    string variant = binaryReader.ReadEncryptedString(encryptBytes);
                    string extension = binaryReader.ReadEncryptedString(encryptBytes) ?? DefaultExtension;
                    byte loadType = binaryReader.ReadByte();
                    long offset = binaryReader.Read7BitEncodedInt64();
                    int length = binaryReader.Read7BitEncodedInt32();
                    int hashCode = binaryReader.ReadInt32();
                    int compressedLength = binaryReader.Read7BitEncodedInt32();
                    int compressedHashCode = binaryReader.ReadInt32();
                    resources[i] = new PackVersionList.Resource(name, variant, extension, loadType, offset, length, hashCode, compressedLength, compressedHashCode);
                }

                return new PackVersionList(dataOffset, dataLength, dataHashCode, resources);
            }
        }
    }
}
