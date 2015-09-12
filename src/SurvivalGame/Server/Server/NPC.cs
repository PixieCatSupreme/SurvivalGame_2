//#define YOU_SPIN_ME_RIGHT_ROUND_BABY

using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        private Vector2 targetArea;
        private double lastAttackTime;

        public NPC(string n, Stats s, int h, Vector2 pos, IntVector2 chunkpos)
            : base(n, s, h, pos, chunkpos)
        {
            targetArea = pos + chunkpos * Resc.ChunkSize;
            lastAttackTime = 0;
        }

        public void DoStuff(float deltaTime, ref Creature[] players, int Index)
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
            float dist;
            Vect2 v0 = new Vect2(Pos.X + ChunkPos.X * Resc.ChunkSize, Pos.Y + ChunkPos.Y * Resc.ChunkSize);
            Vect2 v1;
            int p=0;
            if (index > 0)
            {
                v1 = new Vect2(players[0].Pos.X + players[0].ChunkPos.X * Resc.ChunkSize, players[0].Pos.Y + players[0].ChunkPos.Y * Resc.ChunkSize);
                dist = Vect2.Distance(v0, v1);
                for (int i = 0; i < index; i++)
                {
                    Vect2 v = new Vect2(players[i].Pos.X + players[i].ChunkPos.X * Resc.ChunkSize, players[i].Pos.Y + players[i].ChunkPos.Y * Resc.ChunkSize);
                    float d = Vect2.Distance(v, v0);
                    if (d < dist)
                    {
                        dist = d;
                        v1 = v;
                        p=i;
                    }
                }
                Rotation = Vect2.Angle(v0, v1);
                if (dist < 1f)
                {
                    if (lastAttackTime+500<DateTime.Now.Millisecond)
                    {
                        Combat.OnAttackNPC(ref players[p], this, 0.7f, 1);
                        lastAttackTime = DateTime.Now.Millisecond;
                    }
                }
                else
                {
                    Vect2 m = Vect2.Normalize(v1-v0);
                    Pos += new Vector2(m.X*deltaTime,m.Y*deltaTime);
                }
            }

            return t;
        }

        private bool MoveToNearest()
        {
            bool t = false;
            return t;
        }

    }
}
