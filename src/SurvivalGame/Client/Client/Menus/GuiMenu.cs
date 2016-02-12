using Mentula.Content;
using Mentula.GuiItems.Containers;
using Mentula.GuiItems.Core;
using Mentula.GuiItems.Items;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mentula.Client.Menus
{
    public sealed class GuiMenu : Menu
    {
        private MainGame MGame { get { return (MainGame)Game; } }
        private const string CORPSE_PREFIX = "dd_";
        private bool chatResized;

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
                BackColor: Color.Cornsilk.ApplyAlpha(A),
                ForeColor: Color.Crimson,
                BorderStyle: BorderStyle.FixedSingle);

            Label lblChat = AddLabel(
                Name: "Chat",
                Position: new Vector2(health.Position.X + health.Width, y),
                Width: ScreenWidth - (int)(health.Position.X + health.Width),
                Height: guiH - 25,
                BackColor: Color.Cornsilk.ApplyAlpha(A),
                ForeColor: Color.Crimson);

            Slider sldChat = AddSlider(
                Name: "Slider",
                Position: new Vector2(lblChat.Position.X + lblChat.Width, y),
                Width: lblChat.Height,
                Rotation: 1.57f,
                Enabled: false,
                Visible: false);

            input.TextChanged += (sender, args) =>
            {
                if (args.Length > 0 && args[args.Length - 1] == '\n')
                {
                    if (args.Length > 1)
                    {
                        AddChatLine("Local", MGame.hero.Name, input.Text);
                        MGame.networking.SendMsg("Local", input.Text);
                    }
                    input.Text = string.Empty;
                    input.Focused = false;
                }
            };

            input.Refresh();

            int chatMaxLines = (int)(lblChat.Height / font.MeasureString("a").Y);
            lblChat.TextChanged += (sender, args) =>
            {
                int lines = lblChat.GetLineCount() - 1;
                if (lines > chatMaxLines)
                {
                    if (!chatResized)
                    {
                        chatResized = true;
                        lblChat.Width -= sldChat.Height;
                        sldChat.Show();
                    }

                    int newSelect = lines - chatMaxLines;
                    sldChat.MaximumValue = newSelect;
                    if (lblChat.LineStart == newSelect - 1) sldChat.Value = newSelect;
                }
            };

            sldChat.ValueChanged += (sender, args) =>
            {
                lblChat.LineStart = args;
                if (args == sldChat.MaximumValue)
                {
                    sldChat.SlidBarDimentions = new Rectangle(
                        sldChat.Width - sldChat.SlidBarDimentions.Width,
                        sldChat.SlidBarDimentions.Y,
                        sldChat.SlidBarDimentions.Width,
                        sldChat.SlidBarDimentions.Height);
                    sldChat.Refresh();
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

            MouseState state = Mouse.GetState();
            bool down = state.RightButton == ButtonState.Pressed;
            int halfTile = (Res.TileSize >> 1);

            for (int i = 0, j = 0; i < controlls.Count && j < MGame.deads.Length; i++)
            {
                GuiItem cur = controlls[i];
                if (cur.Name.StartsWith(CORPSE_PREFIX))
                {
                    IEntity corpse = MGame.deads[j++];
                    cur.Position = MGame.vGraphics.Camera.Transform(ref corpse);

                    if (down)
                    {
                        float min = cur.Position.X - halfTile;
                        float max = cur.Position.X + halfTile;

                        if (state.X >= min && state.X <= max)
                        {
                            min = cur.Position.Y - halfTile;
                            max = cur.Position.Y + halfTile;

                            if (state.Y >= min && state.Y <= max) cur.Show();
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            MGame.vGraphics.DrawMouse();
        }

        public void AddChatLine(string zone, string sender, string msg)
        {
            string idStr = $"[{zone}][{sender}]: ";
            string fullMsg = $"{idStr}{msg}";
            int width = (int)font.MeasureString(fullMsg).X;

            Label chat = FindControl<Label>("Chat");
            if (width > chat.Width)
            {
                int idStrLen = (int)font.MeasureString(idStr).X;
                float charLen = font.MeasureString("a").X;
                int charCount = (int)((chat.Width - idStrLen) / charLen);

                int lines = (int)Math.Ceiling(msg.Length / (float)charCount);
                for (int i = 0; i < lines; i++)
                {
                    string line = idStr;
                    line += msg.Substring(i * charCount, i != lines - 1 ? charCount : msg.Length - i * charCount) + '\n';

                    chat.Text += line;
                }
            }
            else chat.Text += fullMsg;
        }

        public void CreateDeathDowns(Creature[] corpses)
        {
            for (int i = 0; i < controlls.Count; i++)
            {
                GuiItem cur = controlls[i];
                if (cur.Name.StartsWith(CORPSE_PREFIX))
                {
                    controlls.Remove(cur);
                    i--;
                }
            }

            for (int i = 0; i < corpses.Length; i++)
            {
                Creature cur = corpses[i];

                DropDown dd = AddDropDown(
                    Name: $"{CORPSE_PREFIX}{i}",
                    AutoSize: true,
                    Enabled: false,
                    Visible: false);

                for (int j = 0; j < cur.Parts.Length; j++)
                {
                    Item part = cur.Parts[j];

                    dd.AddOption("Loot", part.Name, $"({part.CalcVolume()})");
                }

                dd.IndexClick += (sender, index) =>
                {
                    sender.Hide();
                };

                dd.Refresh();
            }
        }
    }
}
