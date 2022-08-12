using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EPloy.Game.Net
{
    /// <summary>
    /// 网络频道基类。
    /// </summary>
    public abstract class NetChannel : IDisposable
    {
        private const float DefaultHeartBeatInterval = 30f;

        /// <summary>
        /// 获取网络频道名称。
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 消息回调处理
        /// </summary>
        protected Action<Packet> msgHandler;
        /// <summary>
        /// 获取网络地址类型。
        /// </summary>
        protected AddressFamily AddressFamily;
        /// <summary>
        /// 获取或设置心跳间隔时长，以秒为单位。
        /// </summary>
        protected float HeartBeatInterval;
        /// <summary>
        /// 获取网络频道所使用的 Socket。
        /// </summary>
        protected Socket Socket;
        /// <summary>
        /// 心跳消息包
        /// </summary>
        protected Func<Packet> heartBeatHandler;
        protected readonly Queue<Packet> SendPacketPool;
        protected readonly Queue<Packet> ReceivePacketPool;
        protected readonly HeartBeatState HeartBeatState;
        protected bool Active;
        protected bool Disposed;

        public Action NetConnected { get; set; }
        public Action NetClosed { get; set; }
        public Action<int> NetMissHeartBeat { get; set; }
        public Action<NetErrorCode, SocketError, string> NetError { get; set; }

        /// <summary>
        /// 初始化网络频道基类的新实例。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <param name="NetChannelHelper">网络频道辅助器。</param>
        public NetChannel(string name)
        {
            Name = name ?? string.Empty;
            SendPacketPool = new Queue<Packet>();
            ReceivePacketPool = new Queue<Packet>();
            AddressFamily = AddressFamily.Unknown;
            HeartBeatInterval = DefaultHeartBeatInterval;
            Socket = null;
            HeartBeatState = new HeartBeatState();
            Active = false;
            Disposed = false;

            NetConnected = null;
            NetClosed = null;
            NetMissHeartBeat = null;
            NetError = null;
            msgHandler = null;
            heartBeatHandler = null;
        }
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        public bool Connected
        {
            get
            {
                if (Socket != null)
                {
                    return Socket.Connected;
                }

                return false;
            }
        }

        /// <summary>
        /// 获取要发送的消息包数量。
        /// </summary>
        public int SendPacketCount
        {
            get
            {
                return SendPacketPool.Count;
            }
        }

        /// <summary>
        /// 获取已接收未处理的消息包数量。
        /// </summary>
        public int ReceivePacketCount
        {
            get
            {
                return ReceivePacketPool.Count;
            }
        }

        /// <summary>
        /// 获取丢失心跳的次数。
        /// </summary>
        public int MissHeartBeatCount
        {
            get
            {
                return HeartBeatState.MissHeartBeatCount;
            }
        }

        /// <summary>
        /// 获取心跳等待时长，以秒为单位。
        /// </summary>
        public float HeartBeatElapseSeconds
        {
            get
            {
                return HeartBeatState.HeartBeatElapseSeconds;
            }
        }

        /// <summary>
        /// 网络频道轮询。
        /// </summary>
        public virtual void Update()
        {
            ProcessReceive(); //本地测试
            if (Socket == null || !Active)
            {
                return;
            }

            ProcessSend();
            // ProcessReceive();
            if (Socket == null || !Active)
            {
                return;
            }

            if (HeartBeatInterval > 0f)
            {
                bool sendHeartBeat = false;
                int missHeartBeatCount = 0;
                lock (HeartBeatState)
                {
                    if (Socket == null || !Active)
                    {
                        return;
                    }

                    if (HeartBeatState.HeartBeatElapseSeconds >= HeartBeatInterval)
                    {
                        sendHeartBeat = true;
                        missHeartBeatCount = HeartBeatState.MissHeartBeatCount;

                    }
                }

                if (sendHeartBeat && SendHeartBeat())
                {
                    if (missHeartBeatCount > 0 && NetMissHeartBeat != null)
                    {
                        NetMissHeartBeat(missHeartBeatCount);
                    }
                }
            }
        }

        /// <summary>
        /// 关闭网络频道。
        /// </summary>
        public virtual void Shutdown()
        {
            Close();
            ReceivePacketPool.Clear();
        }

        /// <summary>
        /// 注册网络消息包处理函数。
        /// </summary>
        /// <param name="handler">要注册的网络消息包处理函数。</param>
        public void RegisterHandler(Action<Packet> handler)
        {
            if (handler == null)
            {
                Log.Fatal("Packet handler is invalid.");
            }
            msgHandler = handler;
        }

        /// <summary>
        /// 注册网络消息心跳包
        /// </summary>
        /// <param name="handler">要注册的网络消息包处理函数。</param>
        public void RegisterHeartBeat(Func<Packet> handler)
        {
            if (handler == null)
            {
                Log.Fatal("SendHeartBeat handler is invalid.");
            }
            heartBeatHandler = handler;
        }

        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        /// <param name="ipAddress">远程主机的 IP 地址。</param>
        /// <param name="port">远程主机的端口号。</param>
        public virtual void Connect(IPAddress ipAddress, int port)
        {
            if (Socket != null)
            {
                Close();
                Socket = null;
            }

            switch (ipAddress.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    AddressFamily = AddressFamily.IPv4;
                    break;

                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    AddressFamily = AddressFamily.IPv6;
                    break;

                default:
                    string errorMessage = UtilText.Format("Not supported address family '{0}'.", ipAddress.AddressFamily);
                    if (NetError != null)
                    {
                        NetError(NetErrorCode.AddressFamilyError, SocketError.Success, errorMessage);
                        return;
                    }
                    Log.Fatal(errorMessage);
                    break;
            }
        }

        /// <summary>
        /// 关闭连接并释放所有相关资源。
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (Socket == null)
                {
                    return;
                }

                Active = false;

                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                finally
                {
                    Socket.Close();
                    Socket = null;
                    NetClosed?.Invoke();
                }

                lock (SendPacketPool)
                {
                    SendPacketPool.Clear();
                }

                ReceivePacketPool.Clear();

                lock (HeartBeatState)
                {
                    HeartBeatState.Reset(true);
                }
            }
        }

        /// <summary>
        /// 向远程主机发送消息包。
        /// </summary>
        /// <param name="packet">要发送的消息包。</param>
        public void Send(Packet packet)
        {
            if (Socket == null)
            { 
                string errorMessage = "You must connect first.";
                if (NetError != null)
                {
                    NetError(NetErrorCode.SendError, SocketError.Success, errorMessage);
                    packet.Dispose();
                    return;
                }

                Log.Fatal(errorMessage);
            }

            if (!Active)
            {
                string errorMessage = "Socket is not active.";
                if (NetError != null)
                {
                    NetError(NetErrorCode.SendError, SocketError.Success, errorMessage);
                    packet.Dispose();
                    return;
                }

                Log.Fatal(errorMessage);
            }

            if (packet == null || packet.data.Length <= 0)
            {
                string errorMessage = "Packet is invalid.";
                if (NetError != null)
                {
                    NetError(NetErrorCode.SendError, SocketError.Success, errorMessage);
                    packet.Dispose();
                    return;
                }

                Log.Fatal(errorMessage);
            }

            lock (SendPacketPool)
            {
                SendPacketPool.Enqueue(packet);
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        private void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                Close();
            }

            Disposed = true;
        }

        /// <summary>
        /// 向远程主机发送心跳消息包。
        /// </summary>
        /// <param name="packet">要发送的消息包。</param>
        private bool SendHeartBeat()
        {
            lock (SendPacketPool)
            {
                SendPacketPool.Enqueue(heartBeatHandler());
            }
            return true;
        }

        protected abstract void ProcessSend();
        protected abstract void ProcessReceive();

        protected virtual bool DeserializePacket()
        {
            lock (HeartBeatState)
            {
                HeartBeatState.Reset(false);
            }
            return true;
        }
    }
}