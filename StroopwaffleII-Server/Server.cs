using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using System.Threading;
using StroopwaffleII_Shared;
using System.Drawing;

namespace StroopwaffleII_Server {
    class Server {

        public NetServer NetServer { get; set; }
        public NetworkManager NetworkManager { get; set; }
        private SendUpdatesThread SendUpdatesThread { get; set; }

        public Server() {
            NetPeerConfiguration config = new NetPeerConfiguration("sw2");
            config.Port = 7777;
            config.MaximumConnections = 100;

            NetworkManager = new NetworkManager();

            NetServer = new NetServer(config);
            NetServer.Start();

            Initialize();

            Thread serverThread = new Thread(ServerThread);
            serverThread.Start();

            SendUpdatesThread = new SendUpdatesThread(this);
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
                            }
                            else if (status == NetConnectionStatus.Disconnected) {
                                NetworkClient networkClient = NetworkManager.FindClientByLidgrenId(incomingMessage.SenderConnection.RemoteUniqueIdentifier);

                                RemoveClientPacket removeClient = new RemoveClientPacket();
                                removeClient.ID = networkClient.ID;
                                removeClient.LidgrenId = networkClient.LidgrenId;

                                NetOutgoingMessage message = NetServer.CreateMessage();
                                removeClient.Pack(message);
                                // send to all
                                if(NetServer.Connections.Count > 0)
                                    NetServer.SendMessage(message, NetServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);

                                NetworkManager.DestroyClient(incomingMessage.SenderConnection.RemoteUniqueIdentifier);
                                Console.WriteLine("|_ disconnected");
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            PacketType packetType = (PacketType)incomingMessage.ReadByte();
                            Packet packet;

                            switch (packetType) {
                                case PacketType.HelloServer: packet = new HelloServerPacket(); break;
                                case PacketType.PlayerPed: packet = new PlayerPedPacket(); break;
                                default: packet = null; break;
                            }

                            packet.Unpack(incomingMessage);

                            // HelloClientPacket -> HelloServerPacket -> AddClient
                            if(packet is HelloServerPacket) {
                                Console.WriteLine("Hello from " + ((HelloServerPacket)packet).Name);

                                // create the client to be held on the server
                                NetworkClient netClient = NetworkManager.CreateClient((HelloServerPacket)packet, incomingMessage.SenderConnection.RemoteUniqueIdentifier);

                                // then send this newly created client to everyone else to be held
                                AddClientPacket addClient = new AddClientPacket();
                                addClient.ID = netClient.ID;
                                addClient.Name = netClient.Name;
                                addClient.LidgrenId = netClient.LidgrenId;
                                addClient.SafeForNet = true;

                                NetOutgoingMessage message = NetServer.CreateMessage();
                                addClient.Pack(message);

                                // send to all
                                NetServer.SendMessage(message, NetServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);

                                // also send the client their initial position data
                                PlayerPedSpawnPacket playerPedSpawnPacket = new PlayerPedSpawnPacket();
                                playerPedSpawnPacket.ParentId = netClient.ID;
                                playerPedSpawnPacket.Position = new float[] { 652.637f, 6522.557f, 28.21065f };

                                // also make sure to set it in the server's arraylist for AC purposes
                                netClient.NetworkPed.PosX = 652.637f;
                                netClient.NetworkPed.PosY = 6522.557f;
                                netClient.NetworkPed.PosZ = 28.21065f;

                                // send to the client
                                message = NetServer.CreateMessage();
                                playerPedSpawnPacket.Pack(message);
                                NetServer.SendMessage(message, incomingMessage.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            }
                            else if(packet is PlayerPedPacket) {
                                Console.WriteLine("Recv PlayerPedPacket");

                                PlayerPedPacket playerPedPacket = (PlayerPedPacket)packet;

                                NetworkClient networkClient = NetworkManager.FindClientById(playerPedPacket.ParentId);
                                if (networkClient != null) {
                                    networkClient.NetworkPed.PosX = playerPedPacket.PosX;
                                    networkClient.NetworkPed.PosY = playerPedPacket.PosY;
                                    networkClient.NetworkPed.PosZ = playerPedPacket.PosZ;
                                    networkClient.NetworkPed.Pitch = playerPedPacket.Pitch;
                                    networkClient.NetworkPed.Roll = playerPedPacket.Roll;
                                    networkClient.NetworkPed.Yaw = playerPedPacket.Yaw;

                                    networkClient.NetworkPed.Aiming = playerPedPacket.Aiming;
                                    networkClient.NetworkPed.AimPosX = playerPedPacket.AimPosX;
                                    networkClient.NetworkPed.AimPosY = playerPedPacket.AimPosY;
                                    networkClient.NetworkPed.AimPosZ = playerPedPacket.AimPosZ;

                                    networkClient.NetworkPed.Shooting = playerPedPacket.Shooting;
                                    networkClient.NetworkPed.Walking = playerPedPacket.Walking;
                                    networkClient.NetworkPed.Running = playerPedPacket.Running;
                                    networkClient.NetworkPed.Sprinting = playerPedPacket.Sprinting;
                                    networkClient.NetworkPed.Jumping = playerPedPacket.Jumping;
                                    networkClient.NetworkPed.Reloading = playerPedPacket.Reloading;

                                    networkClient.NetworkPed.Heading = playerPedPacket.Heading;

                                    networkClient.NetworkPed.OffsetFrontX = playerPedPacket.OffsetFrontX;
                                    networkClient.NetworkPed.OffsetFrontY = playerPedPacket.OffsetFrontY;
                                    networkClient.NetworkPed.OffsetFrontZ = playerPedPacket.OffsetFrontZ;
                                    networkClient.NetworkPed.Speed = playerPedPacket.Speed;
                                }
                                else {
                                    Console.WriteLine("Shit.. networkClient == null @ PlayerPedPacket, Server::ServerThread()");
                                }
                            }

                            break;
                    }
                    NetServer.Recycle(incomingMessage);
                }
                Thread.Sleep(1);
            }
        }

        private void Initialize() {
            NetworkManager.NetworkVehicles.Add(new NetworkVehicle(NetworkManager.AllocateEntityID()) {
                Name = "police",
                Position = new float[] { 655.637f, 6522.557f, 28.21065f },
                Heading = 180f,
                PrimaryColor = Color.Orange,
                SecondaryColor = Color.Orange
            });

            NetworkManager.NetworkVehicles.Add(new NetworkVehicle(NetworkManager.AllocateEntityID()) {
                Name = "police",
                Position = new float[] { 660.637f, 6522.557f, 28.21065f },
                Heading = 180f,
                PrimaryColor = Color.Orange,
                SecondaryColor = Color.Orange
            });

            NetworkManager.NetworkVehicles.Add(new NetworkVehicle(NetworkManager.AllocateEntityID()) {
                Name = "police2",
                Position = new float[] { 665.637f, 6522.557f, 28.21065f },
                Heading = 180f,
                PrimaryColor = Color.Orange,
                SecondaryColor = Color.Orange
            });
        }

        public AddVehiclePacket ConstructAddVehiclePacket() {
            return new AddVehiclePacket {
                ID = NetworkManager.AllocateEntityID(),
                Name = "police",
                Position = new float[] { 655.637f, 6522.557f, 28.21065f },
                Heading = 180f,
                PrimaryColor = Color.Orange,
                SecondaryColor = Color.Orange
            };
        }

        static void Main(string[] args) {
            new Server();
        }
    }
}
