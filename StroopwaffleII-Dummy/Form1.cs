using Lidgren.Network;
using StroopwaffleII;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StroopwaffleII_Dummy {
    public partial class Form1 : Form {
        private NetPeerConfiguration Config { get; set; }
        public NetClient LidgrenClient { get; set; }

        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Config = new NetPeerConfiguration("sw2");
            Config.AutoFlushSendQueue = false;

            LidgrenClient = new NetClient(Config);
            LidgrenClient.RegisterReceivedCallback(new SendOrPostCallback(ReadPackets));

            LidgrenClient.Start();
            NetOutgoingMessage hail = LidgrenClient.CreateMessage("Hail, citizen!");
            LidgrenClient.Connect("192.168.1.133", 7777, hail);  
        }

        public void ReadPackets(object peer) {
            if(LidgrenClient.ServerConnection != null) {
                NetIncomingMessage netIncomingMessage;

                while ((netIncomingMessage = LidgrenClient.ServerConnection.Peer.ReadMessage()) != null) {
                    if (netIncomingMessage.MessageType == NetIncomingMessageType.StatusChanged) {
                        NetConnectionStatus status = (NetConnectionStatus)netIncomingMessage.ReadByte();

                        if (status == NetConnectionStatus.Connected) {
                            Console.WriteLine("Connected");
                            LidgrenClient.FlushSendQueue();
                        }
                    }
                    LidgrenClient.Recycle(netIncomingMessage);
                }
            }    
        }
    }
}
