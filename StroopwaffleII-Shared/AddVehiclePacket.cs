using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Drawing;

namespace StroopwaffleII_Shared {
    public class AddVehiclePacket : Packet {
        public int ID { get; set; }
        public string Name { get; set; }
        public float[] Position { get; set; }
        public float Heading { get; set; }
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }

        public override void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.AddVehicle);
            message.Write(ID);
            message.Write(Name);
            message.Write(Serialize(Position));
            message.Write(Heading);
            message.Write(PrimaryColor.ToArgb());
            message.Write(SecondaryColor.ToArgb());
        }

        // PacketType gets voided due to it already begin read
        public override void Unpack(NetIncomingMessage message) {
            ID = message.ReadInt32();
            Name = message.ReadString();
            Position = Deserialize<float[]>(message.ReadString());
            Heading = message.ReadFloat();
            PrimaryColor = Color.FromArgb(message.ReadInt32());
            SecondaryColor = Color.FromArgb(message.ReadInt32());         
        }
    }
}
