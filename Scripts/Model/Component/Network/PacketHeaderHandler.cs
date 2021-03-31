//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Network;

namespace ETModel
{
    public class PacketHeaderHandler : IPacketHeader, IReference
    {
        public PacketHeaderHandler()
        {
            PacketLength = 0;
        }
        public int PacketLength
        {
            get;
            set;
        }

        public void Clear()
        {
            PacketLength = 0;
        }
    }
}

