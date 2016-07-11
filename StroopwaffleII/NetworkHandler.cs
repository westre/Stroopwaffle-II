using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.Threading;
using StroopwaffleII_Shared;

namespace StroopwaffleII {
    public class NetworkHandler {

        private NetPeerConfiguration Config { get; set; }
        public NetClient LidgrenClient { get; set; }
        private NetworkManager NetworkManager { get; set; }

        public NetworkHandler() {
            Config = new NetPeerConfiguration("sw2");
            //Config.AutoFlushSendQueue = false;

            NetworkManager = new NetworkManager();

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
                if (netIncomingMessage.MessageType == NetIncomingMessageType.Data) {
                    PacketType packetType = (PacketType)netIncomingMessage.ReadByte();
                    IPacket packet;

                    switch (packetType) {
                        case PacketType.HelloClient: packet = new HelloClientPacket(); break;
                        case PacketType.AddClient: packet = new AddClientPacket(); break;
                        default: packet = null; break;
                    }

                    packet.Unpack(netIncomingMessage);

                    // received our first packet from the server
                    if (packet is HelloClientPacket) {
                        Game.DisplayNotification("Hello from Server: " + ((HelloClientPacket)packet).Payload);

                        // send our information to the server for to be processed
                        HelloServerPacket helloServer = new HelloServerPacket();
                        helloServer.Name = "westre";

                        NetOutgoingMessage message = LidgrenClient.CreateMessage();
                        helloServer.Pack(message);
                        LidgrenClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
                    }
                    else if (packet is AddClientPacket) {
                        Game.DisplayNotification("AddClientPacket");

                        AddClientPacket addClientPacket = (AddClientPacket)packet;

                        NetworkClient addClient = new NetworkClient(addClientPacket.ID);
                        addClient.Name = addClientPacket.Name;

                        NetworkManager.NetworkClients.Add(addClient);

                        Game.DisplayNotification("AddClientPacket added: " + NetworkManager.NetworkClients.Count);
                    }
                }

                LidgrenClient.Recycle(netIncomingMessage);
            }
        }
    }
}
