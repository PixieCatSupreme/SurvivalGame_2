//#define ZILTOID
using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;
using System.Collections.Generic;
using Mentula.Content;
using static Mentula.Utilities.Resources.Res;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        private IntVector2[] path;
        private int moveI;

        public NPC(Creature c)
            : base(c)
        {
            moveI = 0;
#if ZILTOID
            path = new IntVector2[4];
            path[0] = new IntVector2(-10, -10);
            path[1] = new IntVector2(0, -10);
            path[2] = new IntVector2(-10, 0);
            path[3] = new IntVector2(0, 0);
#endif
        }

        public void WalkPath(float deltaTime)
        {
            if (path != null)
            {
                float speed = deltaTime * 10;
                while (speed > 0 && moveI < path.Length)
                {
                    Vector2 pos = new Vector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
                    float dist = Vector2.Distance(pos, path[moveI]);
                    Vector2 rot = path[moveI] - pos;
                    Rotation = MathEX.VectorToRadians(rot);
                    if (dist < speed)
                    {
                        Pos += rot;
                        speed -= dist;
                        moveI++;
                    }
                    else
                    {
                        Pos += (rot / dist * speed);
                        speed = 0;
                    }
                }
                FormatPos();
            }
        }

        public void GeneratePath(Creature target, List<Chunk> chunks)
        {
            Vector2 pos = new Vector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
            Vector2 targetPos = new Vector2(target.Pos.X + target.ChunkPos.X * ChunkSize, target.Pos.Y + target.ChunkPos.Y * ChunkSize);
            bool inRange = true;
            bool diff = true;
            if (Vector2.Distance(pos, targetPos) > 64)
            {
                inRange = false;
            }
            if (path != null)
            {
                if (Vector2.Distance(path[path.Length - 1], targetPos) < 1)
                {
                    diff = false;
                }
            }
            if (inRange && diff)
            {
                int minX = Math.Min(ChunkPos.X, target.ChunkPos.X) - 1;
                int maxX = Math.Max(ChunkPos.X, target.ChunkPos.X) + 1;
                int minY = Math.Min(ChunkPos.Y, target.ChunkPos.Y) - 1;
                int maxY = Math.Max(ChunkPos.Y, target.ChunkPos.Y) + 1;
                List<Chunk> c = new List<Chunk>();
                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].ChunkPos.X>=minX&& chunks[i].ChunkPos.X <= maxX&& chunks[i].ChunkPos.Y >= minY && chunks[i].ChunkPos.Y <= maxY)
                    {
                        c.Add(chunks[i]);
                    }
                }
            }
        }

    }
}
