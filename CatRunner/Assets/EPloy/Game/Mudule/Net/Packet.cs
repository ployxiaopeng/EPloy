using System;

namespace EPloy.Game.Net
{
    /// <summary>
    /// 网络消息包
    /// </summary>
    public  class Packet :  IDisposable
    {
        /// <summary>
        ///消息头
        /// </summary>
        public int header { get; set; }
        /// <summary>
        ///消息数据
        /// </summary>
        public byte[] data { get; set; }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            header = 0;
            data = null;
            GC.SuppressFinalize(this);
        }
    }
}
