using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Utilities.Net
{
    public static class NetExtensions
    {
        public static IntVector2 ReadPoint(this NetBuffer msg)
        {
            return new IntVector2(msg.ReadInt32(), msg.ReadInt32());
        }

        public static unsafe void Write(this NetBuffer msg, IntVector2* point)
        {
            msg.Write(point->X);
            msg.Write(point->Y);
        }

        public static Chunk ReadChunk(this NetBuffer msg)
        {
            IntVector2 chunkPos = msg.ReadPoint();
            Chunk result = new Chunk(chunkPos);

            ushort length = msg.ReadUInt16();
            result.Tiles = new Tile[length];

            for (int i = 0; i < length; i++)
            {
                int texture = msg.ReadInt32();
                IntVector2 pos = msg.ReadPoint();
                result.Tiles[i] = new Tile(texture, pos);
            }

            return result;
        }

        public static void Write(this NetBuffer msg, Chunk value)
        {
            msg.Write(value.ChunkPos.X);
            msg.Write(value.ChunkPos.Y);

            msg.Write((ushort)value.Tiles.Length);

            for (int i = 0; i < value.Tiles.Length; i++)
            {
                Tile cur = value.Tiles[i];

                msg.Write(cur.Tex);
                msg.Write(cur.Pos.X);
                msg.Write(cur.Pos.Y);
            }
        }

        public static Chunk[] ReadChunks(this NetBuffer msg)
        {
            ushort length = msg.ReadUInt16();
            Chunk[] result = new Chunk[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = msg.ReadChunk();
            }

            return result;
        }

        public static void Write(this NetBuffer msg, Chunk[] value)
        {
            msg.Write((ushort)value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                msg.Write(value[i]);
            }
        }

        public static Player[] ReadPlayers(this NetBuffer msg)
        {
            int length = msg.ReadUInt16();
            Player[] result = new Player[length];

            for (int i = 0; i < length; i++)
            {
                IntVector2 chunk = msg.ReadPoint();
                Vector2 tile = msg.ReadVector2();
                float rot = msg.ReadHalfPrecisionSingle();
                float health = msg.ReadHalfPrecisionSingle();
                string name = msg.ReadString();

                result[i] = new Player(chunk, tile, rot, health, name);
            }

            return result;
        }

        public static unsafe void Write(this NetBuffer msg, ref KeyValuePair<long, Creature>[] players, int length)
        {
            if (length > 0)
            {
                msg.Write((ushort)length);

                for (int i = 0; i < length; i++)
                {
                    KeyValuePair<long, Creature> cur = players[i];

                    fixed (IntVector2* cP = &cur.Value.ChunkPos) msg.Write(cP);
                    fixed (Vector2* cT = &cur.Value.Pos) msg.Write(cT);
                    msg.WriteHalfPrecision(cur.Value.Rotation);
                    msg.WriteHalfPrecision(cur.Value.GetHealth());
                    msg.Write(cur.Value.Name);
                }
            }
        }
    }
}