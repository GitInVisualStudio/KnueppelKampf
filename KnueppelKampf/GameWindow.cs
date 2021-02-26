﻿using KnueppelKampfBase.Game;
using KnueppelKampfBase.Networking;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnueppelKampf
{
    public partial class GameWindow : Window
    {
        private Client client;

        public GameWindow() : base(60, 30)
        {
            InitializeComponent();

            client = new Client("localhost");
            client.StartConnecting();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            SendInputPacket();
        }

        private void SendInputPacket()
        {
            if (client.ConnectionStatus == ConnectionStatus.Connected)
            {
                GameAction[] pressedActions = ActionManager.GetActions();
                InputPacket p = new InputPacket(client.XorSalt, pressedActions);
                client.SendPacket(p);
                Console.WriteLine("Sent input packet");
                return;
            }
            //Console.WriteLine("Client not connected, didn't send input packet");
        }
    }
}