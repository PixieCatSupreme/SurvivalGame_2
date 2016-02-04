﻿using Mentula.Content.MM;
using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Mentula.Content
{
    [MMEditable]
    public class Creature : Item, IEntity
    {
        public Vector2 Pos { get; set; }
        public IntVector2 ChunkPos { get; set; }
        public float Rotation { get; set; }

        public readonly Stats Stats;
        public readonly int TextureId;
        public readonly bool IsBio;

        private Dictionary<int, Item> equipment;
        private List<Item> inventory;

        public Tag[] Systems { get; private set; }
        private readonly Tag[] DefaultSystemsVal;

        internal Creature()
            : base(0, "The Unnameable", Cheats.Unobtanium, ulong.MaxValue)
        {
            Stats = new Stats(short.MaxValue);
            DefaultSystemsVal = CalcSystems();
            equipment = new Dictionary<int, Item>();
            inventory = new List<Item>();
        }

        public Creature(ulong id, string name, int tex, bool isBio, Stats stats, Item[] parts)
            : base(id, name, parts)
        {
            TextureId = tex;
            IsBio = isBio;
            Stats = stats;
            DefaultSystemsVal = CalcSystems();
            equipment = new Dictionary<int, Item>();
            inventory = new List<Item>();
        }

        public Tag[] CalcSystems()
        {
            Systems = GetAllTags();
            return Systems;
        }

        public bool CalcIsAlive()
        {
            return IsBio ? Systems.FirstIsFalse(v => v > 0, 0, 6, 7, 8, 9) : Systems.FirstIsFalse(0, v => v > 0);
        }

        public byte GetHealth()
        {
            byte health = byte.MaxValue;
            for (int i = 0; i < Systems.Length; i++)
            {
                byte h = (byte)(Systems[i].Value / DefaultSystemsVal[i].Value * byte.MaxValue);
                if (h < health)
                {
                    health = h;
                }
            }
            return health;
        }

        public unsafe void UpdatePos(IntVector2* chunk, Vector2* tile)
        {
            ChunkPos = *chunk;
            Pos = *tile;
        }

        public void FormatPos()
        {
            while (Pos.X < 0 | Pos.Y < 0 | Pos.X > Res.ChunkSize | Pos.Y > Res.ChunkSize)
            {
                if (Pos.X < 0)
                {
                    Pos = new Vector2(Pos.X + Res.ChunkSize, Pos.Y);
                    ChunkPos = new IntVector2(ChunkPos.X - 1, ChunkPos.Y);
                }
                else if (Pos.X > Res.ChunkSize)
                {
                    Pos = new Vector2(Pos.X - Res.ChunkSize, Pos.Y);
                    ChunkPos = new IntVector2(ChunkPos.X + 1, ChunkPos.Y);
                }

                if (Pos.Y < 0)
                {
                    Pos = new Vector2(Pos.X, Pos.Y + Res.ChunkSize);
                    ChunkPos = new IntVector2(ChunkPos.X, ChunkPos.Y - 1);
                }
                else if (Pos.Y > Res.ChunkSize)
                {
                    Pos = new Vector2(Pos.X, Pos.Y - Res.ChunkSize);
                    ChunkPos = new IntVector2(ChunkPos.X, ChunkPos.Y + 1);
                }
            }
        }

        public long CalcMaxStorage(R tags)
        {
            int storageId = tags.GetTagId("Items/Storage");

            long result = 0;
            foreach (KeyValuePair<int, Item> item in equipment)
            {
                Tag storage = item.Value.Tags.FirstOrDefault(t => t.Key == storageId);
                result += storage.Value;
            }

            return result;
        }

        public void EquipItem(R slots, int slot, Item item)
        {
            if (slots.ContainsKey(slot))
            {
                if (!equipment.ContainsKey(slot))
                {
                    equipment.Add(slot, item);
                }
            }
        }

        public Item GetEquipment(int slot)
        {
            if (equipment.ContainsKey(slot))
            {
                return equipment[slot];
            }

            return null;
        }

        public Item UnequipItem(int slot)
        {
            if (equipment.ContainsKey(slot))
            {
                Item item = equipment[slot];
                equipment.Remove(slot);
                return item;
            }

            return null;
        }
    }

    public interface IEntity
    {
        Vector2 Pos { get; set; }
        IntVector2 ChunkPos { get; set; }
        float Rotation { get; set; }

        unsafe void UpdatePos(IntVector2* chunk, Vector2* tile);
        void FormatPos();
    }
}
