using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

// http://www.koonsolo.com/news/dewitters-gameloop/
// http://gamedev.stackexchange.com/questions/44931/calculating-delta-time-what-is-wrong
// http://forum.unity3d.com/threads/multiplayer-game-tick-time.164531/

namespace StroopwaffleII_Server {
    class SendUpdatesThread {
        private Server Server { get; set; }
        private Thread Thread { get; set; }
        private const int HERTZ = 30;
        private const int SKIP_TICKS = 1000 / HERTZ;

        public SendUpdatesThread(Server server) {
            Server = server;

            Thread updateThread = new Thread(UpdateThread);
            updateThread.Start();
        }

        private void UpdateThread() {
            int nextGameTick = Environment.TickCount;
            int sleepTime = 0;

            while(true) {
                Tick();

                nextGameTick += SKIP_TICKS;
                sleepTime = nextGameTick - Environment.TickCount;

                if(sleepTime >= 0) {
                    Thread.Sleep(sleepTime);
                }
                /*else {
                    Console.WriteLine("We are running behind by " + sleepTime + "ms.");
                }*/
            }
        }

        private void Tick() {

        }
    }
}
