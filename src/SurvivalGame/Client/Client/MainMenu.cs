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

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<object[]> DiscoverCalled;

        private TextBox txtName;
        private TextBox txtHost;
        private Label lblError;
        private Button btnConnect;

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
            txtName = new TextBox(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH - (txtHeight >> 1), txtWidth, txtHeight), font) { Text = "UserName", Focused = true };
            txtHost = new TextBox(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + (txtHeight >> 1), txtWidth, txtHeight), font) { Text = Ips.Frank.ToString(), BackColor = Color.Transparent };
            lblError = new Label(game.GraphicsDevice, new Rectangle(wndMinW + (txtWidth >> 1), wndMinH - (txtHeight >> 1), txtWidth, txtHeight), font) { AutoSize = true, BackColor = Color.Transparent };
            btnConnect = new Button(game.GraphicsDevice, new Rectangle(wndMinW - (txtWidth >> 1), wndMinH + ((txtHeight >> 1) * 3), txtWidth, txtHeight), font) { Text = "Connect" };

            btnConnect.LeftClick += btnConnect_LeftClick;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            txtName.Update(ms, ks, dt);
            txtHost.Update(ms, ks, dt);
            lblError.Update(ms);
            btnConnect.Update(ms, dt);

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            batch.Begin();
            txtName.Draw(batch);
            txtHost.Draw(batch);
            lblError.Draw(batch);
            btnConnect.Draw(batch);
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