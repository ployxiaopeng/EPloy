using System.Net;
using System.Net.Sockets;
using EPloy.Game;
using EPloy.Game.Net;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 网络模块TCP
    /// </summary>
    public partial class NetMudule : IHotfixModule
    {
        private NetChannel NetChannel;
        public override void Awake()
        {
            NetChannel = new TcpNetChannel("Main");
            NetChannel.NetConnected = NetConnected;
            NetChannel.NetClosed = NetClosed;
            NetChannel.NetMissHeartBeat = NetMissHeartBeat;
            NetChannel.NetError = NetError;

            NetChannel.RegisterHandler(MsgHandler);
            NetChannel.RegisterHeartBeat(HeartBeat);

            GetMessageTypes();
            HotFixMudule.RegisterUpdate(NetChannel.Update);
        }

        public override void OnDestroy()
        {
            HotFixMudule.RemoveUpdate(NetChannel.Update);
            NetChannel.Dispose();
            NetChannel = null;
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            NetChannel.Connect(ipAddress, port);
        }

        private void NetConnected()
        {

        }
        private void NetClosed()
        {

        }
        private void NetMissHeartBeat(int number)
        {

        }
        private void NetError(NetErrorCode netErrorCode, SocketError socketError, string msg)
        {
            Log.Error(msg);
        }
    }
}
