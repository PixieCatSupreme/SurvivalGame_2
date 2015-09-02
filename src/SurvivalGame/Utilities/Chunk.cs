using Mentula.Engine.Core;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

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
        public static Vect2 GetTotalPos(IntVector2 chunk, Vector2 tile)
        {
            return new Vect2(tile.X + chunk.X * global::Mentula.Utilities.Resources.Res.ChunkSize, tile.Y + chunk.Y * global::Mentula.Utilities.Resources.Res.ChunkSize);
        }
    }
}