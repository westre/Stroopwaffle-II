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

        public bool Aiming { get; set; }
        public float AimPosX { get; set; }
        public float AimPosY { get; set; }
        public float AimPosZ { get; set; }

        public bool Shooting { get; set; }
        public bool Walking { get; set; }
        public bool Running { get; set; }
        public bool Sprinting { get; set; }
        public bool Jumping { get; set; }
        public bool Reloading { get; set; }

        public float Heading { get; set; }

        public float OffsetFrontX { get; set; }
        public float OffsetFrontY { get; set; }
        public float OffsetFrontZ { get; set; }
        public float Speed { get; set; }

        public void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.PlayerPed);
            message.Write(ParentId);
            message.Write(PosX);
            message.Write(PosY);
            message.Write(PosZ);
            message.Write(Pitch);
            message.Write(Roll);
            message.Write(Yaw);

            message.Write(Aiming);
            message.Write(AimPosX);
            message.Write(AimPosY);
            message.Write(AimPosZ);

            message.Write(Shooting);
            message.Write(Walking);
            message.Write(Running);
            message.Write(Sprinting);
            message.Write(Jumping);
            message.Write(Reloading);

            message.Write(Heading);

            message.Write(OffsetFrontX);
            message.Write(OffsetFrontY);
            message.Write(OffsetFrontZ);
            message.Write(Speed);
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

            Aiming = message.ReadBoolean();
            AimPosX = message.ReadFloat();
            AimPosY = message.ReadFloat();
            AimPosZ = message.ReadFloat();

            Shooting = message.ReadBoolean();
            Walking = message.ReadBoolean();
            Running = message.ReadBoolean();
            Sprinting = message.ReadBoolean();
            Jumping = message.ReadBoolean();
            Reloading = message.ReadBoolean();

            Heading = message.ReadFloat();

            OffsetFrontX = message.ReadFloat();
            OffsetFrontY = message.ReadFloat();
            OffsetFrontZ = message.ReadFloat();
            Speed = message.ReadFloat();
        }
    }
}
