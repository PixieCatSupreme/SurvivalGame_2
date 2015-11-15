using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Content;
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
            fixed (IntVector2* cP = &value.ChunkPos) msg.Write(cP);

            msg.Write((ushort)value.Tiles.Length);
            for (int i = 0; i < value.Tiles.Length; i++) msg.Write(value.Tiles[i].Tex);

            msg.Write((ushort)value.Destructibles.Count);
            for (int i = 0; i < value.Destructibles.Count; i++)
            {
                Destructible cur = value.Destructibles[i];

                msg.Write(cur.Id);
                fixed (Vector2* tP = &cur.Pos) msg.Write(tP);
                msg.WriteHalfPrecision(cur.GetHealth());
            }
        }

        public static unsafe void Write(this NetBuffer msg, NPC value)
        {
            fixed (IntVector2* cP = &value.creature.ChunkPos) msg.Write(cP);
            fixed (Vector2* tP = &value.creature.Pos) msg.Write(tP);
            msg.WriteHalfPrecision(value.creature.Rotation);
            msg.Write(value.creature.GetHealth());
            msg.Write(value.creature.Name);
            msg.Write(value.creature.TextureId);
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
                    msg.Write(cur.Value.GetHealth());
                    msg.Write(cur.Value.Name);
                    msg.Write(9997);
                }
            }
        }
    }
}