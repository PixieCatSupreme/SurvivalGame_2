

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Resc = Mentula.Utilities.Resources.Res;

namespace Mentula.Utilities
{
    public class Chunk
    {
        public Tile[] Tiles;
        public IntVector2 ChunkPos;
        public float Rain;
        public List<NPC> NPCs;

        public Chunk(IntVector2 pos)
        {
            ChunkPos = pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetTotalPos(IntVector2 chunk, Vector2 tile)
        {
            return new Vector2(tile.X + chunk.X * Resc.ChunkSize, tile.Y + chunk.Y * Resc.ChunkSize);
        }

        public static unsafe void FormatPos(IntVector2* chunk, Vector2* tile)
        {
            while(-tile->X < 0 || -tile->Y < 0 || -tile->X > Resc.ChunkSize || -tile->Y > Resc.ChunkSize)
            {
                if (-tile->X < 0)
                {
                    tile->X -= Resc.ChunkSize;
                    chunk->X++;
                }
                else if (-tile->X > Resc.ChunkSize)
                {
                    tile->X += Resc.ChunkSize;
                    chunk->X--;
                }

                if (-tile->Y < 0)
                {
                    tile->Y -= Resc.ChunkSize;
                    chunk->Y++;
                }
                else if (-tile->Y > Resc.ChunkSize)
                {
                    tile->Y += Resc.ChunkSize;
                    chunk->Y--;
                }
            }
        }
    }
}