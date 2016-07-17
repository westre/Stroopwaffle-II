using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class HelloClientPacket : Packet {
        public string Payload { get; set; }

        public override void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.HelloClient);
            message.Write(Payload);
        }

        // PacketType gets voided due to it already begin read
        public override void Unpack(NetIncomingMessage message) {
            Payload = message.ReadString();
        }
    }
}
