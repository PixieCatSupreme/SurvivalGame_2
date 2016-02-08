using Mentula.GuiItems.Containers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Client.Menus
{
    public sealed class GuiMenu : Menu
    {
        private MainGame MGame { get { return (MainGame)Game; } }

        public GuiMenu(MainGame game)
            : base(game)
        { }

        public override void Initialize()
        {
            font = MGame.vGraphics.fonts["MenuFont"];
            int guiH = ScreenHeight >> 2;
            int y = ScreenHeight - guiH;

            AddGuiItem(
                Name: "Inventory",
                Position: new Vector2(0, y),
                Width: ScreenWidth / 3,
                Height: guiH,
                BackColor: Color.Gray);

            TabContainer tbC = AddTabContainer(
                Name: "Skills",
                Position: new Vector2(ScreenWidth / 3, y), 
                TabRectangle: new Rectangle(0,0,ScreenWidth / 3, guiH));

            tbC.AddTab("SKills", Color.LightGray);
            tbC.AddTab("Crafting", Color.DimGray);
            tbC.AddTab("Smithing", Color.SlateGray);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            MGame.vGraphics.DrawMouse();
        }
    }
}
