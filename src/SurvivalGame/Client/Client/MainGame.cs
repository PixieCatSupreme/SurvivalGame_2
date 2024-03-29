#define COLLISION

using Mentula.Client.Menus;
using Mentula.Content;
using Mentula.Utilities;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;
using MEx = Mentula.Utilities.MathExtensions.MathEX;
using Vect2 = Mentula.Engine.Core.Vect2;

namespace Mentula.Client
{
    public class MainGame : Game
    {
        public bool AllowInput;

        internal GameState gameState;
        internal Creature hero;
        internal Creature[] npcs;
        internal Creature[] deads;
        internal Chunk[] chunks;

        internal MediumGraphics vGraphics;
        internal ClientNetworking networking;

        internal MainMenu mainMenu;
        internal SingleplayerMenu singleMenu;
        internal MultiplayerMenu multiMenu;
        internal GuiMenu gui;

        public MainGame()
        {
            Content.RootDirectory = "Content";
            chunks = new Chunk[0];
            npcs = new Creature[0];
            deads = new Creature[0];

            Components.Add(vGraphics = new MediumGraphics(this));
        }

        protected override void Initialize()
        {
            Components.Add(mainMenu = new MainMenu(this));
            Components.Add(singleMenu = new SingleplayerMenu(this));
            Components.Add(multiMenu = new MultiplayerMenu(this));
            Components.Add(gui = new GuiMenu(this));
            Components.Add(networking = new ClientNetworking(this));

            SetState(GameState.MainMenu);
            singleMenu.DiscoverCalled += OnConnect;
            multiMenu.DiscoverCalled += OnConnect;

            base.Initialize();
        }

