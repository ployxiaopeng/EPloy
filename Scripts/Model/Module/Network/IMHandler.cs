using GameFramework.Network;
using System;

namespace ETModel
{
    public interface IMHandler
    {
        void Handle(INetworkChannel Channel, object message);
        Type GetMessageType();
    }
}