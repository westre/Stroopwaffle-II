using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using System.Threading;
using StroopwaffleII_Shared;

namespace StroopwaffleII_Server {
    class Server {

        private NetServer NetServer { get; set; }
        private NetworkManager NetworkManager { get; set; }

        public Server() {
            NetPeerConfiguration config = new NetPeerConfiguration("sw2");
            config.Port = 7777;
            config.MaximumConnections = 100;

            NetworkManager = new NetworkManager();

            NetServer = new NetServer(config);
            NetServer.Start();

            Thread serverThread = new Thread(ServerThread);
            serverThread.Start();
        }

        public void ServerThread() {
            Console.WriteLine("Starting server...");

            while(true) {
                NetIncomingMessage incomingMessage;
                while ((incomingMessage = NetServer.ReadMessage()) != null) {
                    switch (incomingMessage.MessageType) {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = incomingMessage.ReadString();
                            Console.WriteLine("[WARNING] " + text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)incomingMessage.ReadByte();

                            string reason = incomingMessage.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(incomingMessage.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected) {
                                Console.WriteLine("|_ connected");

                                HelloClientPacket helloClient = new HelloClientPacket();
                                helloClient.Payload = NetUtility.ToHexString(incomingMessage.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason;

                                NetOutgoingMessage message = NetServer.CreateMessage();
                                helloClient.Pack(message);
                                NetServer.SendMessage(message, incomingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                                Console.WriteLine("Sent message");
                            }
                            else if (status == NetConnectionStatus.Disconnected) {
                                DestroyClient(incomingMessage.SenderConnection);
                                Console.WriteLine("|_ disconnected");
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            PacketType packetType = (PacketType)incomingMessage.ReadByte();
                            IPacket packet;

                            switch (packetType) {
                                case PacketType.HelloServer: packet = new HelloServerPacket(); break;
                                default: packet = null; break;
                            }

                            packet.Unpack(incomingMessage);

                            // HelloClientPacket -> HelloServerPacket -> AddClient
                            if(packet is HelloServerPacket) {
                                Console.WriteLine("Hello from " + ((HelloServerPacket)packet).Name);

                                // create the client to be held on the server
                                NetworkClient netClient = CreateClient((HelloServerPacket)packet, incomingMessage.SenderConnection);

                                // then send this newly created client to everyone else to be held
                                AddClientPacket addClient = new AddClientPacket();
                                addClient.ID = netClient.ID;
                                addClient.Name = netClient.Name;

                                NetOutgoingMessage message = NetServer.CreateMessage();
                                addClient.Pack(message);
                                // send to all
                                NetServer.SendMessage(message, NetServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            }

                            break;
                    }
                    NetServer.Recycle(incomingMessage);
                }
                Thread.Sleep(1);
            }
        }

        private NetworkClient CreateClient(HelloServerPacket packet, NetConnection netConnection) {
            int freeId = NetworkManager.AllocateEntityID();

            NetworkClient newClient = new NetworkClient(freeId);
            newClient.Name = packet.Name;
            newClient.NetConnection = netConnection;

            NetworkManager.NetworkClients.Add(newClient);
            Console.WriteLine("New networkClient added, size: " + NetworkManager.NetworkClients.Count);

            return newClient;
        }

        private void DestroyClient(NetConnection netConnection) {
            NetworkClient client = NetworkManager.FindClient(netConnection);
            if(client != null) {
                NetworkManager.NetworkClients.Remove(client);
                Console.WriteLine("Removed networkCLient, size: " + NetworkManager.NetworkClients.Count);
            }
            else {
                Console.WriteLine("Could not find client Server::DestroyClient");
            }
        }

        static void Main(string[] args) {
            new Server();
        }
    }
}
