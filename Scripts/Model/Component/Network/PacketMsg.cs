using GameFramework.Network;

namespace ETModel
{
    public class PacketMsg : Packet
    {
        private byte[] _bytes;
        private int packetId;
        public override int Id
        {
            get { return packetId; }
        }

        public override void Clear()
        {
            _bytes = null;
        }

        public void PutByte(byte[] bytes)
        {
            _bytes = bytes;
        }

        /// <summary>
        /// 获取包的二进制数据
        /// </summary>
        public byte[] toByteAarray
        {
            get { return _bytes; }
        }
        /// <summary>
        /// 获取包的二进制数据长度
        /// </summary>
        public int ByteAarrayLengh
        {
            get { return _bytes.Length; }
        }

    }
}
