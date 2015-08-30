#define LOCAL
#if !LOCAL
#define JOËLL
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;
using NPConfig = Lidgren.Network.NetPeerConfiguration;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NIM = Lidgren.Network.NetIncomingMessage;
using NOM = Lidgren.Network.NetOutgoingMessage;
using Mentula.Utilities.Resources;

namespace Client
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