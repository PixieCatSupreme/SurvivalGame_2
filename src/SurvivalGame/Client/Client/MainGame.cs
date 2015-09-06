using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Utilities;
using Mentula.Utilities.Net;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConfig = Lidgren.Network.NetPeerConfiguration;
using Vect2 = Mentula.Engine.Core.Vect2;

namespace Mentula.Client
{
    public class MainGame : Game
    {
        public const int HEIGHT = 1080;
        public const int WIDTH = 1920;
        public const int HOST = 0;

        internal VertexGraphics vGraphics;
        internal Actor hero;
        internal Chunk[] chunks;

        private ClientNetworking networking;

        public MainGame()
        {
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            vGraphics = new VertexGraphics(this)
                {
                    PreferredBackBufferHeight = HEIGHT,
                    PreferredBackBufferWidth = WIDTH,
                    SynchronizeWithVerticalRetrace = false
                };

            Components.Add(networking = new ClientNetworking(this));
        }

        protected override void Initialize()
        {
            hero = new Actor();
            chunks = new Chunk[0];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            vGraphics.Load(Content);
            base.LoadContent();
        }

        protected unsafe override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.OemPlus))
            {
                switch (HOST)
                {
                    case (0):
                        networking.LocalConnect();
                        break;
                    case (1):
                        networking.NetworkConnect(Ips.Joëll);
                        break;
                    case (2):
                        networking.NetworkConnect(Ips.Nico);
                        break;
                    case (4):
                        networking.NetworkConnect(Ips.Frank);
                        break;
                }
            }

            if (state.IsKeyDown(Keys.OemMinus))
            {
                networking.Disconect();
            }

            Vector2 move = new Vector2();
            if (state.IsKeyDown(Keys.W)) move.Y += 5f * delta;
            if (state.IsKeyDown(Keys.A)) move.X += 5f * delta;
            if (state.IsKeyDown(Keys.S)) move.Y -= 5f * delta;
            if (state.IsKeyDown(Keys.D)) move.X -= 5f * delta;
            if (state.IsKeyDown(Keys.Escape)) Exit();

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

            MouseState mState = Mouse.GetState();           /* We need to add 90 degrees (* PI / 180) because */
            Vect2 mousePos = new Vect2(mState.X, mState.Y); /* the angle will be returned facing right. */
            hero.Rotation = Vect2.Angle(vGraphics.Camera.Offset, mousePos) + 1.5707963f;

            vGraphics.Update(hero, delta);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            vGraphics.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            networking.Disconect();
            networking.Stop();
            base.OnExiting(sender, args);
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
    }
}