#define LOCAL
#if !LOCAL
#define JOËLL
#endif

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
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConfig = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.Client
{
    public class Main : Game
    {
        public const int HEIGHT = 600;
        public const int WIDTH = 800;

        private NetClient client;
        private float timeDiff;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private TextureCollection textures;
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
            chunks = new Chunk[0];
            textures = new TextureCollection(Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textures.LoadFromConfig("R/Textures");

            base.LoadContent();
        }

        protected unsafe override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.C))
            {
#if LOCAL
                client.DiscoverKnownPeer("localhost", Ips.PORT);
#endif
#if JOËLL
                client.DiscoverKnownPeer(Ips.EndJoëll);
#endif
#if !LOCAL && !JOËLL
                client.DiscoverKnownPeer(Ips.EndNico);
#endif
            }
            if (state.IsKeyDown(Keys.D))
            {
                client.Disconnect("User Disconnect.");
            }

            if (state.IsKeyDown(Keys.W)) possition.Y += 1;
            if (state.IsKeyDown(Keys.A)) possition.X += 1;
            if (state.IsKeyDown(Keys.S)) possition.Y -= 1;
            if (state.IsKeyDown(Keys.D)) possition.X -= 1;

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
                        }
                        break;
                }
            }

            if (client.ConnectionStatus == NetConnectionStatus.Connected && timeDiff >= 1f / 30)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)NDT.PlayerUpdate);
                fixed (IntVector2* cP = &currentChunk) nom.Write(cP);
                fixed (Vector2* tP = &possition) nom.Write(tP);
                client.SendMessage(nom, NetDeliveryMethod.ReliableOrdered);
                timeDiff = 0;
            }

            timeDiff += (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            cam.Update(Matrix3.ApplyTranslation(new Engine.Core.Vect2(possition.X, possition.Y)));
            IntVector2[] vertices;
            cam.Transform(ref chunks, out vertices);

            spriteBatch.Begin();
            int index = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                Chunk chunk = chunks[i];
                for(int j = 0; j < chunk.Tiles.Length; j++)
                {
                    Vector2 pos = vertices[index].ToVector2();
                    spriteBatch.Draw(textures[chunk.Tiles[j].Tex], pos, Color.White);
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
    }
}