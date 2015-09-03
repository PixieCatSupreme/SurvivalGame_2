using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Utilities
{
    public class Actor
    {
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
    }
}
