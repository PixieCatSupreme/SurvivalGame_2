using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;
using Vect2 = Mentula.Engine.Core.Vect2;
using MEx = Mentula.Utilities.MathExtensions.MathEX;
using Mentula.Content;

namespace Mentula.Client
{
    public class MainGame : Game
    {
        internal GameState gameState;
        internal NPC hero;
        internal NPC[] npcs;
        internal Chunk[] chunks;

        internal MediumGraphics vGraphics;
        internal MainMenu mainMenu;
        internal ClientNetworking networking;

        public MainGame()
        {
            Content.RootDirectory = "Content";
            
            Components.Add(vGraphics = new MediumGraphics(this));
            Components.Add(mainMenu = new MainMenu(this));
            Components.Add(networking = new ClientNetworking(this));
            SetState(GameState.MainMenu);
        }

        protected override void Initialize()
        {
            hero = new NPC();
            chunks = new Chunk[0];
            npcs = new NPC[0];
            mainMenu.DiscoverCalled += OnConnect;
            base.Initialize();
        }

        protected unsafe override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                if (gameState == GameState.Game)
                {
                    float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    KeyboardState state = Keyboard.GetState();

                    Vector2 inp = new Vector2();
                    if (state.IsKeyDown(Keys.Escape)) Exit();
                    if (state.IsKeyDown(Keys.OemMinus)) vGraphics.SCALE *= 1 - 2f * delta;
                    if (state.IsKeyDown(Keys.OemPlus)) vGraphics.SCALE *= 1 + 2f * delta;
                    if (state.IsKeyDown(Keys.W)) inp.Y -= 1;
                    if (state.IsKeyDown(Keys.A)) inp.X -= 1;
                    if (state.IsKeyDown(Keys.S)) inp.Y += 1;
                    if (state.IsKeyDown(Keys.D)) inp.X += 1;
                    if (state.IsKeyDown(Keys.E)) networking.Disconect();

                    if (inp != Vector2.Zero)
                    {
                        inp = Vector2.Normalize(inp) * Actor.MOVE_SPEED * delta;
                        Vector2 outp = new Vector2();
                        hero.Pos -= inp;

                        fixed (IntVector2* cP = &hero.ChunkPos)
                        {
                            fixed (Vector2* tP = &hero.Pos)
                            {
                                Chunk.FormatPos(cP, tP);
                            }
                        }

                        IntVector2 chunkPos = -hero.ChunkPos;
                        Vector2 tilePos = -hero.Pos;

                        IntVector2 NW_CPos = chunkPos;
                        IntVector2 NE_CPos = tilePos.X + Actor.DIFF >= Res.ChunkSize ? chunkPos + IntVector2.UnitX : chunkPos;
                        IntVector2 SW_CPos = tilePos.Y + Actor.DIFF >= Res.ChunkSize ? chunkPos + IntVector2.UnitY : chunkPos;
                        IntVector2 SE_CPos = new IntVector2(NE_CPos.X, SW_CPos.Y);

                        IntVector2 NW_TPos = new IntVector2(tilePos);
                        IntVector2 NE_TPos = new IntVector2(Actor.FormatPos(tilePos + new Vector2(Actor.DIFF, 0)));
                        IntVector2 SW_TPos = new IntVector2(Actor.FormatPos(tilePos + new Vector2(0, Actor.DIFF)));
                        IntVector2 SE_TPos = new IntVector2(Actor.FormatPos(tilePos + new Vector2(Actor.DIFF)));

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

                            fixed (IntVector2* cP = &hero.ChunkPos)
                            {
                                fixed (Vector2* tP = &hero.Pos)
                                {
                                    Chunk.FormatPos(cP, tP);
                                }
                            }
                        }
                    }

                    MouseState mState = Mouse.GetState();
                    Vect2 mousePos = new Vect2(mState.X, mState.Y);
                    hero.Rotation = Vect2.Angle(vGraphics.Camera.Offset, mousePos);
                }
            }
            base.Update(gameTime);
        }

        public void UpdateChunks(Chunk[] newChunks, NPC[] newNpcs)
        {
            int index = 0;

            for (int i = 0; i < chunks.Length && index < newChunks.Length; i++)
            {
                Chunk cur = chunks[i];

                if (Math.Abs(cur.ChunkPos.X + hero.ChunkPos.X) > Res.Range_C ||
                    Math.Abs(cur.ChunkPos.Y + hero.ChunkPos.Y) > Res.Range_C)
                {
                    chunks[i] = newChunks[index];
                    index++;
                }
            }

            index = 0;

            for (int i = 0; i < npcs.Length && index < newNpcs.Length; i++)
            {
                NPC cur = npcs[i];

                if (Math.Abs(cur.ChunkPos.X + hero.ChunkPos.X) > Res.Range_C ||
                    Math.Abs(cur.ChunkPos.Y + hero.ChunkPos.Y) > Res.Range_C)
                {
                    npcs[i] = newNpcs[index];
                    index++;
                }
            }

            vGraphics.UpdateChunks(ref chunks, ref npcs);
        }

        public void SetState(GameState newState)
        {
            switch (newState)
            {
                case (GameState.MainMenu):
                    IsMouseVisible = true;
                    vGraphics.Visible = false;
                    mainMenu.Enabled = true;
                    mainMenu.Visible = true;
                    break;
                case (GameState.Loading):
                    IsMouseVisible = false;
                    vGraphics.Visible = false;
                    mainMenu.Enabled = false;
                    mainMenu.Visible = false;
                    break;
                case (GameState.Game):
                    IsMouseVisible = false;
                    vGraphics.Visible = true;
                    mainMenu.Visible = false;
                    mainMenu.Enabled = false;
                    break;
            }

            gameState = newState;
        }

        protected void OnConnect(object sender, object[] args)
        {
            hero.Name = (string)args[0];
            if (args.Length > 1) networking.NetworkConnect((IPAddress)args[1]);
            else networking.LocalConnect();
            SetState(GameState.Loading);
        }
    }
}