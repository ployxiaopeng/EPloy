using System;

namespace EPloy.Net
{
    public interface IMHandler
    {
        void Handle(NetChannel Channel, object message);
        Type GetMessageType();
    }
}