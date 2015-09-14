using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;

namespace Mentula.Utilities
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
            while (Pos.X < 0 | Pos.Y < 0 | Pos.X > Resc.ChunkSize | Pos.Y > Resc.ChunkSize)
            {
                if (Pos.X < 0)
                {
                    Pos.X += Resc.ChunkSize;
                    ChunkPos.X--;
                }
                else if (Pos.X > Resc.ChunkSize)
                {
                    Pos.X -= Resc.ChunkSize;
                    ChunkPos.X++;
                }

                if (Pos.Y < 0)
                {
                    Pos.Y += Resc.ChunkSize;
                    ChunkPos.Y--;
                }
                else if (Pos.Y > Resc.ChunkSize)
                {
                    Pos.Y -= Resc.ChunkSize;
                    ChunkPos.Y++;
                }
            }
        }

        public static Vector2 FormatPos(Vector2 tilePos)
        {
            while (tilePos.X < 0 | tilePos.Y < 0 | tilePos.X > Resc.ChunkSize | tilePos.Y > Resc.ChunkSize)
            {
                if (tilePos.X < 0) tilePos.X += Resc.ChunkSize;
                else if (tilePos.X > Resc.ChunkSize) tilePos.X -= Resc.ChunkSize;

                if (tilePos.Y < 0) tilePos.Y += Resc.ChunkSize;
                else if (tilePos.Y > Resc.ChunkSize) tilePos.Y -= Resc.ChunkSize;
            }

            return tilePos;
        }
    }
}
