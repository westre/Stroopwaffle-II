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

        private int Counter { get; set; }
        private bool BusyLerping { get; set; }

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

                        TestVehicle = new Vehicle("police", pos);
                        new Vehicle("police", pos2);

                        GameFiber.Sleep(1000);

                        TestPed.Tasks.EnterVehicle(TestVehicle, -1);
                    });

                    //Game.LocalPlayer.Character.CurrentVehicle.

                    //new Vehicle("ENTITYXF", Game.LocalPlayer.Character.GetOffsetPosition(Vector3.RelativeFront * 5f));
  
                }


                if(TestPed != null && TestPed.CurrentVehicle != null) {
                    TestPed.CurrentVehicle.IsEngineOn = true;
                    TestPed.CurrentVehicle.CanBeDamaged = false;
                    TestPed.CurrentVehicle.CanTiresBurst = false;
                    TestPed.CurrentVehicle.MakePersistent();

                    if(Game.LocalPlayer.Character.CurrentVehicle != null) {
                        TestPed.CurrentVehicle.SteeringAngle = Game.LocalPlayer.Character.CurrentVehicle.SteeringAngle;
                        TestPed.CurrentVehicle.SteeringScale = Game.LocalPlayer.Character.CurrentVehicle.SteeringScale;                   
                        TestPed.CurrentVehicle.CollisionIgnoredEntity = Game.LocalPlayer.Character.CurrentVehicle;
                        TestPed.CurrentVehicle.IsSirenOn = Game.LocalPlayer.Character.CurrentVehicle.IsSirenOn;
                        TestPed.CurrentVehicle.IsEngineStarting = Game.LocalPlayer.Character.CurrentVehicle.IsEngineStarting;
                        TestPed.CurrentVehicle.IsEngineOn = Game.LocalPlayer.Character.CurrentVehicle.IsEngineOn;

                        Game.LocalPlayer.Character.CurrentVehicle.Opacity = 0.1f;

                        // new code
                        NativeFunction.Natives.SetVehicleForwardSpeed(TestPed.CurrentVehicle, Game.LocalPlayer.Character.CurrentVehicle.Speed);
                        TestPed.CurrentVehicle.SetRotationYaw(Game.LocalPlayer.Character.CurrentVehicle.Rotation.Yaw);

                        if (TestPed.CurrentVehicle.Position.DistanceTo(Game.LocalPlayer.Character.CurrentVehicle.Position + new Vector3(0, 5, 0)) >= 0.6 && !BusyLerping) {
                            GameFiber.StartNew(delegate {
                                BusyLerping = true;
                                for (float percent = 0; percent < 1; percent += 0.05f) {
                                    Vector3 lerped = new Vector3();

                                    lerped.X = MathHelper.Lerp(TestPed.CurrentVehicle.Position.X, Game.LocalPlayer.Character.CurrentVehicle.Position.X, percent);
                                    lerped.Y = MathHelper.Lerp(TestPed.CurrentVehicle.Position.Y, Game.LocalPlayer.Character.CurrentVehicle.Position.Y + 5, percent);
                                    lerped.Z = MathHelper.Lerp(TestPed.CurrentVehicle.Position.Z, Game.LocalPlayer.Character.CurrentVehicle.Position.Z, percent);

                                    if (Game.LocalPlayer.Character.CurrentVehicle.IsInAir) {
                                        TestPed.CurrentVehicle.Rotation = Game.LocalPlayer.Character.CurrentVehicle.Rotation;
                                        TestPed.CurrentVehicle.Position = lerped;
                                    }
                                    else {
                                        TestPed.CurrentVehicle.SetPositionWithSnap(lerped);
                                    }
                                    NativeFunction.Natives.SetVehicleForwardSpeed(TestPed.CurrentVehicle, Game.LocalPlayer.Character.CurrentVehicle.Speed);
                                    GameFiber.Sleep(1);
                                }
                                BusyLerping = false;
                            });
                        }
                        //end of new code                
                    }
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
