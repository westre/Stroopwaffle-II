using Lidgren.Network;
using Rage;
using Rage.Native;
using StroopwaffleII_Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class SendUpdatesThread {

        private NetworkHandler NetworkHandler { get; set; }

        private const int HERTZ = 30;
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
            if(NetworkHandler.LidgrenClient.ConnectionStatus == NetConnectionStatus.Connected) {

                if(NetworkHandler.NetworkManager.GetLocalPlayer() != null && NetworkHandler.NetworkManager.GetLocalPlayer().SafeForNet) {
                    // Send ped packet to the server
                    PlayerPedPacket playerPedPacket = ConstructPlayerPedPacket();
                    NetOutgoingMessage message = NetworkHandler.LidgrenClient.CreateMessage();
                    playerPedPacket.Pack(message);
                    NetworkHandler.LidgrenClient.SendMessage(message, NetDeliveryMethod.Unreliable);
                } 
            }
        }

        private PlayerPedPacket ConstructPlayerPedPacket() {
            Vector3 camPosition = NativeFunction.Natives.GetGameplayCamCoord<Vector3>();
            Vector3 rot = NativeFunction.Natives.GetGameplayCamRot<Vector3>(0);
            Vector3 dir = Utility.RotationToDirection(rot);
            Vector3 posLookAt = camPosition + dir * 1000f;

            PlayerPedPacket packet = new PlayerPedPacket();
            packet.ParentId = NetworkHandler.NetworkManager.GetLocalPlayer().ID;
            packet.PosX = Game.LocalPlayer.Character.Position.X;
            packet.PosY = Game.LocalPlayer.Character.Position.Y;
            packet.PosZ = Game.LocalPlayer.Character.Position.Z;
            packet.Pitch = Game.LocalPlayer.Character.Rotation.Pitch;
            packet.Roll = Game.LocalPlayer.Character.Rotation.Roll;
            packet.Yaw = Game.LocalPlayer.Character.Rotation.Yaw;

            packet.Aiming = Game.LocalPlayer.IsFreeAiming;
            packet.AimPosX = posLookAt.X;
            packet.AimPosY = posLookAt.Y;
            packet.AimPosZ = posLookAt.Z;

            packet.Shooting = Game.LocalPlayer.Character.IsShooting;
            packet.Walking = Game.LocalPlayer.Character.IsWalking;
            packet.Running = Game.LocalPlayer.Character.IsRunning;
            packet.Sprinting = Game.LocalPlayer.Character.IsSprinting;
            packet.Jumping = Game.LocalPlayer.Character.IsJumping;
            packet.Reloading = Game.LocalPlayer.Character.IsReloading;

            packet.Heading = Game.LocalPlayer.Character.Heading;

            packet.OffsetFrontX = Game.LocalPlayer.Character.GetOffsetPositionFront(10f).X;
            packet.OffsetFrontY = Game.LocalPlayer.Character.GetOffsetPositionFront(10f).Y;
            packet.OffsetFrontZ = Game.LocalPlayer.Character.GetOffsetPositionFront(10f).Z;
            packet.Speed = Game.LocalPlayer.Character.Speed;

            return packet;
        }
    }
}
