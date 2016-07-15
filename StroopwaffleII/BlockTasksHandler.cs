using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class BlockTasksHandler {
        private GameFiber Block { get; set; }
        public bool Blocked { get; set; }

        public BlockTasksHandler() {

        }

        public void StartBlock(int until) {
            Console.WriteLine("Starting block...");
            int counter = 0;
            Blocked = true;

            GameFiber.StartNew(delegate {
                while(Blocked) {
                    if (counter == until) {
                        Blocked = false;
                        Console.WriteLine("UnBlocked");
                    }
                    counter++;
                }
            });
        }
    }
}