        protected unsafe override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                if (gameState == GameState.Game)
                {
                    float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    KeyboardState kState = Keyboard.GetState();
                    MouseState mState = Mouse.GetState();

                    Vector2 inp = new Vector2();
                    if (AllowInput)
                    {
                        if (kState.IsKeyDown(Keys.Escape)) Exit();
                        if (kState.IsKeyDown(Keys.OemMinus) || mState.ScrollWheelValue < 0) vGraphics.SCALE *= 1 - 2f * delta;
                        if (kState.IsKeyDown(Keys.OemPlus) || mState.ScrollWheelValue > 0) vGraphics.SCALE *= 1 + 2f * delta;
                        if (kState.IsKeyDown(Keys.W)) inp.Y -= 1;
                        if (kState.IsKeyDown(Keys.A)) inp.X -= 1;
                        if (kState.IsKeyDown(Keys.S)) inp.Y += 1;
                        if (kState.IsKeyDown(Keys.D)) inp.X += 1;
                        if (kState.IsKeyDown(Keys.E)) networking.Disconect();
                        if (kState.IsKeyDown(Keys.PrintScreen))
                        {
                            bool succeeded = vGraphics.TakeScreenshot();
                            gui.AddChatLine("Global", "Client", succeeded ? "Saved screenshot" : "Failed to save screenshot");
                        }
                    }

                    if (inp != Vector2.Zero)
                    {
                        inp = Vector2.Normalize(inp) * Res.MOVE_SPEED * delta;
                        Vector2 outp = new Vector2();
                        hero.Pos -= inp;
                        IntVector2 ct = hero.ChunkPos;
                        Vector2 tt = hero.Pos;
                        Chunk.FormatPos(ref ct, ref tt);
                        hero.ChunkPos = ct;
                        hero.Pos = tt;

#if COLLISION
                        IntVector2 chunkPos = -hero.ChunkPos;
                        Vector2 tilePos = -hero.Pos;

                        IntVector2 NW_CPos = chunkPos;
                        IntVector2 NE_CPos = tilePos.X + Res.DIFF >= Res.ChunkSize ? chunkPos + IntVector2.UnitX : chunkPos;
                        IntVector2 SW_CPos = tilePos.Y + Res.DIFF >= Res.ChunkSize ? chunkPos + IntVector2.UnitY : chunkPos;
                        IntVector2 SE_CPos = new IntVector2(NE_CPos.X, SW_CPos.Y);

                        IntVector2 NW_TPos = new IntVector2(tilePos);
                        IntVector2 NE_TPos = new IntVector2(MathEX.FormatPos(tilePos + new Vector2(Res.DIFF, 0)));
                        IntVector2 SW_TPos = new IntVector2(MathEX.FormatPos(tilePos + new Vector2(0, Res.DIFF)));
                        IntVector2 SE_TPos = new IntVector2(MathEX.FormatPos(tilePos + new Vector2(Res.DIFF)));

                        bool? NW_T = null;
                        bool? NE_T = null;
                        bool? SW_T = null;
                        bool? SE_T = null;

                        for (int i = 0; i < chunks.Length; i++)
                        {
                            Chunk cur = chunks[i];

                            for (int j = 0; j < cur.Destrucables.Length; j++)
                            {
                                Destructable d = cur.Destrucables[j];

                                if (!NW_T.HasValue && cur.ChunkPos == NW_CPos && d.Pos == NW_TPos) NW_T = d;
                                if (!NE_T.HasValue && cur.ChunkPos == NE_CPos && d.Pos == NE_TPos) NE_T = d;
                                if (!SW_T.HasValue && cur.ChunkPos == SW_CPos && d.Pos == SW_TPos) SW_T = d;
                                if (!SE_T.HasValue && cur.ChunkPos == SE_CPos && d.Pos == SE_TPos) SE_T = d;

                                if (NW_T.HasValue && NE_T.HasValue && SW_T.HasValue && SE_T.HasValue) break;
                            }
                        }

                        bool NW = NW_T.HasValue ? NW_T.Value : false;
                        bool NE = NE_T.HasValue ? NE_T.Value : false;
                        bool SW = SW_T.HasValue ? SW_T.Value : false;
                        bool SE = SE_T.HasValue ? SE_T.Value : false;

                        if ((NE || SE) && inp.X > 0 && inp.Y == 0) outp.X = -inp.X;        // Move right false
                        else if ((NW || SW) && inp.X < 0 && inp.Y == 0) outp.X = -inp.X;   // Move left false
                        else if ((SE || SW) && inp.Y > 0 && inp.X == 0) outp.Y = -inp.Y;   // Move down false
                        else if ((NE || NW) && inp.Y < 0 && inp.X == 0) outp.Y = -inp.Y;   // Move up false
                        else if (inp.X > 0 && inp.Y > 0)                                   // Move right, down
                        {
                            if (SE && !NE && !SW)
                            {
                                Vector2 dist = MEx.Abs(Chunk.GetTotalPos(chunkPos, tilePos) - (SE_CPos * Res.ChunkSize + SE_TPos));
                                if (dist.X > dist.Y) outp.X = -inp.X;
                                else if (dist.X < dist.Y) outp.Y = -inp.Y;
                                else outp = -inp;
                            }
                            else if ((SE || NE) && !SW) outp.X = -inp.X;                  // Move right false, down
                            else if ((SE || SW) && !NE) outp.Y = -inp.Y;                  // Move right, down false
                            else if (SE && SW && NE) outp = -inp;                         // Move right false, down false
                        }
                        else if (inp.X > 0 && inp.Y < 0)                                  // Move right, up
                        {
                            if (NE && !SE && !NW)
                            {
                                Vector2 dist = MEx.Abs(Chunk.GetTotalPos(chunkPos, tilePos) - (NE_CPos * Res.ChunkSize + NE_TPos));
                                if (dist.X > dist.Y) outp.X = -inp.X;
                                else if (dist.X < dist.Y) outp.Y = -inp.Y;
                                else outp = -inp;
                            }
                            else if ((NE || SE) && !NW) outp.X = -inp.X;                  // Move right false, up
                            else if ((NE || NW) && !SE) outp.Y = -inp.Y;                  // Move righ, up false
                            else if (NE && NW && SE) outp = -inp;                         // Move right false, up false
                        }
                        else if (inp.X < 0 && inp.Y > 0)                                  // Move left, down
                        {
                            if (SW && !NW && !SE)
                            {
                                Vector2 dist = MEx.Abs(Chunk.GetTotalPos(chunkPos, tilePos) - (SW_CPos * Res.ChunkSize + SW_TPos));
                                if (dist.X > dist.Y) outp.X = -inp.X;
                                else if (dist.X < dist.Y) outp.Y = -inp.Y;
                                else outp = -inp;
                            }
                            else if ((SW || NW) && !SE) outp.X = -inp.X;                  // Move left false, down
                            else if ((SE || SW) && !NW) outp.Y = -inp.Y;                  // Move left, down false
                            else if (SE && SW && NW) outp = -inp;                         // Move left false, down false
                        }
                        else if (inp.X < 0 && inp.Y < 0)                                  // Move left, up
                        {
                            if (NW && !SW && !NE)
                            {
                                Vector2 dist = MEx.Abs(Chunk.GetTotalPos(chunkPos, tilePos) - (NW_CPos * Res.ChunkSize + NW_TPos));
                                if (dist.X > dist.Y) outp.X = -inp.X;
                                else if (dist.X < dist.Y) outp.Y = -inp.Y;
                                else outp = -inp;
                            }
                            else if ((NW || SW) && !NE) outp.X = -inp.X;                  // Move left false, up
                            else if ((NE || NW) && !SW) outp.Y = -inp.Y;                  // Move left, up false
                            else if (NE && NW && SW) outp = -inp;                         // Move left false, up false
                        }

                        if (outp != new Vector2())
                        {
                            hero.Pos -= outp;

                            ct = hero.ChunkPos;
                            tt = hero.Pos;
                            Chunk.FormatPos(ref ct, ref tt);
                            hero.ChunkPos = ct;
                            hero.Pos = tt;
                        }
#endif
                    }

                    Vect2 mousePos = new Vect2(mState.X, mState.Y);
                    hero.Rotation = Vect2.Angle(vGraphics.Camera.Offset, mousePos);
                }
            }
            base.Update(gameTime);
        }

        public void UpdateChunks(Chunk[] newChunks, Creature[] newNpcs, Creature[] newDeads)
        {
            int index = 0;

            for (int i = 0; i < chunks.Length && index < newChunks.Length; i++)
            {
                Chunk cur = chunks[i];

                if (Math.Abs(cur.ChunkPos.X + hero.ChunkPos.X) > Res.Range_C ||
                    Math.Abs(cur.ChunkPos.Y + hero.ChunkPos.Y) > Res.Range_C)
                {
                    chunks[i] = newChunks[index++];
                }
            }

            index = 0;

            for (int i = 0; i < npcs.Length && index < newNpcs.Length; i++)
            {
                Creature cur = npcs[i];

                if (Math.Abs(cur.ChunkPos.X + hero.ChunkPos.X) > Res.Range_C ||
                    Math.Abs(cur.ChunkPos.Y + hero.ChunkPos.Y) > Res.Range_C)
                {
                    npcs[i] = newNpcs[index++];
                }
            }

            for (int i = 0; i < npcs.Length; i++)
            {
                Creature cur = npcs[i];
                if (cur.Durability == 0)
                {
                    index = npcs.Length - 1;
                    if (index != i) npcs[i--] = npcs[index];
                    Array.Resize(ref npcs, index);
                }
            }

            index = 0;
            bool refreshDd = false;

            for (int i = 0; i < deads.Length && index < newDeads.Length; i++)
            {
                Creature cur = deads[i];

                if (Math.Abs(cur.ChunkPos.X + hero.ChunkPos.X) > Res.Range_C ||
                    Math.Abs(cur.ChunkPos.Y + hero.ChunkPos.Y) > Res.Range_C)
                {
                    deads[i] = newDeads[index++];
                    refreshDd = true;
                }
            }

            if (index < newDeads.Length)
            {
                int i = deads.Length;
                Array.Resize(ref deads, deads.Length + (newDeads.Length - index));
                for (; index < newDeads.Length; i++)
                {
                    deads[i] = newDeads[index++];
                    refreshDd = true;
                }
            }

            if (refreshDd) gui.CreateDeathDowns(deads);
            vGraphics.UpdateChunks(ref chunks, ref npcs, ref deads);
        }

        public void SetState(GameState newState)
        {
            switch (newState)
            {
                case (GameState.MainMenu):
                    IsMouseVisible = true;
                    vGraphics.Hide();
                    mainMenu.Show();
                    singleMenu.Hide();
                    multiMenu.Hide();
                    gui.Hide();
                    break;
                case (GameState.SingleplayerMenu):
                    IsMouseVisible = true;
                    vGraphics.Hide();
                    mainMenu.Hide();
                    singleMenu.Show();
                    multiMenu.Hide();
                    gui.Hide();
                    break;
                case (GameState.MultiplayerMenu):
                    IsMouseVisible = true;
                    vGraphics.Hide();
                    mainMenu.Hide();
                    singleMenu.Hide();
                    multiMenu.Show();
                    gui.Hide();
                    break;
                case (GameState.Loading):
                    IsMouseVisible = false;
                    vGraphics.Hide();
                    mainMenu.Hide();
                    singleMenu.Hide();
                    multiMenu.Hide();
                    gui.Hide();
                    break;
                case (GameState.Game):
                    IsMouseVisible = false;
                    vGraphics.Show();
                    mainMenu.Hide();
                    singleMenu.Hide();
                    multiMenu.Hide();
                    gui.Show();
                    break;
            }

            gameState = newState;
        }

        protected void OnConnect(object sender, object[] args)
        {
            hero = new Creature(0, (string)args[0], 0, false, new Stats(), new Item[0]);
            if (args.Length > 1) networking.NetworkConnect((IPAddress)args[1]);
            else networking.LocalConnect();
            SetState(GameState.Loading);
        }
    }
}