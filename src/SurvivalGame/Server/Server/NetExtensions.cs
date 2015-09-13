using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Utilities;
using Mentula.Utilities.Net;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Mentula.Server
{
    internal static class NetExtensions
    {
        public static unsafe void Write(this NetBuffer msg, Chunk value)
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

        public static unsafe void Write(this NetBuffer msg, NPC value)
        {
            fixed (IntVector2* cP = &value.ChunkPos) msg.Write(cP);
            fixed (Vector2* tP = &value.Pos) msg.Write(tP);
            msg.WriteHalfPrecision(value.Rotation);
            msg.WriteHalfPrecision(value.GetHealth());
            msg.Write(value.Name);
            msg.Write(value.TextureId);
        }

        public static void Write(this NetBuffer msg, Chunk[] value)
        {
            msg.Write((ushort)value.Length);

            for (int i = 0; i < value.Length; i++) msg.Write(value[i]);
        }

        public static void Write(this NetBuffer msg, NPC[] value)
        {
            msg.Write((ushort)value.Length);

            for (int i = 0; i < value.Length; i++) msg.Write(value[i]);
        }

        public static unsafe void Write(this NetBuffer msg, ref KeyValuePair<long, Creature>[] players, int length, long id)
        {
            msg.Write((ushort)(length - 1));

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    KeyValuePair<long, Creature> cur = players[i];

                    if (cur.Key == id) continue;

                    fixed (IntVector2* cP = &cur.Value.ChunkPos) msg.Write(cP);
                    fixed (Vector2* cT = &cur.Value.Pos) msg.Write(cT);
                    msg.WriteHalfPrecision(cur.Value.Rotation);
                    msg.WriteHalfPrecision(cur.Value.GetHealth());
                    msg.Write(cur.Value.Name);
                    msg.Write(9997);
                }
            }
        }
    }
}