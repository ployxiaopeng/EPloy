using System.Collections.Generic;
using System;
using System.IO;
using EPloy.Net;
using EPloy.Checker;

/// <summary>
/// 网络模块TCP
/// </summary>
public partial class NetModule : IGameModule
{
    private readonly Dictionary<int, Type> MessageTypes = new Dictionary<int, Type>();

    /// <summary>
    /// 获取所有MessageType
    /// </summary>
    private void GetMessageTypes()
    {
        Type[] Types = HotfixHelper.GetHotfixTypes;
        foreach (Type type in Types)
        {
            object[] objects = type.GetCustomAttributes(typeof(MessageAttribute), false);
            if (objects.Length != 0)
            {
                MessageAttribute attribute = (MessageAttribute)objects[0];
                if (MessageTypes.ContainsKey(attribute.RpcId))
                {
                    Log.Error(UtilText.Format("Repetition RpcId: {0} type: {1}  ", attribute.RpcId, type));
                    continue;
                }
                MessageTypes.Add(attribute.RpcId, type);
            }
        }
    }
    /// <summary>
    /// 协议类生成
    /// </summary>
    public T CreateMessage<T>() where T : IMessage
    {
        MessageAttribute attribute = (MessageAttribute)typeof(T).GetCustomAttributes(typeof(MessageAttribute), false)[0];
        if (attribute == null)
        {
            Log.Error(UtilText.Format("No Exist Message type: {1}  ", typeof(T)));
            return default(T);
        }
        IRequest message = (IRequest)MessagePool.Instance.Fetch(typeof(T));
        message.RpcId = attribute.RpcId;
        return (T)message;
    }


    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(IRequest request)
    {
        Packet packet = new Packet();
        packet.header = request.RpcId;
        byte[] bHeader = ProtobufHelper.intToBytes(request.RpcId); //协议头
        byte[] bBata = ProtobufHelper.ToBytes(request);//协议内容
        byte[] bCount = ProtobufHelper.intToBytes(bHeader.Length + bBata.Length);//协议总长度
        MemoryStream memory = new MemoryStream(bCount.Length + bHeader.Length + bBata.Length);
        memory.Write(bCount, 0, bCount.Length);
        memory.Write(bHeader, 0, bHeader.Length);
        memory.Write(bBata, 0, bBata.Length);
        packet.data = memory.GetBuffer();
        NetChannel.Send(packet);
        MessagePool.Instance.Recycle(request);
        TestNet();
    }

    /// <summary>
    /// 注册心跳消息
    /// </summary>
    private Packet HeartBeat()
    {

        return new Packet();
    }

    private void TestNet()
    {
        G2C_Register register = (G2C_Register)MessagePool.Instance.Fetch(typeof(G2C_Register));
        register.RpcId = HotfixOpcode.G2C_Register;
        register.Error = 0;
        register.Message = "本地测试注册成功";
        byte[] bHeader = ProtobufHelper.intToBytes(register.RpcId); //协议头
        byte[] bBata = ProtobufHelper.ToBytes(register);//协议内容
        byte[] bCount = ProtobufHelper.intToBytes(bHeader.Length + bBata.Length);//协议总长度
        MemoryStream memory = new MemoryStream(bCount.Length + bHeader.Length + bBata.Length);
        memory.Write(bCount, 0, bCount.Length);
        memory.Write(bHeader, 0, bHeader.Length);
        memory.Write(bBata, 0, bBata.Length);
        ((TcpNetChannel)NetChannel).TestNetReceiver(memory.GetBuffer());
    }
}