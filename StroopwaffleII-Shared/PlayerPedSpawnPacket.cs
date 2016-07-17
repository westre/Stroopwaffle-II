using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class PlayerPedSpawnPacket : Packet {
        public int ParentId { get; set; }
        public float[] Position { get; set; }

        public override void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.PlayerPedSpawn);
            message.Write(ParentId);
            message.Write(Serialize(Position));
        }

        // PacketType gets voided due to it already begin read
        public override void Unpack(NetIncomingMessage message) {
            ParentId = message.ReadInt32();
            Position = Deserialize<float[]>(message.ReadString());
        }
    }
}
