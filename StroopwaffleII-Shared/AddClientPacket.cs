using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class AddClientPacket : IPacket {
        public int ID { get; set; }
        public string Name { get; set; }
        public long LidgrenId { get; set; }

        public void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.AddClient);
            message.Write(ID);
            message.Write(Name);
            message.Write(LidgrenId);
        }

        // PacketType gets voided due to it already begin read
        public void Unpack(NetIncomingMessage message) {
            ID = message.ReadInt32();
            Name = message.ReadString();
            LidgrenId = message.ReadInt64();
        }
    }
}
