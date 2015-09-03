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
            int x = (int)(tile->X / Resc.ChunkSize);
            int y = (int)(tile->Y / Resc.ChunkSize);

            if (x != 0 || y != 0)
            {
                tile->X -= x * Resc.ChunkSize;
                tile->Y -= y * Resc.ChunkSize;

                chunk->X += x;
                chunk->Y += y;
            }
        }
    }
}