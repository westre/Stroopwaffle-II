using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StroopwaffleII_Shared;

[assembly: Rage.Attributes.Plugin("Stroopwaffle II", Description = "GTA V programmable multiplayer modification", Author = "westre")]

namespace StroopwaffleII {
    public class EntryPoint {
        private GameInitializer GameInitializer { get; set; }
        private NetworkManager NetworkManager { get; set; }

        public EntryPoint() {
            GameInitializer = new GameInitializer();
            GameInitializer.DisableScripts();

            NetworkManager = new NetworkManager();

            while (true) {
                GameInitializer.DisableByFrame();
                GameInitializer.RemoveAllEntities();

                if (Game.IsKeyDown(Keys.NumPad0)) {
                    Game.DisplayNotification("Pressed NumPad0" + Guid.NewGuid());
                }

                // Allow other plugins and the game to process.
                GameFiber.Yield();
            }
        }

        public static void Main() {
            new EntryPoint();
        }
    }
}
