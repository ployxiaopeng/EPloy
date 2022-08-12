using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EPloy.Net
{
    /// <summary>
    /// TCP 网络频道。
    /// </summary>
    public class TcpNetChannel : NetChannel
    {
        private readonly AsyncCallback connectCallback;
        private readonly AsyncCallback sendCallback;
        private readonly AsyncCallback receiveCallback;
        private readonly NetReceiver netReceiver;

        /// <summary>
        /// 初始化网络频道的新实例。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        public TcpNetChannel(string name) : base(name)
        {
            connectCallback = ConnectCallback;
            sendCallback = SendCallback;
            receiveCallback = ReceiveCallback;
            netReceiver = new NetReceiver();
        }

        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        /// <param name="ipAddress">远程主机的 IP 地址。</param>
        /// <param name="port">远程主机的端口号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public override void Connect(IPAddress ipAddress, int port)
        {
            base.Connect(ipAddress, port);
            Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (Socket == null)
            {
                string errorMessage = "Initialize network channel failure.";
                if (NetError != null)
                {
                    NetError(NetErrorCode.SocketError, SocketError.Success, errorMessage);
                    return;
                }
                Log.Fatal(errorMessage);
            }
     
            try
            {
                Socket.BeginConnect(ipAddress, port, ConnectCallback, Socket);
            }
            catch (Exception exception)
            {
                if (NetError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetError(NetErrorCode.ConnectError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return;
                }

                throw;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
            }
            catch (Exception exception)
            {
                Active = false;
                if (NetError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetError(NetErrorCode.ConnectError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return;
                }

                throw;
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
            NetConnected?.Invoke();
            Active = true;
            StartReceive();
        }

        protected override void ProcessSend()
        {
            {
                try
                {
                    while (SendPacketPool.Count > 0)
                    {
                        Packet packet = null;
                        lock (SendPacketPool)
                        {
                            packet = SendPacketPool.Dequeue();
                        }
                        Socket.BeginSend(packet.data, 0, packet.data.Length, SocketFlags.None, SendCallback, packet);
                    }
                }
                catch (Exception exception)
                {
                    Active = false;
                    if (NetError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetError(NetErrorCode.SendError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    }
                    Log.Fatal(exception.ToString());
                    throw;
                }
            }
        }

        protected override void ProcessReceive()
        {
            try
            {
                while (ReceivePacketPool.Count > 0)
                {
                    Packet packet = null;
                    lock (ReceivePacketPool)
                    {
                        packet = ReceivePacketPool.Dequeue();
                    }
                    msgHandler(packet);
                    packet.Dispose();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString());
                Active = false;
                if (NetError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetError(NetErrorCode.DeserializePacketError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return;
                }

                throw;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Packet packet = (Packet)ar.AsyncState;
            packet.Dispose();
        }

        private void StartReceive()
        {
            try
            {
                Socket.BeginReceive(netReceiver.Stream.GetBuffer(), (int)netReceiver.Stream.Position, (int)(netReceiver.Stream.Length - netReceiver.Stream.Position), SocketFlags.None, ReceiveCallback, Socket);
            }
            catch (Exception exception)
            {
                Active = false;
                if (NetError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetError(NetErrorCode.ReceiveError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return;
                }

                throw;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (!socket.Connected)
            {
                return;
            }

            int bytesReceived = 0;
            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            catch (Exception exception)
            {
                Active = false;
                if (NetError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetError(NetErrorCode.ReceiveError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return;
                }

                throw;
            }

            if (bytesReceived <= 0)
            {
                Close();
                return;
            }

            netReceiver.Stream.Position += bytesReceived;
            if (netReceiver.Stream.Position < netReceiver.Stream.Length)
            {
                StartReceive();
                return;
            }

            netReceiver.Stream.Position = 0L;
            bool processSuccess = DeserializePacket();
            if (processSuccess)
            {
                StartReceive();
                return;
            }
        }

        protected override bool DeserializePacket()
        {
            base.DeserializePacket();
            try
            {
                int count = netReceiver.HasMsg();
                while (count > 0)
                {
                    Packet packet = new Packet();
                    packet.header = netReceiver.GetHeader();
                    packet.data = netReceiver.GetData(count);
                    if (packet.header > 0)
                    {
                        ReceivePacketPool.Enqueue(packet);
                    }
                    count = netReceiver.HasMsg();
                }
            }
            catch (Exception exception)
            {
                Active = false;
                if (NetError != null)
                {
                    SocketException socketException = exception as SocketException;
                    NetError(NetErrorCode.DeserializePacketHeaderError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                    return false;
                }
                throw;
            }

            return true;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            netReceiver.Dispose();
        }


        /// <summary>
        /// 测试网络消息包处理
        /// </summary>
        public void TestNetReceiver(byte[] data)
        {
            netReceiver.Stream = new MemoryStream(data);
            DeserializePacket();
        }
    }
}
