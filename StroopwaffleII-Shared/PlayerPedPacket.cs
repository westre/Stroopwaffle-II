using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class PlayerPedPacket : IPacket {
        public int ParentId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        public void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.PlayerPed);
            message.Write(ParentId);
            message.Write(PosX);
            message.Write(PosY);
            message.Write(PosZ);
            message.Write(Pitch);
            message.Write(Roll);
            message.Write(Yaw);
        }

        // PacketType gets voided due to it already begin read
        public void Unpack(NetIncomingMessage message) {
            ParentId = message.ReadInt32();
            PosX = message.ReadFloat();
            PosY = message.ReadFloat();
            PosZ = message.ReadFloat();
            Pitch = message.ReadFloat();
            Roll = message.ReadFloat();
            Yaw = message.ReadFloat();
        }
    }
}
