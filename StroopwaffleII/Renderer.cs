using Rage;
using Rage.Native;
using StroopwaffleII_Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class Renderer {
        private NetworkHandler NetworkHandler { get; set; }
        private bool ProcessingPedInterpolation { get; set;}
        Dictionary<NetworkClient, RenderPed> PhysicalPeds { get; set; }

        public Renderer(NetworkHandler networkHandler) {
            NetworkHandler = networkHandler;
            PhysicalPeds = new Dictionary<NetworkClient, RenderPed>();
        }

        public void Render() {
            foreach(NetworkClient networkClient in NetworkHandler.NetworkManager.NetworkClients) {
                // World ped does not exist
                if (!PhysicalPeds.ContainsKey(networkClient)) {
                    PhysicalPeds[networkClient] = new RenderPed();
                    PhysicalPeds[networkClient].BlockTasksHandler = new BlockTasksHandler();
                    PhysicalPeds[networkClient].Ped = new Ped("s_m_m_paramedic_01", new Vector3(networkClient.NetworkPed.PosX, networkClient.NetworkPed.PosY, networkClient.NetworkPed.PosZ), 0f);
                    PhysicalPeds[networkClient].Ped.IsPersistent = true;
                    PhysicalPeds[networkClient].Ped.BlockPermanentEvents = true;
                    PhysicalPeds[networkClient].Ped.CanBeDamaged = false;

                    WeaponAsset asset = new WeaponAsset("WEAPON_PISTOL");
                    PhysicalPeds[networkClient].Ped.Inventory.GiveNewWeapon(asset, 1000, true);
                }

                Ped ped = PhysicalPeds[networkClient].Ped;
                Vector3 pedPosition = new Vector3(networkClient.NetworkPed.PosX, networkClient.NetworkPed.PosY + 2, networkClient.NetworkPed.PosZ);
                Rotator pedRotation = new Rotator(networkClient.NetworkPed.Pitch, networkClient.NetworkPed.Roll, networkClient.NetworkPed.Yaw);
                Vector3 posLookAt = new Vector3(networkClient.NetworkPed.AimPosX, networkClient.NetworkPed.AimPosY, networkClient.NetworkPed.AimPosZ);
                Vector3 offsetFront = new Vector3(networkClient.NetworkPed.OffsetFrontX, networkClient.NetworkPed.OffsetFrontY, networkClient.NetworkPed.OffsetFrontZ);

                if (networkClient.NetworkPed.Aiming && !networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Sprinting) {
                    if (!networkClient.NetworkPed.Shooting && !PhysicalPeds[networkClient].BlockTasksHandler.Blocked) {
                        ped.Tasks.AimWeaponAt(posLookAt, 10);
                    }
                    else if (networkClient.NetworkPed.Shooting && !PhysicalPeds[networkClient].BlockTasksHandler.Blocked) {
                        NativeFunction.Natives.TaskShootAtCoord(ped, posLookAt.X, posLookAt.Y, posLookAt.Z, 355, 0xC6EE6B4C); // full auto
                        PhysicalPeds[networkClient].BlockTasksHandler.StartBlock(355);
                    }
                }
                else if (networkClient.NetworkPed.Aiming && (networkClient.NetworkPed.Walking || networkClient.NetworkPed.Running || networkClient.NetworkPed.Sprinting)) { // running etc.
                    if (!PhysicalPeds[networkClient].AimFiberActive && !networkClient.NetworkPed.Shooting) {
                        PhysicalPeds[networkClient].AimFiberActive = true;
                        GameFiber.StartNew(delegate {
                            ped.Tasks.GoToWhileAiming(offsetFront, posLookAt, 0f, networkClient.NetworkPed.Speed, false, FiringPattern.FullAutomatic);
                            GameFiber.Wait(350);
                            PhysicalPeds[networkClient].AimFiberActive = false;
                        });
                    }
                    else if (networkClient.NetworkPed.Shooting) {
                        ped.Tasks.GoToWhileAiming(offsetFront, posLookAt, 0f, networkClient.NetworkPed.Speed, true, FiringPattern.FullAutomatic);
                    }
                }
                else if (!networkClient.NetworkPed.Aiming && !networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Sprinting) {
                    ped.Tasks.Clear();
                    ped.Heading = networkClient.NetworkPed.Heading;
                }
                else if (!networkClient.NetworkPed.Aiming && networkClient.NetworkPed.Walking && !networkClient.NetworkPed.Running && !networkClient.NetworkPed.Running) {
                    Console.WriteLine("Walking");
                    ped.Tasks.GoStraightToPosition(offsetFront, 1f, networkClient.NetworkPed.Heading, 0f, 10);
                }
                else if (!networkClient.NetworkPed.Aiming && !networkClient.NetworkPed.Walking && (networkClient.NetworkPed.Running || networkClient.NetworkPed.Sprinting)) {
                    Console.WriteLine("Running");
                    ped.Tasks.GoStraightToPosition(offsetFront, 4f, networkClient.NetworkPed.Heading, 0f, 10);
                }

                // Interpolation
                if(NetworkHandler.BetweenFrames) {
                    // 1 / 60 = 0.016! * 1000 = 1.666!
                    float hz = (1 / (float)SendUpdatesThread.HERTZ) * 1000f;

                    GameFiber.StartNew(delegate {
                        // divide by 2 so we can use Sleep(1)
                        for (float till = 0; till < hz / 2f; till += 1f) {
                            Vector3 lerped = new Vector3();

                            lerped.X = MathHelper.Lerp(ped.Position.X, pedPosition.X, till / (hz / 2f));
                            lerped.Y = MathHelper.Lerp(ped.Position.Y, pedPosition.Y, till / (hz / 2f));
                            lerped.Z = MathHelper.Lerp(ped.Position.Z, pedPosition.Z, till / (hz / 2f));

                            ped.Position = lerped;
                            GameFiber.Sleep(1);
                        }
                        NetworkHandler.BetweenFrames = false;
                    }); 
                }
          
                ped.Rotation = pedRotation;
            }
        }

        public void Clear() {
            PhysicalPeds.Clear();
        }
    }
}
