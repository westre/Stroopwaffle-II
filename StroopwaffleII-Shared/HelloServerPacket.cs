using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class HelloServerPacket : IPacket {
        public string Name { get; set; }

        public void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.HelloServer);
            message.Write(Name);
        }

        // PacketType gets voided due to it already begin read
        public void Unpack(NetIncomingMessage message) {
            Name = message.ReadString();
        }
    }
}
