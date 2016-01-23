using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System.Linq;
using Mentula.Utilities.MathExtensions;

namespace Mentula.Content
{
    [MMEditable]
    public class Creature : Item, IEntity
    {
        [MMIgnore]
        public Vector2 Pos { get; set; }
        [MMIgnore]
        public IntVector2 ChunkPos { get; set; }
        [MMIgnore]
        public float Rotation { get; set; }

        public readonly string Name;
        public readonly Stats Stats;
        public readonly int TextureId;
        [MMOptional]
        public bool IsBio;
        [MMIgnore]
        public Tag[] Systems { private set; get; }
        [MMIgnore]
        public bool IsAlive { private set; get; }
        [MMIgnore]
        private Tag[] DefaultSystemsVal;

        internal Creature()
            : base(new byte[0])
        {
            Name = "The Unnameable";
            Stats = new Stats(short.MaxValue);
        }

        internal Creature(string n, Stats stats, int textId)
            :base (new byte[0]) // TODO: Change to propper key.
        {
            Pos = Vector2.Zero;
            ChunkPos = IntVector2.Zero;
            Name = n;
            Stats = stats;
            TextureId = textId;
        }

        public static Creature CreatePlayer(string name)
        {
            return new Creature(name, new Stats(6), 9997);
        }

        public Tag[] CalcSystems()
        {
            Systems = GetAllTags();
            return Systems;
        }

        public bool CalcIsAlive()
        {
            IsAlive = IsBio ? Systems.FirstIsFalse(v => v > 0, 0, 6, 7, 8, 9) : Systems.FirstIsFalse(0, v => v > 0);
            return IsAlive;
        }

        public byte GetHealth()
        {
            DefaultSystemsVal = GetAllTagsWithDurability();
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
