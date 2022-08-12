using System;
using System.IO;

namespace EPloy.Game.Net
{
    /// <summary>
    /// 处理粘包情况
    /// 协议构成 长度（4字节）+协议头（（4字节））+协议内容 
    /// </summary>
    public class NetReceiver : IDisposable
    {
        //最大每次接受16m
        private const int DefaultBufferLength = 1024 * 16;
        //协议头定死4个字节 长度也是
        private const int HeadLength = 4;
        public MemoryStream Stream { get; set; }
        public int PacketLength { get; private set; }
        public bool Disposed { get; private set; }

        public NetReceiver()
        {
            Stream = new MemoryStream(DefaultBufferLength);
            PacketLength = 0;
            Disposed = false;
        }

        /// <summary>
        /// 是否有消息 有 >0
        /// </summary>
        /// <returns></returns>
        public int HasMsg()
        {
            byte[] data = new byte[HeadLength];
            Stream.Read(data, 0, data.Length);
            return ProtobufHelper.bytesToInt(data);
        }

        public int GetHeader()
        {
            byte[] data = new byte[HeadLength];
            Stream.Read(data, 0, data.Length);
            return ProtobufHelper.bytesToInt(data);
        }

        public byte[] GetData(int count)
        {
            byte[] data = new byte[count- HeadLength];
            Stream.Read(data, 0, data.Length);
            return data;
        }

        public void PreparePacketLength(int packetLength)
        {
            if (packetLength <= 0)
            {
                Log.Fatal("Packet header is invalid.");
            }
            Reset(packetLength);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }
            }

            Disposed = true;
        }

        private void Reset(int targetLength)
        {
            Stream.Position = 0L;
            Stream.SetLength(targetLength);
        }
    }
}
