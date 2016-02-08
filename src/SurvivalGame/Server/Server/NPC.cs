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
                float speed = deltaTime;
                while (speed > 0 && moveI < path.Length)
                {
                    Vector2 pos = new Vector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
                    float dist = Vector2.Distance(pos, path[moveI]);
                    Vector2 rot = new Vector2(path[moveI].X - pos.X, path[moveI].Y - pos.Y);
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
            int csize = Resc.ChunkSize;
            int cRange = 1;
            int tRange = 5;

            List<Chunk> c = new List<Chunk>();
            int xmin = Math.Min(target.ChunkPos.X, ChunkPos.X) - cRange;
            int xmax = Math.Max(target.ChunkPos.X, ChunkPos.X) + cRange;
            int ymin = Math.Min(target.ChunkPos.Y, ChunkPos.Y) - cRange;
            int ymax = Math.Max(target.ChunkPos.Y, ChunkPos.Y) + cRange;

            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].ChunkPos.X <= xmax && chunks[i].ChunkPos.X >= xmin && chunks[i].ChunkPos.Y <= ymax && chunks[i].ChunkPos.Y >= ymin)
                {
                    c.Add(chunks[i]);
                }
            }

            IntVector2 startpos = new IntVector2(Pos.X, Pos.Y);
            IntVector2 endpos = new IntVector2(target.Pos.X, target.Pos.Y);
            AStar.Map m = new AStar.Map(startpos, endpos);

            xmin = xmin * csize - tRange;
            xmax = xmax * csize + tRange;
            ymin = ymin * csize - tRange;
            ymax = ymax * csize + tRange;

            for (int i = 0; i < c.Count; i++)
            {
                for (int j = 0; j < c[i].Tiles.Length; j++)
                {
                    int xt = c[i].ChunkPos.X * csize + c[i].Tiles[j].Pos.X;
                    int yt = c[i].ChunkPos.Y * csize + c[i].Tiles[j].Pos.Y;
                    if (xt > xmin && xt < xmax && yt > ymin && yt < ymax)
                    {
                        IntVector2 n = new IntVector2(xt, yt);
                        bool ispatheable = true;
                        for (int k = 0; k < c[i].Destructibles.Count; k++)
                        {
                            int xd = (int)c[i].Destructibles[k].Pos.X + c[i].Destructibles[k].ChunkPos.X * csize;
                            int yd = (int)c[i].Destructibles[k].Pos.Y + c[i].Destructibles[k].ChunkPos.Y * csize;
                            if (xt == xd && yt == yd)
                            {
                                ispatheable = false;
                            }
                        }
                        m.AddNode(n, 10, ispatheable);
                    }
                }
            }
            path = AStar.Route4(ref m);
        }


    }
}
