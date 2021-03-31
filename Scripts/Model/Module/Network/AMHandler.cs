using GameFramework.Network;
using System;

namespace ETModel
{
	public abstract class AMHandler<Message> : IMHandler where Message: class
	{
		protected abstract void Run(INetworkChannel Channel, Message message);

		public void Handle(INetworkChannel Channel, object msg)
		{
			Message message = msg as Message;
			if (message == null)
			{
				Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(Message).Name}");
				return;
			}
			if (!Channel.Connected)
			{
				Log.Error($"Channel disconnect {msg}");
				return;
			}
			this.Run(Channel, message);
		}

		public Type GetMessageType()
		{
			return typeof(Message);
		}
	}
}