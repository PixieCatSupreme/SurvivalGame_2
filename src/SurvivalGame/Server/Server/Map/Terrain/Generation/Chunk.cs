using System.Collections.Generic;
using Mentula.Utilities;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Resc = Mentula.Utilities.Resources.Res;

namespace Mentula.Server
{
    public class Chunk
    {
        public Tile[] Tiles;
        public IntVector2 ChunkPos;
        public float Rain;
        public List<NPC> Creatures;

        public Chunk(IntVector2 pos)
        {
            ChunkPos = pos;
            Creatures = new List<NPC>();
        }
    }
}