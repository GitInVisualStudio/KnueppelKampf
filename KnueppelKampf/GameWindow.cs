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

        private Label debugData;

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
