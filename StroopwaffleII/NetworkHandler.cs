using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.Threading;

namespace StroopwaffleII {
    public class NetworkHandler {

        private NetPeerConfiguration Config { get; set; }
        public NetClient LidgrenClient { get; set; }

        public NetworkHandler() {
            Config = new NetPeerConfiguration("sw2");
            Config.AutoFlushSendQueue = false;

            LidgrenClient = new NetClient(Config);
            LidgrenClient.Start();
        }

        public NetConnection Connect(string ip) {
            NetOutgoingMessage hail = LidgrenClient.CreateMessage("Hail, citizen!");
            return LidgrenClient.Connect(ip, 7777, hail);
        }

        public void Disconnect() {
            LidgrenClient.Disconnect("Bye.");
        }

        public void ReadPackets() {
            NetIncomingMessage netIncomingMessage;

            while ((netIncomingMessage = LidgrenClient.ServerConnection.Peer.ReadMessage()) != null) {
                if (netIncomingMessage.MessageType == NetIncomingMessageType.StatusChanged) {
                    NetConnectionStatus status = (NetConnectionStatus)netIncomingMessage.ReadByte();

                    if (status == NetConnectionStatus.Connected) {
                        Console.WriteLine("Connected");
                        LidgrenClient.FlushSendQueue();
                    }
                }
                LidgrenClient.Recycle(netIncomingMessage);
            }
        }
    }
}
