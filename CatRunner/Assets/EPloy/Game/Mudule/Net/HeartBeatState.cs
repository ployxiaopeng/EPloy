
namespace EPloy.Game.Net
{
    public class HeartBeatState
    {
        public float HeartBeatElapseSeconds { get; private set; }
        public int MissHeartBeatCount { get; private set; }

        public HeartBeatState()
        {
            HeartBeatElapseSeconds = 0f;
            MissHeartBeatCount = 0;
        }

        public void SetHeartBeatState(float heartBeatElapseSeconds, int missHeartBeatCount)
        {
            HeartBeatElapseSeconds = heartBeatElapseSeconds;
            MissHeartBeatCount = missHeartBeatCount;
        }

        public void Reset(bool resetHeartBeatElapseSeconds)
        {
            if (resetHeartBeatElapseSeconds)
            {
                HeartBeatElapseSeconds = 0f;
            }

            MissHeartBeatCount = 0;
        }
    }

    /// <summary>
    /// 网络地址类型。
    /// </summary>
    public enum AddressFamily : byte
    {
        /// <summary>
        /// 未知。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// IP 版本 4。
        /// </summary>
        IPv4,

        /// <summary>
        /// IP 版本 6。
        /// </summary>
        IPv6
    }

    public enum NetErrorCode : byte
    {
        /// <summary>
        /// 未知错误。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 地址族错误。
        /// </summary>
        AddressFamilyError,

        /// <summary>
        /// Socket 错误。
        /// </summary>
        SocketError,

        /// <summary>
        /// 连接错误。
        /// </summary>
        ConnectError,

        /// <summary>
        /// 发送错误。
        /// </summary>
        SendError,

        /// <summary>
        /// 接收错误。
        /// </summary>
        ReceiveError,

        /// <summary>
        /// 序列化错误。
        /// </summary>
        SerializeError,

        /// <summary>
        /// 反序列化消息包头错误。
        /// </summary>
        DeserializePacketHeaderError,

        /// <summary>
        /// 反序列化消息包错误。
        /// </summary>
        DeserializePacketError
    }
}
