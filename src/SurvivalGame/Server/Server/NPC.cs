//#define YOU_SPIN_ME_RIGHT_ROUND_BABY

using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;
using System.Collections.Generic;
using Mentula.Content;

namespace Mentula.Server
{
    public class NPC
    {
        public Creature creature;
        private IntVector2[] path;

        public void WalkPath(float deltaTime)
        {
            Vector2 pos = creature.Pos + creature.ChunkPos * Resc.ChunkSize;
            float movedist = 1 * deltaTime;

            while (movedist > 0)
            {
                float dist = 0;
                IntVector2 target = new IntVector2();
                bool foundpath = false;

                for (int i = 0; i < path.Length - 1; i++)
                {
                    dist = Vector2.Distance(pos, path[i + 1]);
                    float dist2 = Vector2.Distance(path[i], path[i + 1]);
                    if (dist <= dist2)
                    {
                        target = path[i + 1];
                        foundpath = true;
                        break;
                    }
                }

                if (foundpath)
                {
                    if (dist > movedist)
                    {
                        Vector2 mpos = target - pos;
                        mpos.Normalize();
                        mpos *= movedist;
                        creature.Pos += mpos;
                        movedist = 0;
                    }

                    else
                    {
                        Vector2 mpos = target - pos;
                        float mdist = mpos.Length();
                        creature.Pos += mpos;
                        movedist -= mdist;
                    }
                }

                else
                {
                    break;
                }
            }
        }

        public void GeneratePath(Creature target, List<Chunk> chunks)
        {
            int csize = Resc.ChunkSize;
            int cRange = 1;
            int tRange = 5;

            List<Chunk> c = new List<Chunk>();
            int xmin = Math.Min(target.ChunkPos.X, creature.ChunkPos.X) - cRange;
            int xmax = Math.Max(target.ChunkPos.X, creature.ChunkPos.X) + cRange;
            int ymin = Math.Min(target.ChunkPos.Y, creature.ChunkPos.Y) - cRange;
            int ymax = Math.Max(target.ChunkPos.Y, creature.ChunkPos.Y) + cRange;

            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].ChunkPos.X <= xmax && chunks[i].ChunkPos.X >= xmin && chunks[i].ChunkPos.Y <= ymax && chunks[i].ChunkPos.Y >= ymin)
                {
                    c.Add(chunks[i]);
                }
            }

            IntVector2 startpos = new IntVector2(creature.Pos.X, creature.Pos.Y);
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


        public static implicit operator NPC(Creature c)
        {
            return new NPC() { creature = c };
        }
    }
}
