﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StroopwaffleII_Shared;

namespace StroopwaffleII {
    class NetworkManager {
        public List<NetworkClient> NetworkClients { get; set; }
        public List<NetworkPed> NetworkPeds { get; set; }
        public List<NetworkVehicle> NetworkVehicles { get; set; }

        private bool[] EntityIDs { get; set; }
    
        public NetworkManager() {
            EntityIDs = new bool[1000];
        }

        // searches for a free unique ID to be used as a network ID
        // if found, it allocates it, if not, it will return a -1
        public int AllocateEntityID() {
            for(int index = 0; index < EntityIDs.Length; index++) {
                if(!EntityIDs[index]) {
                    EntityIDs[index] = true;
                    return index;
                }
            }
            return -1;
        }
    }
}