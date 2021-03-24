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
            mouse = mouse - new Vector(Width / 2, Height / 2);
            if(thePlayer != null)
                thePlayer.Rotation = mouse.Angle;
            //TODO: send rotation to the server
        }

        public override void Init()
        {
            base.Init();

            worldManager = new WorldManager();
            //worldManager.Entities.Add(thePlayer = new Player(new Vector(300, 300)));
            client = new Client("localhost", worldManager);
            client.StartConnecting();
            client.GameInitialized += (object sender, GameObject player) =>
            {
                thePlayer = (Player)player;
                control = thePlayer.GetComponent<ControlComponent>();
                this.worldManager.Camera = thePlayer;
            };
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            //GameAction[] pressedActions = ActionManager.GetActions();

            //control.HandleInputs(pressedActions);

            SendInputOrKeepAlive();
            if (client.IsTimedOut())
                MessageBox.Show("Connection to server timed out.");

            //client.StartGettingGameInfo();
            debugData.Invoke(new MethodInvoker(() =>
            {
                debugData.Text = client.IngameStatus.ToString() + "\nYourEntityId: " + thePlayer?.Id + "\nStateId: " + client.WorldStateAck;
            }));
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
                if (client.IngameStatus == IngameStatus.InRunningGame)
                {
                    GameAction[] pressedActions = ActiveForm == this ? ActionManager.GetActions() : new GameAction[] { };
                    InputPacket p = new InputPacket(client.XorSalt, pressedActions, client.WorldStateAck);
                    client.SendPacket(p);
                    //control?.HandleInputs(pressedActions);
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
