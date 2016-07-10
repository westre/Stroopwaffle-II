using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII_Shared {
    public class NetworkEntity {

        protected int ID { get; set; }

        public NetworkEntity(int id) {
            this.ID = id;
        }
    }
}
