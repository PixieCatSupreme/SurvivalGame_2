using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net;
using Vect2 = Mentula.Engine.Core.Vect2;

namespace Mentula.Client
{
    public class MainGame : Game
    {
        internal GameState gameState;
        internal NPC hero;
        internal NPC[] players;
        internal Chunk[] chunks;

        internal VertexGraphics vGraphics;
        internal MainMenu mainMenu;
        internal ClientNetworking networking;

        public MainGame()
        {
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            Components.Add(vGraphics = new VertexGraphics(this));
            Components.Add(mainMenu = new MainMenu(this));
            Components.Add(networking = new ClientNetworking(this));
            SetState(GameState.MainMenu);
        }

        protected override void Initialize()
        {
            hero = new NPC();
            chunks = new Chunk[0];
            players = new NPC[0];
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

                    Vector2 move = new Vector2();
                    if (state.IsKeyDown(Keys.W)) move.Y += 5f * delta;
                    if (state.IsKeyDown(Keys.A)) move.X += 5f * delta;
                    if (state.IsKeyDown(Keys.S)) move.Y -= 5f * delta;
                    if (state.IsKeyDown(Keys.D)) move.X -= 5f * delta;
                    if (state.IsKeyDown(Keys.E)) networking.Disconect();
                    if (state.IsKeyDown(Keys.Escape)) Exit();
                    if (state.IsKeyDown(Keys.OemMinus)) vGraphics.SCALE *= 1 - 2f * delta;
                    if (state.IsKeyDown(Keys.OemPlus)) vGraphics.SCALE *= 1 + 2f * delta;

                    if (move != Vector2.Zero)
                    {
                        hero.Pos += move;

                        fixed (IntVector2* cP = &hero.ChunkPos)
                        {
                            fixed (Vector2* tP = &hero.Pos)
                            {
                                Chunk.FormatPos(cP, tP);
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

        public void UpdateChunks(Chunk[] newChunks)
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

            vGraphics.UpdateChunks(ref chunks);
        }

        public void SetState(GameState newState)
        {
            switch (newState)
            {
                case (GameState.MainMenu):
                    vGraphics.Visible = false;
                    mainMenu.Enabled = true;
                    mainMenu.Visible = true;
                    break;
                case (GameState.Loading):
                    vGraphics.Visible = false;
                    mainMenu.Visible = false;
                    mainMenu.Enabled = false;
                    break;
                case (GameState.Game):
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
            networking.NetworkConnect((IPAddress)args[1]);
            SetState(GameState.Loading);
        }
    }
}