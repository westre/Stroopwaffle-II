using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using StroopwaffleII_Shared;
using Lidgren.Network;

// http://www.koonsolo.com/news/dewitters-gameloop/
// http://gamedev.stackexchange.com/questions/44931/calculating-delta-time-what-is-wrong
// http://forum.unity3d.com/threads/multiplayer-game-tick-time.164531/

namespace StroopwaffleII_Server {
    class SendUpdatesThread {
        private Server Server { get; set; }
        private Thread Thread { get; set; }
        private const int HERTZ = 60;
        private const int SKIP_TICKS = 1000 / HERTZ;

        public SendUpdatesThread(Server server) {
            Server = server;

            Thread updateThread = new Thread(UpdateThread);
            updateThread.Start();
        }

        private void UpdateThread() {
            int nextGameTick = Environment.TickCount;
            int sleepTime = 0;

            while(true) {
                Tick();

                nextGameTick += SKIP_TICKS;
                sleepTime = nextGameTick - Environment.TickCount;

                if(sleepTime >= 0) {
                    Thread.Sleep(sleepTime);
                }
                /*else {
                    Console.WriteLine("We are running behind by " + sleepTime + "ms.");
                }*/
            }
        }

        private void Tick() {
            foreach (NetworkClient netClient in Server.NetworkManager.NetworkClients) {
                PlayerPedPacket pedPacket = new PlayerPedPacket {
                    ParentId = netClient.ID,
                    PosX = netClient.NetworkPed.PosX,
                    PosY = netClient.NetworkPed.PosY,
                    PosZ = netClient.NetworkPed.PosZ,
                    Pitch = netClient.NetworkPed.Pitch,
                    Roll = netClient.NetworkPed.Roll,
                    Yaw = netClient.NetworkPed.Yaw,      
                    Aiming = netClient.NetworkPed.Aiming,
                    AimPosX = netClient.NetworkPed.AimPosX,
                    AimPosY = netClient.NetworkPed.AimPosY,
                    AimPosZ = netClient.NetworkPed.AimPosZ,
                    Shooting = netClient.NetworkPed.Shooting,
                    Walking = netClient.NetworkPed.Walking,
                    Running = netClient.NetworkPed.Running,
                    Sprinting = netClient.NetworkPed.Sprinting,
                    Jumping = netClient.NetworkPed.Jumping,
                    Reloading = netClient.NetworkPed.Reloading,
                    Heading = netClient.NetworkPed.Heading,
                    OffsetFrontX = netClient.NetworkPed.OffsetFrontX,
                    OffsetFrontY = netClient.NetworkPed.OffsetFrontY,
                    OffsetFrontZ = netClient.NetworkPed.OffsetFrontZ,
                    Speed = netClient.NetworkPed.Speed,
                };

                // TODO anti-cheat measures here
                // just send the raw packet to the clients
                NetOutgoingMessage message = Server.NetServer.CreateMessage();
                pedPacket.Pack(message);
                // send to all

                if (Server.NetServer.Connections.Count > 0)
                    Server.NetServer.SendMessage(message, Server.NetServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }

            foreach (NetworkVehicle netVehicle in Server.NetworkManager.NetworkVehicles) {
                VehiclePacket vehiclePacket = new VehiclePacket {
                    ID = netVehicle.ID,
                    Name = netVehicle.Name,
                    Position = netVehicle.Position,
                    Heading = netVehicle.Heading,
                    PrimaryColor = netVehicle.PrimaryColor,
                    SecondaryColor = netVehicle.SecondaryColor
                };

                NetOutgoingMessage message = Server.NetServer.CreateMessage();
                vehiclePacket.Pack(message);

                if (Server.NetServer.Connections.Count > 0)
                    Server.NetServer.SendMessage(message, Server.NetServer.Connections, NetDeliveryMethod.Unreliable, 0);
            }
        }
    }
}
