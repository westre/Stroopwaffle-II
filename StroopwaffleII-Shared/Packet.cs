using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII_Shared {
    public interface IPacket {
        void Pack(NetOutgoingMessage message);
        void Unpack(NetIncomingMessage message);
    }
}
