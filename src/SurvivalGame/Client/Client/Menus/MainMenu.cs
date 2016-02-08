using Mentula.GuiItems.Containers;
using Mentula.GuiItems.Items;
using Microsoft.Xna.Framework;

namespace Mentula.Client.Menus
{
    public sealed class MainMenu : Menu
    {
        private MainGame MGame { get { return (MainGame)Game; } }

        public MainMenu(MainGame game)
            : base(game)
        { }

        public override void Initialize()
        {
            font = MGame.vGraphics.fonts["MenuFont"];
            int txtW = 150, txtH = 25;
            int txtWM = txtW >> 1, txtHM = txtH >> 1;

            Button btnSingleplayer = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 4),
                Width: txtW,
                Height: txtH,
                Text: "Singleplayer");

            Button btnMultiplayer = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 8),
                Width: txtW,
                Height: txtH,
                Text: "Multiplayer");

            Button btnOptions = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 12),
                Width: txtW,
                Height: txtH,
                Text: "Options");

            Button btnQuit = AddButton(
                Position: new Vector2(ScreenWidthMiddle - txtWM, ScreenHeightMiddle + txtHM * 16),
                Width: txtW,
                Height: txtH,
                Text: "Quit");

            btnSingleplayer.LeftClick += (sender, args) => MGame.SetState(GameState.SingleplayerMenu);
            btnMultiplayer.LeftClick += (sender, args) => MGame.SetState(GameState.MultiplayerMenu);
            btnOptions.LeftClick += (sender, args) => MGame.SetState(GameState.Options);
            btnQuit.LeftClick += (sender, args) => MGame.Exit();

            base.Initialize();
        }
    }
}