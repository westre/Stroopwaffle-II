using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class HelloClientPacket : IPacket {
        public string Payload { get; set; }

        public void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.HelloClient);
            message.Write(Payload);
        }

        // PacketType gets voided due to it already begin read
        public void Unpack(NetIncomingMessage message) {
            Payload = message.ReadString();
        }
    }
}
