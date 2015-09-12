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
        public const int HEIGHT = 600;
        public const int WIDTH = 800;
        public const int HOST = 0;

        internal GameState gameState;
        internal string heroName;
        internal Actor hero;
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
            gameState = GameState.MainMenu;

            Components.Add(vGraphics = new VertexGraphics(this)
                {
                    PreferredBackBufferHeight = HEIGHT,
                    PreferredBackBufferWidth = WIDTH,
                    SynchronizeWithVerticalRetrace = false,
                });

            Components.Add(mainMenu = new MainMenu(this) { Visible = true });
            Components.Add(networking = new ClientNetworking(this));
        }

        protected override void Initialize()
        {
            hero = new Actor();
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

                    if (state.IsKeyDown(Keys.E)) networking.Disconect();

                    Vector2 move = new Vector2();
                    if (state.IsKeyDown(Keys.W)) move.Y += 5f * delta;
                    if (state.IsKeyDown(Keys.A)) move.X += 5f * delta;
                    if (state.IsKeyDown(Keys.S)) move.Y -= 5f * delta;
                    if (state.IsKeyDown(Keys.D)) move.X -= 5f * delta;
                    if (state.IsKeyDown(Keys.Escape)) Exit();

                    if (move != Vector2.Zero)

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

        protected override void OnExiting(object sender, EventArgs args)
        {
            networking.Disconect();
            networking.Stop();
            base.OnExiting(sender, args);
        }

        protected void OnConnect(object sender, object[] args)
        {
            heroName = (string)args[0];
            networking.NetworkConnect((IPAddress)args[1]);
            mainMenu.Visible = false;
            mainMenu.Enabled = false;
        }
    }
}