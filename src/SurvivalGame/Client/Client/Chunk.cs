using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Client
{
    public class Chunk
    {
        public Tile[] Tiles;
        public IntVector2 ChunkPos;

        public Chunk(IntVector2 pos)
        {
            ChunkPos = pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetTotalPos(IntVector2 chunk, Vector2 tile)
        {
            return new Vector2(tile.X + chunk.X * Res.ChunkSize, tile.Y + chunk.Y * Res.ChunkSize);
        }

        public static unsafe void FormatPos(IntVector2* chunk, Vector2* tile)
        {
            while (-tile->X < 0 || -tile->Y < 0 || -tile->X > Res.ChunkSize || -tile->Y > Res.ChunkSize)
            {
                if (-tile->X < 0)
                {
                    tile->X -= Res.ChunkSize;
                    chunk->X++;
                }
                else if (-tile->X > Res.ChunkSize)
                {
                    tile->X += Res.ChunkSize;
                    chunk->X--;
                }

                if (-tile->Y < 0)
                {
                    tile->Y -= Res.ChunkSize;
                    chunk->Y++;
                }
                else if (-tile->Y > Res.ChunkSize)
                {
                    tile->Y += Res.ChunkSize;
                    chunk->Y--;
                }
            }
        }
    }
}