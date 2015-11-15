using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Creature : IEntity
    {
        public Vector2 Pos { get; set; }
        public IntVector2 ChunkPos { get; set; }
        public float Rotation { get; set; }

        public readonly string Name;
        public readonly Stats Stats;
        public readonly int TextureId;
        public Item[] Parts;
        public bool isbio;
        public bool IsAlive { private set; get; }

        internal Creature()
        {
            Name = "The Unnameable";
            Stats = new Stats(short.MaxValue);
        }

        internal Creature(string n, Vector2 pos, IntVector2 chunkpos)
        {
            Pos = pos;
            ChunkPos = chunkpos;
            Name = n;
            Stats = new Stats(10);
        }

        internal Creature(string n, Stats s, Vector2 pos, IntVector2 chunkpos)
        {
            Pos = pos;
            ChunkPos = chunkpos;
            Name = n;
            Stats = s;
        }

        public static Creature CreatePlayer(string name)
        {
            return new Creature(name, Vector2.Zero, IntVector2.Zero);
        }

        public void CalcIsAlive()
        {
            IsAlive= true;
        }

        public byte GetHealth()
        {
            return 0;
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
