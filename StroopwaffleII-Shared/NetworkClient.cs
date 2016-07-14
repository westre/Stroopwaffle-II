using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace StroopwaffleII_Shared {
    public class NetworkClient : NetworkEntity {

        public bool LocalPlayer { get; set; }
        public string Name { get; set; }
        public long LidgrenId { get; set; }

        public NetworkClient(int id) : base(id) {

        }
    }
}
