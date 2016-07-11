using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using System.Threading;

namespace StroopwaffleII_Server {
    class Server {

        private NetServer NetServer { get; set; }

        public Server() {
            NetPeerConfiguration config = new NetPeerConfiguration("sw2");
            config.Port = 7777;
            config.MaximumConnections = 100;

            NetServer = new NetServer(config);
            Thread serverThread = new Thread(ServerThread);
            serverThread.Start();
        }

        public void ServerThread() {
            Console.WriteLine("Starting server...");

            while(true) {
                NetIncomingMessage incomingMessage;
                while ((incomingMessage = NetServer.ReadMessage()) != null) {
                    switch (incomingMessage.MessageType) {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = incomingMessage.ReadString();
                            Console.WriteLine("[WARNING] " + text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)incomingMessage.ReadByte();

                            string reason = incomingMessage.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(incomingMessage.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected) {
                                Console.WriteLine("|_ connected");
                            }
                            else if (status == NetConnectionStatus.Disconnected) {
                                Console.WriteLine("|_ disconnected");
                            }
                            break;
                    }
                    NetServer.Recycle(incomingMessage);
                }
                Thread.Sleep(1);
            }
        }

        static void Main(string[] args) {
            new Server();
        }
    }
}
