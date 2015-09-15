//#define YOU_SPIN_ME_RIGHT_ROUND_BABY

using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;
using System.Collections.Generic;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        private Vector2 targetArea;
        private double lastAttackTime;
        private IntVector2 startpos;
        private IntVector2 endpos;
        private IntVector2[] nodeArray;

        public NPC(string n, Stats s, int h, Vector2 pos, IntVector2 chunkpos)
            : base(n, s, h, pos, chunkpos)
        {
            targetArea = pos + chunkpos * Resc.ChunkSize;
            lastAttackTime = 0;
        }

        public void DoStuff(ref Chunk[] c, float deltaTime, ref Creature[] players, int Index)
        {
#if !YOU_SPIN_ME_RIGHT_ROUND_BABY
            Attack(deltaTime, ref players, Index);
#else
            Rotation += deltaTime * 3;
            if (Rotation>=Math.PI*2)
            {
                Rotation -= (float)(Math.PI * 2);
            }
            Pos += MathEX.RadiansToVector(Rotation) * deltaTime*5;
#endif

        }

        private bool Attack(float deltaTime, ref Creature[] players, int index)
        {
            bool t = false;


            if (index > 0)
            {
                int p = 0;
                Vect2 v0 = new Vect2(Pos.X + ChunkPos.X * Resc.ChunkSize, Pos.Y + ChunkPos.Y * Resc.ChunkSize);
                Vect2 v1 = new Vect2(players[0].Pos.X + players[0].ChunkPos.X * Resc.ChunkSize, players[0].Pos.Y + players[0].ChunkPos.Y * Resc.ChunkSize);
                float dist = Vect2.Distance(v0, v1);
                for (int i = 1; i < index; i++)
                {
                    Vect2 v = new Vect2(players[i].Pos.X + players[i].ChunkPos.X * Resc.ChunkSize, players[i].Pos.Y + players[i].ChunkPos.Y * Resc.ChunkSize);
                    float d = Vect2.Distance(v, v0);
                    if (d < dist)
                    {
                        dist = d;
                        v1 = v;
                        p = i;
                    }
                }
                Rotation = Vect2.Angle(v0, v1);
                if (dist < 1f)
                {
                    if (lastAttackTime + 50 < DateTime.Now.Millisecond)
                    {
                        Combat.OnAttackNPC(ref players[p], this, 0.7f, 1);
                        lastAttackTime = DateTime.Now.Millisecond;
                        t = true;
                    }
                }
            }

            return t;
        }

        private bool MoveToNearest(ref Chunk[] c, float delta, ref Creature[] players, int index)
        {
            bool t = false;
            if (index > 0)
            {
                int n = 0;
                Vect2 p0 = new Vect2(ChunkPos.X * Resc.ChunkSize + Pos.X, ChunkPos.Y * Resc.ChunkSize + Pos.Y);
                Vect2 p1 = new Vect2(players[0].ChunkPos.X * Resc.ChunkSize + players[0].Pos.X, players[0].ChunkPos.Y * Resc.ChunkSize + players[0].Pos.Y);
                float dist = Vect2.Distance(p0, p1);
                for (int i = 1; i < index; i++)
                {
                    Vect2 p2 = new Vect2(players[i].ChunkPos.X * Resc.ChunkSize + players[i].Pos.X, players[i].ChunkPos.Y * Resc.ChunkSize + players[i].Pos.Y);
                    float d = Vect2.Distance(p0, p2);
                    if (d < dist)
                    {
                        dist = d;
                        p1 = p2;
                        n = i;
                    }
                }
                IntVector2 sPos = new IntVector2(p0.X, p0.Y);
                IntVector2 ePos = new IntVector2(p1.X, p1.Y);
                getpath(ref sPos, ref ePos, ref dist, ref c);
                Vect2 gp = new Vect2(nodeArray[0].X, nodeArray[0].Y);
                Rotation = Vect2.Angle(p0, gp);
                Vect2 newpos = p0 - gp;
                newpos.Normalize();
                newpos *= delta;
                newpos += p0;
                if (Vect2.Distance(p0, newpos) < Vect2.Distance(p0, gp))
                {
                    p0 = newpos;
                }
                else
                {
                    p0 = gp;
                }
                ChunkPos = new IntVector2(p0.X / Resc.ChunkSize, p0.Y / Resc.ChunkSize);
                Pos = new Vector2(p0.X % Resc.ChunkSize, p0.Y % Resc.ChunkSize);
            }
            return t;
        }

        private void getpath(ref IntVector2 sPos, ref IntVector2 ePos, ref float dist, ref Chunk[] c)
        {

            if ((startpos != sPos || endpos != ePos) && dist < 20)
            {
                AStar.Node[] nr = new AStar.Node[Resc.ChunkSize * Resc.ChunkSize * 9];
                IntVector2 p = new IntVector2(c[0].ChunkPos.X * Resc.ChunkSize, c[0].ChunkPos.Y * Resc.ChunkSize);
                for (int i = 0; i < nr.Length; i++)
                {
                    IntVector2 pos = p + new IntVector2(i % (Resc.ChunkSize * 3), i / (Resc.ChunkSize * 3));
                    nr[i] = new AStar.Node(pos, 10, true);
                }
                for (int i = 0; i < c.Length; i++)
                {
                    for (int j = 0; j < c[i].Destructibles.Count; j++)
                    {
                        int ind = (int)((c[i].Destructibles[j].ChunkPos.X * Resc.ChunkSize - Pos.X + c[i].Destructibles[j].Pos.X) +
                            (c[i].Destructibles[j].ChunkPos.Y * Resc.ChunkSize - Pos.Y + c[i].Destructibles[j].Pos.Y) * Resc.ChunkSize);
                        nr[ind].wall = true;

                    }
                }
                AStar.Map d = new AStar.Map(Resc.ChunkSize * 3, sPos, ePos, nr);
                nodeArray = AStar.GetRoute8(d);
            }
        }

    }
}
