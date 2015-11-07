using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Actor
    {
        public const float MOVE_SPEED = 10;
        public const float DIFF = 0.8f;

        public Vector2 Pos;
        public IntVector2 ChunkPos;
        public float Rotation;

        public Actor()
        {
            Pos = Vector2.Zero;
            ChunkPos = IntVector2.Zero;
        }

        public Actor(Vector2 pos, IntVector2 chunkpos)
        {
            Pos = pos;
            ChunkPos = chunkpos;
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
                    Pos.X += Res.ChunkSize;
                    ChunkPos.X--;
                }
                else if (Pos.X > Res.ChunkSize)
                {
                    Pos.X -= Res.ChunkSize;
                    ChunkPos.X++;
                }

                if (Pos.Y < 0)
                {
                    Pos.Y += Res.ChunkSize;
                    ChunkPos.Y--;
                }
                else if (Pos.Y > Res.ChunkSize)
                {
                    Pos.Y -= Res.ChunkSize;
                    ChunkPos.Y++;
                }
            }
        }

        public static Vector2 FormatPos(Vector2 tilePos)
        {
            while (tilePos.X < 0 | tilePos.Y < 0 | tilePos.X > Res.ChunkSize | tilePos.Y > Res.ChunkSize)
            {
                if (tilePos.X < 0) tilePos.X += Res.ChunkSize;
                else if (tilePos.X > Res.ChunkSize) tilePos.X -= Res.ChunkSize;

                if (tilePos.Y < 0) tilePos.Y += Res.ChunkSize;
                else if (tilePos.Y > Res.ChunkSize) tilePos.Y -= Res.ChunkSize;
            }

            return tilePos;
        }
    }
}
