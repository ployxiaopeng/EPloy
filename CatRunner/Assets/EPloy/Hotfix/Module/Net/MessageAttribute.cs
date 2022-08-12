using System;

namespace EPloy.Net
{
    /// <summary>
    /// 协议标识
    /// </summary>
	public class MessageAttribute : Attribute
    {
        public int RpcId { get; }
        public Type AttributeType { get; }
        public MessageAttribute(int rpcId)
        {
            this.AttributeType = this.GetType();
            RpcId = rpcId;
        }
    }
}