using System.Net;
using System.Net.Sockets;
using EPloy.Net;

/// <summary>
/// 网络模块TCP
/// </summary>
public partial class NetModule : IGameModule
{
    private NetChannel NetChannel;
    public void Awake()
    {
        NetChannel = new TcpNetChannel("Main");
        NetChannel.NetConnected = NetConnected;
        NetChannel.NetClosed = NetClosed;
        NetChannel.NetMissHeartBeat = NetMissHeartBeat;
        NetChannel.NetError = NetError;

        NetChannel.RegisterHandler(MsgHandler);
        NetChannel.RegisterHeartBeat(HeartBeat);

        GetMessageTypes();
    }

    public void Update()
    {
        NetChannel.Update();
    }

    public void OnDestroy()
    {
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
