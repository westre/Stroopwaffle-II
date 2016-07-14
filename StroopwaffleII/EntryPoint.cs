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

        private Ped TestPed { get; set; }

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
                    WeaponAsset asset = new WeaponAsset("WEAPON_PISTOL");
                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon(asset, 1000, true);
                }

                if (Game.IsKeyDown(Keys.NumPad2)) {
                    TestPed = new Ped("s_m_m_paramedic_01", Game.LocalPlayer.Character.Position, 0f);
                    TestPed.IsPersistent = true;
                    TestPed.BlockPermanentEvents = true;
                    TestPed.CanBeDamaged = false;

                    WeaponAsset asset = new WeaponAsset("WEAPON_PISTOL");
                    TestPed.Inventory.GiveNewWeapon(asset, 1000, true);
                }

                if (NetworkHandler.LidgrenClient != null && NetworkHandler.LidgrenClient.ServerConnection != null) {
                    NetworkHandler.ReadPackets();
                }

                SendUpdatesThread.OnPluginUpdate();
                Renderer.Render();

                if(TestPed != null) {
                    Vector3 pedPosition = new Vector3(Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y + 2, Game.LocalPlayer.Character.Position.Z);
                    Rotator pedRotation = new Rotator(Game.LocalPlayer.Character.Rotation.Pitch, Game.LocalPlayer.Character.Rotation.Roll, Game.LocalPlayer.Character.Rotation.Yaw);


                    /*if(!networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Sprinting) {
                        ped.Heading = networkClient.NetworkPed.Heading;
                    }
                    else if(networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Running) {
                        ped.Tasks.GoStraightToPosition(pedPosition, 1f, networkClient.NetworkPed.Heading, 0f, 10);
                    }
                    else if(!networkClient.NetworkPed.Walking && (networkClient.NetworkPed.Running || networkClient.NetworkPed.Sprinting)) {
                        ped.Tasks.GoStraightToPosition(pedPosition, 4f, networkClient.NetworkPed.Heading, 0f, 10);
                    }*/

                    TestPed.Position = Game.LocalPlayer.Character.Position;
                    //TestPed.Rotation = pedRotation;

                    TestPed.Tasks.GoStraightToPosition(Game.LocalPlayer.Character.GetOffsetPositionFront(10f), 4f, Game.LocalPlayer.Character.Heading, 0f, 10); // ARGH
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
