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
        private Vehicle TestVehicle { get; set; }
        
        private BlockTasksHandler BlockTasksHandler { get; set; }      

        public EntryPoint() {
            GameInitializer = new GameInitializer();
            GameInitializer.DisableScripts();

            NetworkHandler = new NetworkHandler();

            SendUpdatesThread = new SendUpdatesThread(NetworkHandler);
            GraphicsRenderer = new GraphicsRenderer(NetworkHandler);
            Renderer = new Renderer(NetworkHandler);

            BlockTasksHandler = new BlockTasksHandler();

            GameInitializer.RemoveAllEntities();

            while (true) {
                GameInitializer.DisableByFrame();

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
                    GameFiber.StartNew(delegate {
                        Vector3 pos = Game.LocalPlayer.Character.GetOffsetPositionFront(5f);
                        Vector3 pos2 = Game.LocalPlayer.Character.GetOffsetPositionFront(10f);

                        TestPed = new Ped("s_m_m_paramedic_01", Game.LocalPlayer.Character.GetOffsetPositionFront(2f), 0f);
                        TestPed.IsPersistent = true;
                        TestPed.BlockPermanentEvents = true;
                        TestPed.CanBeDamaged = false;

                        WeaponAsset asset = new WeaponAsset("WEAPON_PISTOL");
                        TestPed.Inventory.GiveNewWeapon(asset, 1000, true);

                        GameFiber.Sleep(2000);

                        TestVehicle = new Vehicle("ENTITYXF", pos);
                        new Vehicle("ENTITYXF", pos2);

                        GameFiber.Sleep(1000);

                        TestPed.Tasks.EnterVehicle(TestVehicle, -1);
                    });

                    //Game.LocalPlayer.Character.CurrentVehicle.

                    //new Vehicle("ENTITYXF", Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront * 5f));
  
                }

                if (NetworkHandler.LidgrenClient != null && NetworkHandler.LidgrenClient.ServerConnection != null) {
                    NetworkHandler.ReadPackets();
                }

                SendUpdatesThread.OnPluginUpdate();
                Renderer.Render();

                /*if(TestPed != null) {
                    Vector3 pedPosition = new Vector3(Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y + 2, Game.LocalPlayer.Character.Position.Z);
                    Rotator pedRotation = new Rotator(Game.LocalPlayer.Character.Rotation.Pitch, Game.LocalPlayer.Character.Rotation.Roll, Game.LocalPlayer.Character.Rotation.Yaw);
                    Ped localPlayer = Game.LocalPlayer.Character;

                    Vector3 camPosition = NativeFunction.Natives.GetGameplayCamCoord<Vector3>();
                    Vector3 rot = NativeFunction.Natives.GetGameplayCamRot<Vector3>(0);
                    Vector3 dir = Utility.RotationToDirection(rot);
                    Vector3 posLookAt = camPosition + dir * 1000f;

                    if (Game.LocalPlayer.IsFreeAiming && !localPlayer.IsWalking && !localPlayer.IsRunning && !localPlayer.IsSprinting) {
                        if (!localPlayer.IsShooting && !BlockTasksHandler.Blocked) {
                            TestPed.Tasks.AimWeaponAt(posLookAt, 10);
                        }        
                        else if(localPlayer.IsShooting && !BlockTasksHandler.Blocked) {
                            NativeFunction.Natives.TaskShootAtCoord(TestPed, posLookAt.X, posLookAt.Y, posLookAt.Z, 355, 0xC6EE6B4C); // full auto
                            BlockTasksHandler.StartBlock(355);
                        }
                    }
                    else if (Game.LocalPlayer.IsFreeAiming && (localPlayer.IsWalking || localPlayer.IsRunning || localPlayer.IsSprinting)) { // running etc.
                        if (!AimFiberActive && !localPlayer.IsShooting) {
                            AimFiberActive = true;
                            GameFiber.StartNew(delegate {
                                TestPed.Tasks.GoToWhileAiming(Game.LocalPlayer.Character.GetOffsetPositionFront(10f), posLookAt, 0f, localPlayer.Speed, false, FiringPattern.SingleShot);
                                GameFiber.Wait(350);
                                AimFiberActive = false;
                            });
                        }
                        else if (localPlayer.IsShooting) {
                            TestPed.Tasks.GoToWhileAiming(Game.LocalPlayer.Character.GetOffsetPositionFront(10f), posLookAt, 0f, localPlayer.Speed, true, FiringPattern.SingleShot);
                        }
                    }
                    else if (!Game.LocalPlayer.IsFreeAiming && !localPlayer.IsWalking && !localPlayer.IsRunning && !localPlayer.IsSprinting) {
                        TestPed.Tasks.Clear();
                        TestPed.Heading = localPlayer.Heading;
                    }
                    else if (!Game.LocalPlayer.IsFreeAiming && localPlayer.IsWalking && !localPlayer.IsRunning && !localPlayer.IsRunning) {
                        Console.WriteLine("Walking");
                        TestPed.Tasks.GoStraightToPosition(Game.LocalPlayer.Character.GetOffsetPositionFront(10f), 1f, localPlayer.Heading, 0f, 10);
                    }
                    else if (!Game.LocalPlayer.IsFreeAiming && !localPlayer.IsWalking && (localPlayer.IsRunning || localPlayer.IsSprinting)) {
                        Console.WriteLine("Running");
                        TestPed.Tasks.GoStraightToPosition(Game.LocalPlayer.Character.GetOffsetPositionFront(10f), 4f, localPlayer.Heading, 0f, 10);
                    }

                    TestPed.Position = pedPosition;
                    TestPed.Rotation = pedRotation;
                }*/

                // Allow other plugins and the game to process.
                GameFiber.Yield();
            }
        }

        public static void Main() {
            new EntryPoint();
        }
    }
}
