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

        public GameWindow() : base(60, 30)
        {
            InitializeComponent();

            //client = new Client("localhost");
            //client.StartConnecting();

            //debugData = new Label()
            //{
            //    AutoSize = true
            //};
            //Controls.Add(debugData);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Vector mouse = new Vector(e.X, e.Y);
            mouse = mouse - thePlayer.Position;
            //thePlayer.Rotation = mouse.Angle;
            //TODO: send rotation to the server
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            //TODO: send gameaction
        }

        public override void Init()
        {
            base.Init();
            this.worldManager.Entities.Add(thePlayer = new Player(new Vector(50.0f, 50.0f)));
            this.worldManager.Entities.Add(new Player(new Vector(550.0f, 50.0f)));
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
            //TODO: processing input packets
            control.HandleInputs(pressedActions);
            //SendInputPacket();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.worldManager.Offset = new Vector(this.Width, this.Height) / 2;
        }

        private void SendInputPacket()
        {
            if (client.ConnectionStatus == ConnectionStatus.Connected)
            {
                GameAction[] pressedActions = ActionManager.GetActions();
                Invoke(new MethodInvoker(() =>
                {
                    debugData.Text = ActionArrayToString(pressedActions);
                }));
                InputPacket p = new InputPacket(client.XorSalt, pressedActions);
                client.SendPacket(p);
                Console.WriteLine("Sent input packet");
                return;
            }
            Console.WriteLine("Client not connected, didn't send input packet");
        }

        private string ActionArrayToString(GameAction[] pressedActions)
        {
            string res = "";
            foreach (GameAction g in pressedActions)
                res += g.ToString();
            return res;
        }
    }
}
