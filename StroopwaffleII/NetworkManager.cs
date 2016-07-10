using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class NetworkManager {

        public List<NetworkClient> NetworkClients { get; set; }

        public List<NetworkPed> NetworkPeds { get; set; }
        public List<NetworkVehicle> NetworkVehicles { get; set; }
    
    }
}
