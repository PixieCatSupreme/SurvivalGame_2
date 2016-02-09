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

        public static void Write(this NetBuffer msg, Tag tag)
        {
            msg.Write(tag.Key);
            msg.Write(tag.Value);
        }

        public static void Write(this NetBuffer msg, Tag[] tags)
        {
            msg.Write(tags.Length);
            for (int i = 0; i < tags.Length; i++)
            {
                msg.Write(tags[i]);
            }
        }

        public static void Write(this NetBuffer msg, Item item)
        {
            msg.Write(item.Name);
            msg.Write(item.Durability);
            msg.Write(item.Tags);

            msg.Write(item.Material == null);
            if (item.Material == null) msg.Write(item.Parts);
            else msg.Write(item.Volume);
        }

        public static void Write(this NetBuffer msg, Item[] items)
        {
            msg.Write(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                msg.Write(items[i]);
            }
        }

        public static void Write(this NetBuffer msg, Stats stats)
        {
            msg.Write(stats.Agility);
            msg.Write(stats.Endurance);
            msg.Write(stats.Intelect);
            msg.Write(stats.Perception);
            msg.Write(stats.Strength);
        }

        public static void Write(this NetBuffer msg, Creature value)
        {
            msg.Write(value.ChunkPos);
            msg.Write(value.Pos);
            msg.WriteHalfPrecision(value.Rotation);
            msg.Write(value.TextureId);
            msg.Write(value.Stats);
            msg.Write(value.Name);
            msg.Write(value.GetHealth());
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

                    msg.Write(cur.Value);
                }
            }
        }
    }
}