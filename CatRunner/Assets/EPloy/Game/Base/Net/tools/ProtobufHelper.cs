using System;
using System.ComponentModel;
using System.IO;
using Google.Protobuf;

namespace EPloy.Net
{
	public static class ProtobufHelper
	{
		public static byte[] ToBytes(object message)
		{
			return ((Google.Protobuf.IMessage) message).ToByteArray();
		}
		
		public static void ToStream(object message, MemoryStream stream)
		{
			((Google.Protobuf.IMessage) message).WriteTo(stream);
		}
		
		public static object FromBytes(Type type, byte[] bytes, int index, int count)
		{
			object message = Activator.CreateInstance(type);
			((Google.Protobuf.IMessage)message).MergeFrom(bytes, index, count);
			ISupportInitialize iSupportInitialize = message as ISupportInitialize;
			if (iSupportInitialize == null)
			{
				return message;
			}
			iSupportInitialize.EndInit();
			return message;
		}
		
		public static object FromBytes(object instance, byte[] bytes, int index, int count)
		{
			object message = instance;
			((Google.Protobuf.IMessage)message).MergeFrom(bytes, index, count);
			ISupportInitialize iSupportInitialize = message as ISupportInitialize;
			if (iSupportInitialize == null)
			{
				return message;
			}
			iSupportInitialize.EndInit();
			return message;
		}
		
		public static object FromStream(Type type, MemoryStream stream)
		{
			object message = Activator.CreateInstance(type);
			((Google.Protobuf.IMessage)message).MergeFrom(stream.GetBuffer(), (int)stream.Position, (int)stream.Length);
			ISupportInitialize iSupportInitialize = message as ISupportInitialize;
			if (iSupportInitialize == null)
			{
				return message;
			}
			iSupportInitialize.EndInit();
			return message;
		}
		
		public static object FromStream(object message, MemoryStream stream)
		{
			// 这个message可以从池中获取，减少gc
			((Google.Protobuf.IMessage)message).MergeFrom(stream.GetBuffer(), (int)stream.Position, (int)stream.Length);
			ISupportInitialize iSupportInitialize = message as ISupportInitialize;
			if (iSupportInitialize == null)
			{
				return message;
			}
			iSupportInitialize.EndInit();
			return message;
		}

        public static byte[] intToBytes(int value)
        {
            byte[] src = new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            return src;
        }

        public static int bytesToInt(byte[] src, int offset = 0)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }

    }
}