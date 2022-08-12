using System;

namespace EPloy.Game.Net
{
    public interface IMHandler
    {
        void Handle(NetChannel Channel, object message);
        Type GetMessageType();
    }
}