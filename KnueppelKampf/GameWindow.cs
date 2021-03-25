using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Networking;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
using KnueppelKampfBase.Render;
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
        private Player thePlayer;
        private ControlComponent control;
        private Label debugData;
        private Button connectBtn;

        public GameWindow() : base(60, WorldManager.TPS)
        {
            InitializeComponent();

            debugData = new Label()
            {
                AutoSize = true
            };
            //Controls.Add(debugData);

            connectBtn = new Button()
            {
                Text = "Connect",
                Location = new Point(100, 0)
            };
            Controls.Add(connectBtn);
            connectBtn.Click += (object sender, EventArgs e) => client.StartQueueing();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Vector mouse = new Vector(e.X, e.Y);
            mouse = mouse - new Vector(Width / 2, Height / 2);
            if(thePlayer != null)
                thePlayer.Rotation = mouse.Angle;
        }

        public override void Init()
        {
            base.Init();

            worldManager = new WorldManager();
            client = new Client("localhost", worldManager);
            client.StartConnecting();
            client.GameInitialized += (object sender, GameObject player) =>
            {
                thePlayer = (Player)player;
                control = thePlayer.GetComponent<ControlComponent>();
                this.worldManager.Camera = thePlayer;
                connectBtn.Hide();
            };
            client.GameEnded += (object sender, EventArgs e) =>
            {
                Vector offset = this.worldManager.Offset;
                this.worldManager = new WorldManager();
                this.worldManager.Offset = offset;
                client.Manager = worldManager;
                thePlayer = null;
                control = null;
                connectBtn.Show();
            };
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            SendInputOrKeepAlive();
            if (client.IsTimedOut())
                MessageBox.Show("Connection to server timed out.");

            //debugData.Invoke(new MethodInvoker(() =>
            //{
            //    debugData.Text = client.IngameStatus.ToString() + "\nYourEntityId: " + thePlayer?.Id + "\nStateId: " + client.WorldStateAck + "\nPlayer rotation: " + thePlayer?.Rotation;
            //}));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.worldManager.Offset = new Vector(this.Width, this.Height) / 2;
        }

        /// <summary>
        /// Sends an input packet and handles inputs if game is active, sends keep alive packet if inactive
        /// </summary>
        private void SendInputOrKeepAlive()
        {
            if (client.ConnectionStatus == ConnectionStatus.Connected)
            {
                if (client.IngameStatus == IngameStatus.InRunningGame)
                {
                    GameAction[] pressedActions = ActiveForm == this ? ActionManager.GetActions() : new GameAction[] { };
                    InputPacket p = new InputPacket(client.XorSalt, pressedActions, client.WorldStateAck, thePlayer.Rotation);
                    client.SendPacket(p);
                    control?.HandleInputs(pressedActions);
                }
                else
                {
                    KeepAlivePacket kap = new KeepAlivePacket(client.XorSalt);
                    client.SendPacket(kap);
                }
            }
        }
    }
}
