using System.Collections.Generic;
using System;
using EPloy.Net;

public delegate void NetListener<in T>(T obj);
/// <summary>
/// 网络模块TCP
/// </summary>
public partial class NetModule : IGameModule
{
    private readonly Dictionary<int, NetListener<IResponse>> NetMessages = new Dictionary<int, NetListener<IResponse>>();

    public void AddListener(int rpcId, NetListener<IResponse> listener)
    {
        if (NetMessages.ContainsKey(rpcId))
        {
            Log.Error(string.Format("Net listener is exist code: {0} 协议监听唯一 ", rpcId));
            return;
        }
        NetMessages.Add(rpcId, listener);
    }

    public void RemoveListener(int rpcId)
    {
        if (NetMessages.ContainsKey(rpcId))
        {
            NetMessages.Remove(rpcId);
        }
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="packet"></param>
    private void MsgHandler(Packet packet)
    {
        Type type = MessageTypes[packet.header];
        if (type == null)
        {
            Log.Error(string.Format("packet header is not exist header: {0}", packet.header));
            return;
        }
        object message = MessagePool.Instance.Fetch(type);
        IResponse response = (IResponse)ProtobufHelper.FromBytes(message, packet.data, 0, packet.data.Length);
        //主动监听的
        if (NetMessages.ContainsKey(packet.header))
        {
            NetMessages[packet.header]?.Invoke(response);
            MessagePool.Instance.Recycle(response);
            return;
        }
        MessagePool.Instance.Recycle(response);
        Log.Error(string.Format("packet Listener is miss header: {0}", packet.header));
    }
}
