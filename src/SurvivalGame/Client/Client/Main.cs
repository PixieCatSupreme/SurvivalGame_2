#define LOCAL
#if !LOCAL
#define JOËLL
#endif

using Lidgren.Network;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConfig = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.Client
{
    public class Main : Game
    {
        private NetClient client;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

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

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if(state.IsKeyDown(Keys.C))
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
            if(state.IsKeyDown(Keys.D))
            {
                client.Disconnect("User Disconnect.");
            }

            base.Update(gameTime);

            NIM msg = null;
            while((msg = client.ReadMessage()) != null)
            {
                switch(msg.MessageType)
                {
                    case(NIMT.DiscoveryResponse):
                        NOM nom = client.CreateMessage(Environment.UserName);
                        client.Connect(msg.SenderEndPoint, nom);
                        break;
                }
            }
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