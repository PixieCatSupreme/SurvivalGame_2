using System.Collections.Generic;
using Mentula.Utilities;

namespace Mentula.Server
{
    public class Chunk
    {
        public Tile[] Tiles;
        public List<Destructible> Destructibles;
        public IntVector2 ChunkPos;
        public float Rain;

        public Chunk(IntVector2 pos)
        {
            Destructibles = new List<Destructible>();
            ChunkPos = pos;
        }
    }
}