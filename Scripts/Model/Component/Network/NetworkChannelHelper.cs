//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;
using GameFramework;
using GameFramework.Network;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;

namespace ETModel
{
    public class NetworkChannelHelper : INetworkChannelHelper
    {
        private INetworkChannel NetworkChannel;

        /// <summary>
        /// 获取消息包头长度。
        /// </summary>
        public int PacketHeaderLength
        {
            get
            {
                return sizeof(int);
            }
        }

        /// <summary>
        /// 初始化网络频道辅助器。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        public void Initialize(INetworkChannel networkChannel)
        {
            NetworkChannel = networkChannel;
            NetworkChannel.SetDefaultHandler(MsgHandler);
            NetworkChannel.HeartBeatInterval = 60f;
        }

        /// <summary>
        /// 关闭并清理网络频道辅助器。
        /// </summary>
        public void Shutdown()
        {
            NetworkChannel = null;
        }

        /// <summary>
        /// 发送心跳消息包。
        /// </summary>
        /// <returns>是否发送心跳消息包成功。</returns>
        public bool SendHeartBeat()
        {
            return true;
        }

        /// <summary>
        /// 序列化消息包。
        /// </summary>
        /// <typeparam name="T">消息包类型。</typeparam>
        /// <param name="packet">要序列化的消息包。</param>
        /// <param name="destination">要序列化的目标流。</param>
        /// <returns>是否序列化成功。</returns>
        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            PacketMsg socketMsg = packet as PacketMsg;
            if (socketMsg == null)
            {
                Log.Error("SocketMsg is invalid.");
                return false;
            }
            destination.Write(socketMsg.toByteAarray, 0, socketMsg.ByteAarrayLengh);
            return true;
        }

        /// <summary>
        /// 反序列消息包头。
        /// </summary>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns></returns>
        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            PacketHeaderHandler Handler = ReferencePool.Acquire<PacketHeaderHandler>();
            customErrorData = null;
            try
            {
                byte[] buflength = new byte[4]; //长度
                source.Read(buflength, 0, (int)source.Length);
                Handler.PacketLength = ProtoBufHelper.bytesToInt(buflength);
            }
            catch (Exception e)
            {
                Log.Error(e);
                customErrorData = string.Format("反序列消息包 Err:{0}" + e);
            }
            return Handler;
        }

        /// <summary>
        /// 反序列化消息包。
        /// </summary>
        /// <param name="packetHeader">消息包头。</param>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns>反序列化后的消息包。</returns>
        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            PacketMsg SocMsg = ReferencePool.Acquire<PacketMsg>();
            customErrorData = null;
            try
            {
                PacketHeaderHandler packetHandler = packetHeader as PacketHeaderHandler;
                byte[] buflength = new byte[packetHandler.PacketLength]; //消息体
                source.Read(buflength, 0, packetHandler.PacketLength);
                SocMsg.PutByte(buflength);
            }
            catch (Exception e)
            {
                Log.Error(e);
                customErrorData = string.Format("反序列消息包 Err:{0}" + e);
            }
            return SocMsg;
        }

        #region 消息回调 传给热更层
        /// <summary>
        /// 消息返回处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        public void MsgHandler(object sender, Packet packet)
        {
            if (!Init.ILRuntime.ILRuntimeMode)
            {
                Log.Error("在编辑器模式下暂无包消息处理");
                return;
            }
            PacketMsg packetMsg = packet as PacketMsg;
            using (var ctx = Init.ILRuntime.AppDomain.BeginInvoke(MethodMsgHandler))
            {
                ctx.PushObject(HotfixInstance);
                ctx.PushObject(sender);
                ctx.PushObject(packet);
                ctx.Invoke();
            }
        }
        /// <summary>
        /// 热更曾消息处理者
        /// </summary>
        private string HotfixMsgHandler;
        private object HotfixInstance;
        private IMethod MethodMsgHandler;
        public NetworkChannelHelper(string hotfixMsgHandler)
        {
            if (!Init.ILRuntime.ILRuntimeMode)
            {
                Log.Error("在编辑器模式下暂无包消息处理");
                return;
            }
            HotfixMsgHandler = Utility.Text.Format("{0}.{1}", "ETHotfix", hotfixMsgHandler);
            //获取热更新层的实例
            IType type = Init.ILRuntime.AppDomain.LoadedTypes[HotfixMsgHandler];
            HotfixInstance = ((ILType)type).Instantiate();
            MethodMsgHandler = type.GetMethod("MsgHandler", 2);
        }
        #endregion
    }
}