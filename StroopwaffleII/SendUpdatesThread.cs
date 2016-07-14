using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class SendUpdatesThread {

        private NetworkHandler NetworkHandler { get; set; }

        private const int HERTZ = 1;
        private const int SKIP_TICKS = 1000 / HERTZ;

        private int OldTickCount { get; set; }
        private int NewTickCount { get; set; }
        private int TickRegister { get; set; }

        public SendUpdatesThread(NetworkHandler networkHandler) {
            this.NetworkHandler = networkHandler;
        }

        public void OnPluginUpdate() {
            OldTickCount = NewTickCount;
            NewTickCount = Environment.TickCount;

            if(OldTickCount > 0 && NewTickCount > 0) {
                int deltaTime = NewTickCount - OldTickCount;

                TickRegister += deltaTime;

                while(TickRegister >= SKIP_TICKS) {
                    Tick();
                    TickRegister -= SKIP_TICKS;
                }
            }        
        }

        private void Tick() {
            Console.WriteLine("Tick " + Guid.NewGuid());
        }
    }
}
