using KnueppelKampfBase.Game;
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

        private Label debugData;
        private Button connectBtn;

        public GameWindow() : base(60, 30)
        {
            InitializeComponent();

            manager = new WorldManager();
            client = new Client("localhost", manager);
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Vector mouse = new Vector(e.X, e.Y);
            mouse = mouse - thePlayer.Position;
            //thePlayer.Rotation = mouse.Angle;
            //TODO: send rotation to the server
        }

        public override void Init()
        {
            base.Init();
            this.worldManager.Entities.Add(thePlayer = new Player(new Vector(50.0f, 50.0f)));
            //this.worldManager.Entities.Add(new Player(new Vector(250.0f, 150.0f)));
            //this.worldManager.Entities.Add(new Floor(new Vector(100, 50), new Vector(50, 50)));
            this.worldManager.Entities.Add(new Floor(new Vector(100, 250), new Vector(500, 50)));
            control = thePlayer.GetComponent<ControlComponent>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            SendInputOrKeepAlive();
            if (client.IsTimedOut())
                MessageBox.Show("Connection to server timed out.");

            client.StartGettingGameInfo();
            debugData.Invoke(new MethodInvoker(() =>
            {
                debugData.Text = client.IngameStatus.ToString() + ". Game: " + client.GameInfo;
            }));
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
