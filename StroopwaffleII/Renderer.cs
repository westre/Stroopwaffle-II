using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class Renderer {
        private NetworkHandler NetworkHandler { get; set; }

        public Renderer(NetworkHandler networkHandler) {
            NetworkHandler = networkHandler;
        }
    }
}
