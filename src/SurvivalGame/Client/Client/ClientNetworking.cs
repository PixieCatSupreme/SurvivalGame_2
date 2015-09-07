using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Utilities;
using Mentula.Utilities.Net;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Net;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConfig = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.Client
{
    public class ClientNetworking : GameComponent
    {
        public NetPeerStatus Status { get { return client.Status; } }
        public NetConnectionStatus NetStatus { get { return client.ConnectionStatus; } }

        private float timeDiff;
        private MainGame game;
        private NetClient client;
        /* 1 -> Discovery send.     0x1
         * 2 -> Restart required    0x2
         * 4 -> ...                 0x4
         * 8 -> ...                 0x8
         * 16 -> ...                0x10
         * 32 -> ...                0x20
         * 64 -> ...                0x40
         * 128 -> ...               0x80   */
        private byte state;

        public ClientNetworking(MainGame game)
            : base(game)
        {
            this.game = game;

            NPConfig config = new NPConfig(Res.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);
            client = new NetClient(config);
        }

        public override void Initialize()
        {
            client.Start();
            base.Initialize();
        }

        public override unsafe void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                                game.hero.ChunkPos = msg.ReadPoint();
                                game.hero.Pos = msg.ReadVector2();
                                break;
                            case (NDT.InitialChunkRequest):
                                game.chunks = msg.ReadChunks();
                                game.vGraphics.UpdateChunks(ref game.chunks);
                                break;
                            case (NDT.ChunkRequest):
                                game.UpdateChunks(msg.ReadChunks());
                                break;
                            case (NDT.PlayerUpdate):
                                game.vGraphics.UpdatePlayers(msg.ReadPlayers());
                                break;
                        }
                        break;
                }
            }

            if (client.ConnectionStatus == NetConnectionStatus.Connected && timeDiff >= 1f / 30)
            {
                IntVector2 netChunk = -game.hero.ChunkPos;
                Vector2 netTile = -game.hero.Pos;

                NOM nom = client.CreateMessage();
                nom.Write((byte)NDT.HeroUpdate);
                nom.Write(&netChunk);
                nom.Write(&netTile);
                nom.WriteHalfPrecision(game.hero.Rotation);
                client.SendMessage(nom, NetDeliveryMethod.UnreliableSequenced);
                timeDiff = 0;
            }

            timeDiff += delta;
            base.Update(gameTime);
        }

        public void LocalConnect()
        {
            if ((state & 0x1) == 0)
            {
                client.DiscoverKnownPeer("localhost", Ips.PORT);
                state |= 0x1;
            }
        }

        public void NetworkConnect(IPAddress address)
        {
            if ((state & 0x1) == 0)
            {
                IPEndPoint end = new IPEndPoint(address, Ips.PORT);
                client.DiscoverKnownPeer(end);
                state |= 0x1;
            }
        }

        public void Disconect()
        {
            client.Disconnect("Normal");
            if ((state & 0x1) != 0) state ^= 0x1;
        }

        public void Stop()
        {
            if ((state & 0x1) == 0)
            {
                client.Shutdown("Normal");
                state ^= 0x1;
            }
            else throw new InvalidOperationException("Disconect must be called before calling stop.");
        }

        public void Kill()
        {
            client.Shutdown("Forced");
            state |= 0x2;
        }

        public void Restart()
        {
            if ((state & 0x2) == 0)
            {
                client.Start();
                state ^= 0x2;
            }
            else throw new InvalidOperationException("Stop or kill must be called before calling restart.");
        }
    }
}