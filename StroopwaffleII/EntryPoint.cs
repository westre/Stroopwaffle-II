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
        private NetworkHandler NetworkHandler { get; set; }
        private SendUpdatesThread SendUpdatesThread { get; set; }
        private GraphicsRenderer GraphicsRenderer { get; set; }
        private Renderer Renderer { get; set; }

        public EntryPoint() {
            GameInitializer = new GameInitializer();
            GameInitializer.DisableScripts();

            NetworkHandler = new NetworkHandler();

            SendUpdatesThread = new SendUpdatesThread(NetworkHandler);
            GraphicsRenderer = new GraphicsRenderer(NetworkHandler);
            Renderer = new Renderer(NetworkHandler);

            while (true) {
                GameInitializer.DisableByFrame();
                GameInitializer.RemoveAllEntities();

                if (Game.IsKeyDown(Keys.NumPad0)) {
                    Game.DisplayNotification("Pressed NumPad0" + Guid.NewGuid());

                    if (NetworkHandler.LidgrenClient.ServerConnection == null) {
                        NetConnection connection = NetworkHandler.Connect("192.168.1.133");
                        if (connection != null) {
                            Game.DisplayNotification("Connecting...");
                        }
                    }
                    else {
                        NetworkHandler.Disconnect();
                        Renderer.Clear();
                        Game.DisplayNotification("Disconnected");
                    }
                }

                if(Game.IsKeyDown(Keys.NumPad1)) {
                    Ped p = new Ped("s_m_m_paramedic_01", Game.LocalPlayer.Character.Position, 0f);
                    p.IsPersistent = true;
                    p.BlockPermanentEvents = true;
                    p.CanBeDamaged = false;
                }

                if (NetworkHandler.LidgrenClient != null && NetworkHandler.LidgrenClient.ServerConnection != null) {
                    NetworkHandler.ReadPackets();
                }

                SendUpdatesThread.OnPluginUpdate();
                Renderer.Render();

                // Allow other plugins and the game to process.
                GameFiber.Yield();
            }
        }

        public static void Main() {
            new EntryPoint();
        }
    }
}
