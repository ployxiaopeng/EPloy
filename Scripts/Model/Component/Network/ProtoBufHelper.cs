using GameFramework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETModel
{
    public static class ProtoBufHelper
    {
        #region ProtoBuf 工具
        public static string ScoketMd5key = "{0}{1}clientHttpKey{2}";
        public static byte[] intToBytes(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((value >> 24) & 0xFF);
            src[1] = (byte)((value >> 16) & 0xFF);
            src[2] = (byte)((value >> 8) & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
        }
        public static int bytesToInt(byte[] src, int offset = 0)
        {
            int value;
            value = (int)((src[offset + 3] & 0xFF)
                | ((src[offset + 2] & 0xFF) << 8)
                | ((src[offset + 1] & 0xFF) << 16)
                | ((src[offset] & 0xFF) << 24));
            return value;
        }

        public static byte[] StringToByte(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
        public static byte[] Serialize<T>(T instance)
        {
            byte[] bytes;
            using (var msg = new MemoryStream())
            {
                Serializer.Serialize(msg, instance);
                bytes = new byte[msg.Position];
                var FullBytes = msg.GetBuffer();
                Array.Copy(FullBytes, bytes, bytes.Length);
            }
            return bytes;
        }
        public static T Deseralize<T>(object obj)
        {
            byte[] bytes = (byte[])obj;
            using (var msg = new MemoryStream(bytes, 0, bytes.Length))
            {
                return Serializer.Deserialize<T>(msg);
            }
        }
        #endregion
    }
}