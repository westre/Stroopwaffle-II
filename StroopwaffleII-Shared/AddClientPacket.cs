using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class AddClientPacket : Packet {
        public int ID { get; set; }
        public string Name { get; set; }
        public long LidgrenId { get; set; }
        public bool SafeForNet { get; set; }

        public override void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.AddClient);
            message.Write(ID);
            message.Write(Name);
            message.Write(LidgrenId);
            message.Write(SafeForNet);
        }

        // PacketType gets voided due to it already begin read
        public override void Unpack(NetIncomingMessage message) {
            ID = message.ReadInt32();
            Name = message.ReadString();
            LidgrenId = message.ReadInt64();
            SafeForNet = message.ReadBoolean();
        }
    }
}
