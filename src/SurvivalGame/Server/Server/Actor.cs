using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Server
{
    public class Actor
    {
        public Vector2 Pos;
        public IntVector2 ChunkPos;

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

    }
}
