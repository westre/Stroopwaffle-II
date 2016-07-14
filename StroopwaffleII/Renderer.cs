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
                    PhysicalPeds[networkClient] = new Ped("s_m_m_paramedic_01", new Vector3(networkClient.NetworkPed.PosX, networkClient.NetworkPed.PosY, networkClient.NetworkPed.PosZ), 0f);
                    PhysicalPeds[networkClient].IsPersistent = true;
                    PhysicalPeds[networkClient].BlockPermanentEvents = true;
                    PhysicalPeds[networkClient].CanBeDamaged = false;

                    WeaponAsset asset = new WeaponAsset("WEAPON_PISTOL");
                    PhysicalPeds[networkClient].Inventory.GiveNewWeapon(asset, 1000, true);
                }

                Ped ped = PhysicalPeds[networkClient];
                Vector3 pedPosition = new Vector3(networkClient.NetworkPed.PosX, networkClient.NetworkPed.PosY + 2, networkClient.NetworkPed.PosZ);
                Rotator pedRotation = new Rotator(networkClient.NetworkPed.Pitch, networkClient.NetworkPed.Roll, networkClient.NetworkPed.Yaw);

                /*if(!networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Sprinting) {
                    ped.Heading = networkClient.NetworkPed.Heading;
                }
                else if(networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Running) {
                    ped.Tasks.GoStraightToPosition(pedPosition, 1f, networkClient.NetworkPed.Heading, 0f, 10);
                }
                else if(!networkClient.NetworkPed.Walking && (networkClient.NetworkPed.Running || networkClient.NetworkPed.Sprinting)) {
                    ped.Tasks.GoStraightToPosition(pedPosition, 4f, networkClient.NetworkPed.Heading, 0f, 10);
                }*/
         
                ped.Position = pedPosition;
                ped.Rotation = pedRotation;
            }
        }

        public void Clear() {
            PhysicalPeds.Clear();
        }
    }
}
