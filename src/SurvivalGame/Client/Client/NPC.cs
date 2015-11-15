using Mentula.Content;
using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;

namespace Mentula.Client
{
    public class NPC : IEntity
    {
        public Vector2 Pos { get; set; }
        public IntVector2 ChunkPos { get; set; }
        public float Rotation { get; set; }
        public string Name;
        public int TextureId;
        public byte HealthPrec;

        public NPC()
        {
            Name = "The Unnameable";
            TextureId = 9997;
            HealthPrec = 100;
        }

        public NPC(IntVector2 chunk, Vector2 tile, float rotation, byte health, string name)
        {
            Pos = tile;
            ChunkPos = chunk;
            Rotation = rotation;
            HealthPrec = health;
            Name = name;
        }

        public void Update(IntVector2 chunk, Vector2 tile, float rot, byte health)
        {
            ChunkPos = chunk;
            Pos = tile;
            Rotation = rot;
            HealthPrec = health;
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
}