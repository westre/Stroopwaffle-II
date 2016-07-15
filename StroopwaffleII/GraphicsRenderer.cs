using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StroopwaffleII_Shared;

namespace StroopwaffleII {
    class GraphicsRenderer {
        private NetworkHandler NetworkHandler { get; set; }


        public GraphicsRenderer(NetworkHandler networkHandler) {
            NetworkHandler = networkHandler;

            Game.RawFrameRender += Game_RawFrameRender;
        }

        private void Game_RawFrameRender(object sender, GraphicsEventArgs e) {
            e.Graphics.DrawText("sizeof: " + NetworkHandler.NetworkManager.NetworkClients.Count, "Arial", 24f, new PointF(100, 100), Color.White);

            float y = 150f;
            foreach(NetworkClient networkClient in NetworkHandler.NetworkManager.NetworkClients) {
                e.Graphics.DrawText(
                    networkClient.ID + ": " + networkClient.NetworkPed.PosX + ", " + networkClient.NetworkPed.PosY + ", " + networkClient.NetworkPed.PosZ +
                    ", jumping: " + networkClient.NetworkPed.Jumping +
                    ", aiming: " + networkClient.NetworkPed.Aiming +
                    ", shooting: " + networkClient.NetworkPed.Shooting +
                    ", walking: " + networkClient.NetworkPed.Walking +
                    ", running: " + networkClient.NetworkPed.Running +
                    ", sprinting: " + networkClient.NetworkPed.Sprinting,
                    "Arial",
                    16f,
                    new PointF(100, y),
                    Color.White);

                y += 20f;
            }

            //e.Graphics.DrawText("LocalPos: " + Game.LocalPlayer.Character.Position.ToString(), "Arial", 16f, new PointF(100, 300), Color.White);
            //e.Graphics.DrawText("Fwd: " + Game.LocalPlayer.Character.ForwardVector.ToString(), "Arial", 16f, new PointF(100, 400), Color.White);

        }
    }
}
