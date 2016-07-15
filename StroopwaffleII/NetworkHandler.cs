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
        public NetworkManager NetworkManager { get; set; }

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
                if(netIncomingMessage.MessageType == NetIncomingMessageType.StatusChanged) {
                    NetConnectionStatus status = (NetConnectionStatus)netIncomingMessage.ReadByte();

                    if(status == NetConnectionStatus.Disconnected) {
                        Game.DisplayNotification("Disconnected.. clearing lists");
                        NetworkManager.Clear();
                    }
                }

                if (netIncomingMessage.MessageType == NetIncomingMessageType.Data) {
                    PacketType packetType = (PacketType)netIncomingMessage.ReadByte();
                    IPacket packet;

                    switch (packetType) {
                        case PacketType.HelloClient: packet = new HelloClientPacket(); break;
                        case PacketType.AddClient: packet = new AddClientPacket(); break;
                        case PacketType.RemoveClient: packet = new RemoveClientPacket(); break;
                        case PacketType.PlayerPed: packet = new PlayerPedPacket(); break;
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
                        addClient.LidgrenId = addClientPacket.LidgrenId;
                        addClient.SafeForNet = addClientPacket.SafeForNet;

                        NetworkManager.NetworkClients.Add(addClient);

                        Game.DisplayNotification("AddClientPacket added: " + NetworkManager.NetworkClients.Count);

                        // Are we the local player?
                        if(addClient.LidgrenId == LidgrenClient.UniqueIdentifier) {
                            addClient.LocalPlayer = true;
                            Game.DisplayNotification("This is the localPlayer");
                        }
                    }
                    else if (packet is RemoveClientPacket) {
                        Game.DisplayNotification("RemoveClientPacket");

                        RemoveClientPacket removeClientPacket = (RemoveClientPacket)packet;

                        NetworkManager.DestroyClient(removeClientPacket.LidgrenId);
                    }
                    else if(packet is PlayerPedPacket) {
                        PlayerPedPacket playerPedPacket = (PlayerPedPacket)packet;

                        NetworkClient networkClient = NetworkManager.FindClientById(playerPedPacket.ParentId);
                        if(networkClient != null) {
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
                            Console.WriteLine("Shit.. networkClient == null @ PlayerPedPacket, NetworkHandler::ReadPackets()");
                        }
                    }
                }

                LidgrenClient.Recycle(netIncomingMessage);
            }
        }
    }
}
