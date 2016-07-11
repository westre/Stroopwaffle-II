using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StroopwaffleII_Shared;
using Lidgren.Network;

[assembly: Rage.Attributes.Plugin("Stroopwaffle II", Description = "GTA V programmable multiplayer modification", Author = "Kevin de K. (westre)")]

namespace StroopwaffleII {
    public class EntryPoint {
        private GameInitializer GameInitializer { get; set; }
        private NetworkManager NetworkManager { get; set; }
        private NetworkHandler NetworkHandler { get; set; }

        public EntryPoint() {
            GameInitializer = new GameInitializer();
            GameInitializer.DisableScripts();

            NetworkManager = new NetworkManager();

            NetworkHandler = new NetworkHandler();

            while (true) {
                GameInitializer.DisableByFrame();
                GameInitializer.RemoveAllEntities();

                if (Game.IsKeyDown(Keys.NumPad0)) {
                    Game.DisplayNotification("Pressed NumPad0" + Guid.NewGuid());

                    if(NetworkHandler.LidgrenClient.ServerConnection == null) {
                        NetConnection connection = NetworkHandler.Connect("192.168.1.133");
                        if(connection != null) {
                            Game.DisplayNotification("Connected");
                        }
                    }
                    else {
                        NetworkHandler.Disconnect();
                        Game.DisplayNotification("Disconnected");
                    }
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
