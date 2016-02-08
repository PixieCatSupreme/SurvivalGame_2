﻿using Mentula.GuiItems.Containers;
using Mentula.GuiItems.Core;
using Mentula.GuiItems.Items;
using Mentula.Utilities.MathExtensions;
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
            const int A = 200;

            font = MGame.vGraphics.fonts["MenuFont"];
            int guiH = ScreenHeight >> 2;
            int y = ScreenHeight - guiH;

            GuiItem inv = AddGuiItem(
                Name: "Inventory",
                Position: new Vector2(0, y),
                Width: ScreenWidth / 3,
                Height: guiH,
                BackColor: Color.Gray.ApplyAlpha(200),
                Enabled: false);

            TabContainer tbC = AddTabContainer(
                Name: "Skills",
                Position: new Vector2(inv.Width, y),
                TabRectangle: new Rectangle(0, 0, ScreenWidth / 4, guiH));

            tbC.AddTab("Skills", Color.LightGray.ApplyAlpha(A));
            tbC.AddTab("Crafting", Color.DarkSlateGray.ApplyAlpha(A));
            tbC.AddTab("Smithing", Color.SlateGray.ApplyAlpha(A));

            GuiItem health = AddGuiItem(
                Name: "Health",
                Position: new Vector2(tbC.Position.X + tbC.TabRectangle.Width, y),
                Width: ScreenWidth / 5,
                Height: guiH,
                BackColor: Color.CadetBlue.ApplyAlpha(A),
                Enabled: false);

            TextBox input = AddTextBox(
                Name: "Input",
                Position: new Vector2(health.Position.X + health.Width, ScreenHeight - 25),
                Width: ScreenWidth - (int)(health.Position.X + health.Width),
                Height: 25,
                Multiline: true,
                BackColor: Color.WhiteSmoke.ApplyAlpha(A));

            Label chat = AddLabel(
                Name: "Chat",
                Position: new Vector2(health.Position.X + health.Width, y),
                Width: ScreenWidth - (int)(health.Position.X + health.Width),
                Height: guiH - 25,
                BackColor: Color.Thistle.ApplyAlpha(A));

            input.TextChanged += (sender, args) =>
            {
                if (args.Length > 0 && args[args.Length - 1] == '\n')
                {
                    if (args.Length > 1) chat.Text += input.Text;
                    input.Text = string.Empty;
                    input.Focused = false;
                }
            };

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TextBox input = FindControl<TextBox>("Input");
            if (input.Focused) MGame.AllowInput = false;
            else MGame.AllowInput = true;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            MGame.vGraphics.DrawMouse();
        }
    }
}
