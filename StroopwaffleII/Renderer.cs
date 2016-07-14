using Rage;
using StroopwaffleII_Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class Renderer {
        private NetworkHandler NetworkHandler { get; set; }

        Dictionary<NetworkClient, Ped> PhysicalPeds { get; set; }

        public Renderer(NetworkHandler networkHandler) {
            NetworkHandler = networkHandler;

            PhysicalPeds = new Dictionary<NetworkClient, Ped>();
        }

        public void Render() {
            foreach(NetworkClient networkClient in NetworkHandler.NetworkManager.NetworkClients) {
                // World ped does not exist
                if (!PhysicalPeds.ContainsKey(networkClient)) {
                    Console.WriteLine("Not found");
                    PhysicalPeds[networkClient] = new Ped("s_m_m_paramedic_01", new Vector3(networkClient.NetworkPed.PosX, networkClient.NetworkPed.PosY, networkClient.NetworkPed.PosZ), 0f);
                    PhysicalPeds[networkClient].IsPersistent = true;
                    PhysicalPeds[networkClient].BlockPermanentEvents = true;
                    PhysicalPeds[networkClient].CanBeDamaged = false;
                }

                Ped ped = PhysicalPeds[networkClient];
                ped.Position = new Vector3(networkClient.NetworkPed.PosX + 5, networkClient.NetworkPed.PosY, networkClient.NetworkPed.PosZ);
                ped.Rotation = new Rotator(networkClient.NetworkPed.Pitch, networkClient.NetworkPed.Roll, networkClient.NetworkPed.Yaw);
            }
        }

        public void Clear() {
            PhysicalPeds.Clear();
        }
    }
}
