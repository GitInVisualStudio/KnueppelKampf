﻿using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
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

        private Label debugData;
        private Button connectBtn;

        public GameWindow() : base(60, 30)
        {
            InitializeComponent();

            client = new Client("localhost");
            client.StartConnecting();

            debugData = new Label()
            {
                AutoSize = true
            };
            Controls.Add(debugData);

            connectBtn = new Button()
            {
                Text = "Connect",
                Location = new Point(100, 0)
            };
            Controls.Add(connectBtn);
            connectBtn.Click += (object sender, EventArgs e) => client.StartQueueing();
        }

        public override void Init()
        {
            base.Init();
            this.worldManager.Entities.Add(new Player(new Vector(50.0f, 50.0f)));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            SendInputPacket();
            if (client.IsTimedOut())
                MessageBox.Show("Connection to server timed out.");

            debugData.Invoke(new MethodInvoker(() =>
            {
                debugData.Text = client.IngameStatus.ToString() + ". Game: " + client.GameId;
            }));
        }

        private void SendInputPacket()
        {
            if (client.ConnectionStatus == ConnectionStatus.Connected)
            {
                GameAction[] pressedActions = ActionManager.GetActions();
                
                InputPacket p = new InputPacket(client.XorSalt, pressedActions);
                client.SendPacket(p);
            }
        }
    }
}
