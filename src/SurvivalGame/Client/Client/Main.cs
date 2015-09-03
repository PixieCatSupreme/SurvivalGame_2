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

namespace Mentula.Client
{
    public class Main : Game
    {
        public const int HEIGHT = 1080;
        public const int WIDTH = 1920;
        public const int HOST = 0;

        private NetClient client;
        private float timeDiff;
        private FPS fpsCounter;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private TextureCollection textures;
        private SpriteFont debugFont;
        private Camera cam;

        private IntVector2 currentChunk;
        private Vector2 possition;

        private Chunk[] chunks;

        public Main()
        {
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;

            graphics = new GraphicsDeviceManager(this)
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
            cam = new Camera();
            fpsCounter = new FPS();
            chunks = new Chunk[0];
            textures = new TextureCollection(Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textures.LoadFromConfig("R/Textures");
            debugFont = Content.Load<SpriteFont>("Fonts/ConsoleFont");

            base.LoadContent();
        }

        protected unsafe override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.OemPlus))
            {
                if (HOST == 0) client.DiscoverKnownPeer("localhost", Ips.PORT);
                else if (HOST == 1) client.DiscoverKnownPeer(Ips.EndJoëll);
                else if (HOST == 2) client.DiscoverKnownPeer(Ips.EndNico);
                else if (HOST == 4) client.DiscoverKnownPeer(Ips.EndFrank);
            }
            if (state.IsKeyDown(Keys.OemMinus))
            {
                client.Disconnect("User Disconnect.");
            }

            Vector2 move = new Vector2();
            if (state.IsKeyDown(Keys.W)) move.Y += .1f;
            if (state.IsKeyDown(Keys.A)) move.X += .1f;
            if (state.IsKeyDown(Keys.S)) move.Y -= .1f;
            if (state.IsKeyDown(Keys.D)) move.X -= .1f;
            if (state.IsKeyDown(Keys.Escape)) Exit();

            if (move != Vector2.Zero)
            {
                possition += move;

                fixed (IntVector2* cP = &currentChunk)
                {
                    fixed (Vector2* tP = &possition)
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
                            case (NDT.PlayerUpdate):
                                currentChunk = msg.ReadPoint();
                                possition = msg.ReadVector2();
                                break;
                            case (NDT.InitialChunkRequest):
                                chunks = msg.ReadChunks();
                                break;
                            case (NDT.ChunkRequest):
                                UpdateChunks(msg.ReadChunks());
                                break;
                        }
                        break;
                }
            }

            if (client.ConnectionStatus == NetConnectionStatus.Connected && timeDiff >= 1f / 30)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)NDT.PlayerUpdate);
                nom.Write(-currentChunk.X);
                nom.Write(-currentChunk.Y);
                nom.Write(-possition.X);
                nom.Write(-possition.Y);
                client.SendMessage(nom, NetDeliveryMethod.UnreliableSequenced);
                timeDiff = 0;
            }

            timeDiff += (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            Vect2 pp = Chunk.GetTotalPos(currentChunk, possition);
            cam.Update(Matrix3.ApplyScale(2f) * Matrix3.ApplyTranslation(pp));
            IntVector2[] vertices;
            cam.Transform(ref chunks, out vertices);

            GraphicsDevice.Clear(Color.LimeGreen);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawString(debugFont, fpsCounter.Avarage.ToString(), Vector2.Zero, Color.Red,0,Vector2.Zero, 1, 0, 0);

            int index = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                Chunk chunk = chunks[i];
                for (int j = 0; j < chunk.Tiles.Length; j++)
                {
                    Vector2 pos = vertices[index].ToVector2();
                    spriteBatch.Draw(textures[chunk.Tiles[j].Tex], pos, null, Color.White, 0, Vector2.Zero, 2f, 0, 1);
                    index++;
                }
            }

            spriteBatch.End();

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

                if (Math.Abs(cur.ChunkPos.X + currentChunk.X) > Res.Range_C ||
                    Math.Abs(cur.ChunkPos.Y + currentChunk.Y) > Res.Range_C)
                {
                    chunks[i] = newChunks[index];
                    index++;
                }
            }
        }
    }
}