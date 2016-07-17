using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII_Shared {
    public class NetworkManager {
        public List<NetworkClient> NetworkClients { get; set; }
        public List<NetworkVehicle> NetworkVehicles { get; set; }

        private bool[] EntityIDs { get; set; }

        public NetworkManager() {
            NetworkClients = new List<NetworkClient>();
            NetworkVehicles = new List<NetworkVehicle>();

            EntityIDs = new bool[1000];
        }

        // searches for a free unique ID to be used as a network ID
        // if found, it allocates it, if not, it will return a -1
        public int AllocateEntityID() {
            for (int index = 0; index < EntityIDs.Length; index++) {
                if (!EntityIDs[index]) {
                    EntityIDs[index] = true;
                    return index;
                }
            }
            return -1;
        }

        public NetworkClient FindClientByLidgrenId(long lidgrenId) {
            var thisClient = (from client in NetworkClients
                             where client.LidgrenId == lidgrenId
                             select client).FirstOrDefault();

            return thisClient; 
        }

        public NetworkClient FindClientById(int id) {
            var thisClient = (from client in NetworkClients
                              where client.ID == id
                              select client).FirstOrDefault();

            return thisClient;
        }

        public NetworkClient CreateClient(HelloServerPacket packet, long lidgrenId) {
            int freeId = AllocateEntityID();

            NetworkClient newClient = new NetworkClient(freeId);
            newClient.Name = packet.Name;
            newClient.LidgrenId = lidgrenId;
            newClient.SafeForNet = true;

            NetworkClients.Add(newClient);
            Console.WriteLine("New networkClient added, size: " + NetworkClients.Count);

            return newClient;
        }

        public void DestroyClient(long lidgrenId) {
            NetworkClient client = FindClientByLidgrenId(lidgrenId);
            if (client != null) {
                EntityIDs[client.ID] = false;
                NetworkClients.Remove(client);
                Console.WriteLine("Removed networkCLient, size: " + NetworkClients.Count);
            }
            else {
                Console.WriteLine("Could not find client Server::DestroyClient");
            }
        }

        public NetworkClient GetLocalPlayer() {
            var thisClient = (from client in NetworkClients
                              where client.LocalPlayer == true
                              select client).FirstOrDefault();

            return thisClient;
        }

        public void Clear() {
            NetworkClients.Clear();
            NetworkVehicles.Clear();
        }
    }
}
