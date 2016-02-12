using Mentula.GuiItems.Containers;
using Mentula.GuiItems.Core;
using Mentula.GuiItems.Items;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;

namespace Mentula.Client.Menus
{
    public sealed class MultiplayerMenu : Menu
    {
        public event EventHandler<object[]> DiscoverCalled;

        private const string TXTNAME = "TxtName";
        private const string TXTHOST = "TxtHost";
        private const string LBLERROR = "LblError";
        private MainGame MGame { get { return (MainGame)Game; } }

        public MultiplayerMenu(MainGame game)
            : base(game)
        { }

        public override void Initialize()
        {
            font = MGame.vGraphics.fonts["MenuFont"];
            int txtW = 150, txtH = 25;
            int txtWM = txtW >> 1, txtHM = txtH >> 1;

            AddGuiItem(
                Name: "Background",
                BackgroundImage: MGame.vGraphics.textures[10001],
                Enabled: false);

            TextBox txtName = AddTextBox(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle - txtHM),
                Width: txtW,
                Height: txtH,
                Text: "UserName",
                Multiline: true,
                Name: TXTNAME,
                BackColor: MainMenu.ButtonBackColor);

            AddTextBox(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 8),
                Width: txtW,
                Height: txtH,
                Text: Ips.Frank.ToString(),
                Name: TXTHOST,
                BackColor: MainMenu.ButtonBackColor);

            Button btnConnect = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 16),
                Width: txtW,
                Height: txtH,
                Text: "Connect",
                BackColor: MainMenu.ButtonBackColor);

            Button btnBack = AddButton(
                Position: new Vector2(0, ScreenHeightMiddle + txtHM * 16),
                Width: txtW,
                Height: txtH,
                Text: "Back",
                BackColor: MainMenu.ButtonBackColor);

            AddLabel(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM),
                Width: txtW,
                Height: txtH,
                AutoSize: true,
                BackColor: Color.Transparent,
                ForeColor: Color.Red,
                Name: LBLERROR);

            txtName.TextChanged += (sender, args) =>
            {
                if (args.Length > 0 && args[args.Length - 1] == '\n')
                {
                    txtName.Text = args.Remove(args.Length - 1);
                    btnConnect.PerformClick(MouseClick.Left);
                }
            };

            btnConnect.LeftClick += btnConnect_LeftClick;
            btnBack.Click += (sender, args) => MGame.SetState(GameState.MainMenu);

            base.Initialize();
        }

        public void SetError(string msg)
        {
            FindControl<Label>(LBLERROR).Text = msg;
        }

        private void btnConnect_LeftClick(GuiItem sender, MouseState state)
        {
            string host = FindControl<TextBox>(TXTHOST).Text.Replace(" ", "");
            string name = FindControl<TextBox>(TXTNAME).Text;

            if (name.Length < 1 || name.Length > 16)
            {
                SetError("Name must be between 1 and 16 characters");
                return;
            }

            if (host.ToUpper() == "LOCALHOST")
            {
                if (DiscoverCalled != null) DiscoverCalled(this, new object[1] { name });
                return;
            }

            string[] subHost = host.Split('.');
            if (subHost.Length != 4) goto HostError;

            byte[] address = new byte[subHost.Length];
            for (int i = 0; i < subHost.Length; i++)
            {
                if (!byte.TryParse(subHost[i], out address[i])) goto HostError;
            }

            IPAddress ip = new IPAddress(address);

            if (DiscoverCalled != null) DiscoverCalled(this, new object[2] { name, ip });
            return;

        HostError:
            SetError("Host invalid.");
            return;
        }
    }
}
