using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII_Shared {
    public class NetworkVehicle : NetworkEntity {
        public string Name { get; set; }
        public float[] Position { get; set; }
        public float Heading { get; set; }
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }

        public NetworkVehicle(int id) : base(id) {
            
        }
    }
}
