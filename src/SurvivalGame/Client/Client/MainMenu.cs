using Mentula.GuiItems.Core;
using Mentula.GuiItems.Items;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;

namespace Mentula.Client
{
    public class MainMenu : GameComponent, IDrawable
    {
        public int DrawOrder { get { return 0; } }
        public bool Visible { get; set; }

        internal MenuState menuState;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<object[]> DiscoverCalled;

        private TextBox txtName;
        private TextBox txtHost;
        private Label lblError;
        private Button btnConnect;

        private Button btnSingleplayer;
        private Button btnMultiplayer;
        private Button btnOptions;
        private Button btnQuit;
        private Button btnBack;

        private MainGame game;
        private SpriteBatch batch;

        public MainMenu(MainGame game)
            : base(game)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            batch = new SpriteBatch(game.GraphicsDevice);
            int wndMinW = game.GraphicsDevice.Viewport.Width >> 1, wndMinH = game.GraphicsDevice.Viewport.Height >> 1;
            SpriteFont font = game.vGraphics.fonts["MenuFont"];

            int txtWidth = 150, txtHeight = 25;
            txtName = new TextBox(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH - (txtHeight >> 1), txtWidth, txtHeight), font) { Text = "UserName" };
            txtHost = new TextBox(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + (txtHeight >> 1), txtWidth, txtHeight), font) { Text = Ips.Frank.ToString(), BackColor = Color.Transparent };
            lblError = new Label(game.GraphicsDevice, new Rectangle(wndMinW + (txtWidth >> 1), wndMinH - (txtHeight >> 1), txtWidth, txtHeight), font) { AutoSize = true, BackColor = Color.Transparent };
            btnConnect = new Button(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + ((txtHeight >> 1) * 16), txtWidth, txtHeight), font) { Text = "Connect" };

            btnSingleplayer = new Button(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + ((txtHeight >> 1) * 4), txtWidth, txtHeight), font) { Text = "Singleplayer" };
            btnMultiplayer = new Button(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + ((txtHeight >> 1) * 8), txtWidth, txtHeight), font) { Text = "Multiplayer" };
            btnOptions = new Button(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + ((txtHeight >> 1) * 12), txtWidth, txtHeight), font) { Text = "Options" };
            btnQuit = new Button(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + ((txtHeight >> 1) * 16), txtWidth, txtHeight), font) { Text = "Quit" };
            btnBack = new Button(game.GraphicsDevice, new Rectangle(0, wndMinH + ((txtHeight >> 1) * 16), txtWidth, txtHeight), font) { Text = "Back" };

            btnSingleplayer.LeftClick += (sender, args) => { menuState = MenuState.SINGLEPLAYER; };
            btnMultiplayer.LeftClick += (sender, args) => { menuState = MenuState.MULTIPLAYER; };
            btnOptions.LeftClick += (sender, args) => { menuState = MenuState.OPTIONS; };
            btnQuit.LeftClick += (sender, args) => { game.Exit(); };
            btnBack.LeftClick += (sender, args) => { menuState = MenuState.MAINMENU; };


            btnConnect.LeftClick += btnConnect_LeftClick;
            txtName.Click += (sender, args) => { txtName.Focused = true; txtHost.Focused = false; };
            txtHost.Click += (sender, args) => { txtName.Focused = false; txtHost.Focused = true; };

            menuState = new MenuState();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            if (game.IsActive)
            {
                switch (menuState)
                {
                    case MenuState.MAINMENU:
                        btnSingleplayer.Update(ms, dt);
                        btnMultiplayer.Update(ms, dt);
                        btnOptions.Update(ms, dt);
                        btnQuit.Update(ms, dt);
                        break;
                    case MenuState.SINGLEPLAYER:
                        txtName.Update(ms, ks, dt);
                        lblError.Update(ms);
                        btnConnect.Update(ms, dt);
                        break;
                    case MenuState.MULTIPLAYER:
                        txtName.Update(ms, ks, dt);
                        txtHost.Update(ms, ks, dt);
                        lblError.Update(ms);
                        btnConnect.Update(ms, dt);
                        break;
                    case MenuState.OPTIONS:
                        break;
                }
                if (menuState != MenuState.MAINMENU)
                {
                    btnBack.Update(ms, dt);
                }
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            batch.Begin();
            switch (menuState)
            {
                case MenuState.MAINMENU:
                    btnSingleplayer.Draw(batch);
                    btnMultiplayer.Draw(batch);
                    btnOptions.Draw(batch);
                    btnQuit.Draw(batch); ;
                    break;
                case MenuState.SINGLEPLAYER:
                    txtName.Draw(batch);
                    lblError.Draw(batch);
                    btnConnect.Draw(batch);
                    break;
                case MenuState.MULTIPLAYER:
                    txtName.Draw(batch);
                    txtHost.Draw(batch);
                    lblError.Draw(batch);
                    btnConnect.Draw(batch);
                    break;
                case MenuState.OPTIONS:
                    break;
            }
            if (menuState != MenuState.MAINMENU)
            {
                btnBack.Draw(batch);
            }

            batch.End();
        }

        public void SetError(string error)
        {
            lblError.Text = error;
        }

        private void btnConnect_LeftClick(GuiItem sender, MouseState state)
        {
            string host = txtHost.Text.Replace(" ", "");
            string name = txtName.Text;

            if (name.Length < 1 || name.Length > 16)
            {
                SetError("Name must be between 1 and 16 characters");
                return;
            }

            if (menuState == MenuState.SINGLEPLAYER || host.ToUpper() == "LOCALHOST")
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
    internal enum MenuState
    {
        MAINMENU,
        SINGLEPLAYER,
        MULTIPLAYER,
        OPTIONS
    }
}