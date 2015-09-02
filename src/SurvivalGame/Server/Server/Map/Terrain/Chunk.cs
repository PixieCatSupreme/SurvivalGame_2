using Mentula.Utilities;

namespace Mentula.Server
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
    }
}
