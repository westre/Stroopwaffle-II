﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class RemoveClientPacket : Packet {
        public int ID { get; set; }
        public long LidgrenId { get; set; }

        public override void Pack(NetOutgoingMessage message) {
            message.Write((byte)PacketType.RemoveClient);
            message.Write(ID);
            message.Write(LidgrenId);
        }

        // PacketType gets voided due to it already begin read
        public override void Unpack(NetIncomingMessage message) {
            ID = message.ReadInt32();
            LidgrenId = message.ReadInt64();
        }
    }
}
