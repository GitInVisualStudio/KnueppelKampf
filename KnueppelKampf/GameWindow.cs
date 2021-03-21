using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Components;
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
        private WorldManager manager;
        private Player thePlayer;
        private ControlComponent control;
        private Label debugData;
        private Button connectBtn;

        public GameWindow() : base(60, 30)
        {
            InitializeComponent();

            manager = new WorldManager();
            client = new Client("localhost", manager);
            //client.StartConnecting();

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
            //connectBtn.Click += (object sender, EventArgs e) => client.StartQueueing();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Vector mouse = new Vector(e.X, e.Y);
            mouse = mouse - new Vector(Width / 2, Height / 2);
            thePlayer.Rotation = mouse.Angle;
            //TODO: send rotation to the server
        }

        public override void Init()
        {
            base.Init();
            this.worldManager.Entities.Add(thePlayer = new Player(new Vector(50.0f, 50.0f)));
            this.worldManager.Entities.Add(new Player(new Vector(550.0f, 50.0f)));
            this.worldManager.Entities.Add(new Item(0)
            {
                Position = new Vector(550, 50)
            }) ;
            int width = 1920;
            //unten
            this.worldManager.Entities.Add(new Floor(new Vector(500, 850), new Vector(width * 0.5f, 500)));
            this.worldManager.Entities.Add(new Floor(new Vector(500 - 75, 850 - 100), new Vector(76, 600)));
            this.worldManager.Entities.Add(new Floor(new Vector(500 + width * 0.5f - 1, 850 - 100), new Vector(76, 600)));
            
            //pillar
            this.worldManager.Entities.Add(new Floor(new Vector(500 - 75 - 50, 850 - 100 - 300), new Vector(75 + 100, 75)));
            this.worldManager.Entities.Add(new Floor(new Vector(500 + width * 0.5f - 50, 850 - 100 - 300), new Vector(75 + 100, 75)));


            //seiten
            this.worldManager.Entities.Add(new Floor(new Vector(50, 650), new Vector(150, 600)));
            this.worldManager.Entities.Add(new Floor(new Vector(width * 0.5f + 800, 650), new Vector(150, 600)));

            //augen
            this.worldManager.Entities.Add(new Floor(new Vector(500 + width * 0.25f - 300, 250), new Vector(150, 450)));
            this.worldManager.Entities.Add(new Floor(new Vector(500 + width * 0.25f + 150, 250), new Vector(150, 450)));

            this.worldManager.Camera = thePlayer;

            control = thePlayer.GetComponent<ControlComponent>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            GameAction[] pressedActions = ActionManager.GetActions();

            control.HandleInputs(pressedActions);

            //SendInputOrKeepAlive();
            //if (client.IsTimedOut())
            //    MessageBox.Show("Connection to server timed out.");

            //client.StartGettingGameInfo();
            //debugData.Invoke(new MethodInvoker(() =>
            //{
            //    debugData.Text = client.IngameStatus.ToString() + ". Game: " + client.GameInfo;
            //}));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.worldManager.Offset = new Vector(this.Width, this.Height) / 2;
        }

        private void SendInputOrKeepAlive()
        {
            if (client.ConnectionStatus == ConnectionStatus.Connected)
            {
                if (client.IngameStatus == IngameStatus.InGame)
                {
                    GameAction[] pressedActions = ActionManager.GetActions();

                    InputPacket p = new InputPacket(client.XorSalt, pressedActions, client.WorldStateAck);
                    client.SendPacket(p);
                    control.HandleInputs(pressedActions);
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
