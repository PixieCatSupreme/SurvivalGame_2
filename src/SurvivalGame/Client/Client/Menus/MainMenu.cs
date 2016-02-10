using Mentula.GuiItems.Containers;
using Mentula.GuiItems.Items;
using Microsoft.Xna.Framework;

namespace Mentula.Client.Menus
{
    public sealed class MainMenu : Menu
    {
        public static readonly Color ButtonBackColor = new Color(150, 150, 130, 150);
        private MainGame MGame { get { return (MainGame)Game; } }

        public MainMenu(MainGame game)
            : base(game)
        { }

        public override void Initialize()
        {
            font = MGame.vGraphics.fonts["MenuFont"];
            int txtW = 150, txtH = 25;
            int txtWM = txtW >> 1, txtHM = txtH >> 1;

            AddGuiItem(
                Name: "Background",
                BackgroundImage: MGame.vGraphics.textures[10000],
                Enabled: false);

            Button btnSingleplayer = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 4),
                Width: txtW,
                Height: txtH,
                Text: "Singleplayer",
                BackColor: ButtonBackColor);

            Button btnMultiplayer = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 8),
                Width: txtW,
                Height: txtH,
                Text: "Multiplayer",
                BackColor: ButtonBackColor);

            Button btnOptions = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 12),
                Width: txtW,
                Height: txtH,
                Text: "Options",
                BackColor: ButtonBackColor);

            Button btnQuit = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 16),
                Width: txtW,
                Height: txtH,
                Text: "Quit",
                BackColor: ButtonBackColor);

            btnSingleplayer.LeftClick += (sender, args) => MGame.SetState(GameState.SingleplayerMenu);
            btnMultiplayer.LeftClick += (sender, args) => MGame.SetState(GameState.MultiplayerMenu);
            btnOptions.LeftClick += (sender, args) => MGame.SetState(GameState.Options);
            btnQuit.LeftClick += (sender, args) => MGame.Exit();

            base.Initialize();
        }
    }
}