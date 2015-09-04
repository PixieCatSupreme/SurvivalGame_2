using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Utilities;
using Mentula.Utilities.Net;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Matrix3 = Mentula.Engine.Core.Matrix3;
using Vect2 = Mentula.Engine.Core.Vect2;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConfig = Lidgren.Network.NetPeerConfiguration;
using System.Collections.Generic;

namespace Mentula.Client
{
    public class Main : Game
    {
        public const int HEIGHT = 1080;
        public const int WIDTH = 1920;
        public const int HOST = 0;

        private NetClient client;
        private bool discSend;
        private float timeDiff;
        private VertexGraphics vGraphics;

        private Actor hero;
        private Chunk[] chunks;

        public Main()
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

            NPConfig config = new NPConfig(Res.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);
            client = new NetClient(config);
        }

        protected override void Initialize()
        {
            client.Start();
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

            if (state.IsKeyDown(Keys.OemPlus) && !discSend)
            {
                if (HOST == 0) client.DiscoverKnownPeer("localhost", Ips.PORT);
                else if (HOST == 1) client.DiscoverKnownPeer(Ips.EndJoëll);
                else if (HOST == 2) client.DiscoverKnownPeer(Ips.EndNico);
                else if (HOST == 4) client.DiscoverKnownPeer(Ips.EndFrank);
                discSend = true;
            }
            if (state.IsKeyDown(Keys.OemMinus))
            {
                client.Disconnect("User Disconnect.");
                discSend = false;
            }

            Vector2 move = new Vector2();
            if (state.IsKeyDown(Keys.W)) move.Y += 5f * delta;
            if (state.IsKeyDown(Keys.A)) move.X += 5f * delta;
            if (state.IsKeyDown(Keys.S)) move.Y -= 5f * delta;
            if (state.IsKeyDown(Keys.D)) move.X -= 5f * delta;
            if (state.IsKeyDown(Keys.Q)) hero.Rotation -= .1f * Res.DEG2RAD;
            if (state.IsKeyDown(Keys.E)) hero.Rotation += .1f * Res.DEG2RAD;
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

            NIM msg = null;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case (NIMT.DiscoveryResponse):
                        NOM nom = client.CreateMessage(Environment.UserName);
                        client.Connect(msg.SenderEndPoint, nom);
                        break;
                    case (NIMT.Data):
                        NDT type = (NDT)msg.ReadByte();

                        switch (type)
                        {
                            case (NDT.HeroUpdate):
                                hero.ChunkPos = msg.ReadPoint();
                                hero.Pos = msg.ReadVector2();
                                break;
                            case (NDT.InitialChunkRequest):
                                chunks = msg.ReadChunks();
                                vGraphics.UpdateChunks(ref chunks);
                                break;
                            case (NDT.ChunkRequest):
                                UpdateChunks(msg.ReadChunks());
                                break;
                            case (NDT.PlayerUpdate):
                                vGraphics.UpdatePlayers(msg.ReadPlayers());
                                break;
                        }
                        break;
                }
            }

            if (client.ConnectionStatus == NetConnectionStatus.Connected && timeDiff >= 1f / 30)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)NDT.HeroUpdate);
                nom.Write(-hero.ChunkPos.X);
                nom.Write(-hero.ChunkPos.Y);
                nom.Write(-hero.Pos.X);
                nom.Write(-hero.Pos.Y);
                client.SendMessage(nom, NetDeliveryMethod.UnreliableSequenced);
                timeDiff = 0;
            }

            timeDiff += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            client.Disconnect("Forced Disconnect");
            client.Shutdown("Bye");
            base.OnExiting(sender, args);
        }

        private void UpdateChunks(Chunk[] newChunks)
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