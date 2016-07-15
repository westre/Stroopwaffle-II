using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII_Shared {
    public class NetworkPed {
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        public bool Aiming { get; set; }
        public float AimPosX { get; set; }
        public float AimPosY { get; set; }
        public float AimPosZ { get; set; }

        public bool Shooting { get; set; }
        public bool Walking { get; set; }
        public bool Running { get; set; }
        public bool Sprinting { get; set; }
        public bool Jumping { get; set; }
        public bool Reloading { get; set; }

        public float Heading { get; set; }

        public float OffsetFrontX { get; set; }
        public float OffsetFrontY { get; set; }
        public float OffsetFrontZ { get; set; }
        public float Speed { get; set; }

        public NetworkPed() {

        }
    }
}
