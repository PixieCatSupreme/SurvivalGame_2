using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Utilities;
using Mentula.Utilities.Net;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
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
        private ushort playerLength;
        private TimeSpan prevAttack;
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
                        NOM nom = client.CreateMessage(game.hero.Name);
                        client.Connect(msg.SenderEndPoint, nom);
                        break;
                    case (NIMT.StatusChanged):
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            game.mainMenu.SetError(msg.ReadString());
                            game.SetState(GameState.MainMenu);
                        }
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
                                msg.ReadNPCs(ref game.npcs);
                                game.vGraphics.UpdateChunks(ref game.chunks, ref game.npcs);
                                game.SetState(GameState.Game);
                                break;
                            case (NDT.ChunkRequest):
                                Chunk[] chunks = msg.ReadChunks();
                                NPC[] npcs = new NPC[0];
                                msg.ReadNPCs(ref npcs);
                                game.UpdateChunks(chunks, npcs);
                                break;
                            case (NDT.Update):
                                playerLength = msg.ReadNPCs(ref game.npcs);
                                msg.ReadNPCUpdate(ref game.npcs, playerLength);

                                game.vGraphics.UpdateChunks(ref game.chunks, ref game.npcs);
                                break;
                        }
                        break;
                }
            }

            MouseState mState = Mouse.GetState();
            if (mState.LeftButton == ButtonState.Pressed && (gameTime.TotalGameTime - prevAttack).Milliseconds > 500)
            {
                prevAttack = gameTime.TotalGameTime;
                NOM nom = client.CreateMessage();
                nom.Write((byte)NDT.Attack);
                client.SendMessage(nom, NetDeliveryMethod.Unreliable);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconect();
                Stop();
            }

            base.Dispose(disposing);
        }
    }

    internal static class NetExtensions
    {
        public static Chunk ReadChunk(this NetBuffer msg)
        {
            IntVector2 chunkPos = msg.ReadPoint();
            Chunk result = new Chunk(chunkPos);

            result.Tiles = msg.ReadTiles();

            return result;
        }

        public static Chunk[] ReadChunks(this NetBuffer msg)
        {
            ushort length = msg.ReadUInt16();
            Chunk[] result = new Chunk[length];

            for (int i = 0; i < length; i++) result[i] = msg.ReadChunk();

            return result;
        }

        public static ushort ReadNPCs(this NetBuffer msg, ref NPC[] npcs)
        {
            ushort length = msg.ReadUInt16();
            if (length > npcs.Length) Array.Resize(ref npcs, length);

            for (int i = 0; i < length; i++)
            {
                IntVector2 chunk = msg.ReadPoint();
                Vector2 tile = msg.ReadVector2();
                float rot = msg.ReadHalfPrecisionSingle();
                float health = msg.ReadHalfPrecisionSingle();
                string name = msg.ReadString();
                int textId = msg.ReadInt32();

                npcs[i] = new NPC(chunk, tile, rot, health, name) { TextureId = textId };
            }

            return length;
        }

        public static void ReadNPCUpdate(this NetBuffer msg, ref NPC[] npcs, int index)
        {
            ushort length = msg.ReadUInt16();

            if (index + length != npcs.Length) Array.Resize(ref npcs, length);

            for (int i = index; i < length; i++)
            {
                IntVector2 chunkPos = msg.ReadPoint();
                Vector2 tilePos = msg.ReadVector2();
                float rot = msg.ReadHalfPrecisionSingle();
                float healthPerc = msg.ReadHalfPrecisionSingle();

                if (npcs[i] != null) npcs[i].Update(chunkPos, tilePos, rot, healthPerc);
                else npcs[i] = new NPC(chunkPos, tilePos, rot, healthPerc, "Darude");
            }
        }

        private static Tile[] ReadTiles(this NetBuffer msg)
        {
            ushort length = msg.ReadUInt16();
            Tile[] result = new Tile[length];

            for (int i = 0; i < length; i++)
            {
                int texture = msg.ReadInt32();
                IntVector2 pos = msg.ReadPoint();
                result[i] = new Tile(texture, pos);
            }

            return result;
        }
    }
}