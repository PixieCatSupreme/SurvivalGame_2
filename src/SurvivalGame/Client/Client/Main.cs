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
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConfig = Lidgren.Network.NetPeerConfiguration;

namespace Client
{
    public class Main : Game
    {
        private NetClient client;
        private float timeDiff;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private IntVector2 currentChunk;
        private Vector2 possition;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            NPConfig config = new NPConfig(Res.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);
            client = new NetClient(config);
        }

        protected override void Initialize()
        {
            client.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
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