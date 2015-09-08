#define YOU_SPIN_ME_RIGHT_ROUND_BABY

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
            GetTarget(ref players, Index);
            if (!TryToAttack(ref players, Index))
            {
                MoveToTarget(deltaTime);
            }
#else
            Rotation += deltaTime * 3;
            if (Rotation>=Math.PI*2)
            {
                Rotation -= (float)(Math.PI * 2);
            }
            Pos += MathEX.RadiansToVector(Rotation) * deltaTime*5;
#endif

        }

        private bool TryToAttack(ref Creature[] players, int Index)
        {
            bool t = false;
            float d = 100;
            int a = 0;
            for (int i = 0; i < Index; i++)
            {
                float dist = Vector2.Distance(players[i].ChunkPos * Resc.ChunkSize + players[i].Pos, ChunkPos * Resc.ChunkSize + Pos);
                if (dist < d)
                {
                    d = dist;
                    a = i;
                }
            }
            if (d < 1)
            {
                t = true;
                Vect2 v0 = new Vect2(ChunkPos.X * Resc.ChunkSize + Pos.X, ChunkPos.Y * Resc.ChunkSize + Pos.Y);
                Vect2 v1 = new Vect2(players[a].ChunkPos.X * Resc.ChunkSize + players[a].Pos.X, players[a].ChunkPos.Y * Resc.ChunkSize + players[a].Pos.Y);
                this.Rotation = Vect2.Angle(v0, v1);
                if (lastAttackTime + 500 < System.DateTime.Now.Millisecond)
                {
                    lastAttackTime = System.DateTime.Now.Millisecond;
                    Combat.OnAttackNPC(ref players[a], this, 1, 1.1f);
                }
            }

            return t;
        }

        private bool GetTarget(ref Creature[] players, int Index)
        {
            bool t = false;
            for (int i = 0; i < Index; i++)
            {
                float d0 = Vector2.Distance(players[i].ChunkPos * Resc.ChunkSize + players[i].Pos, this.ChunkPos * Resc.ChunkSize + this.Pos);
                float d1 = Vector2.Distance(targetArea, this.ChunkPos * Resc.ChunkSize + this.Pos);
                if (d0 < d1 && d0 < 16)
                {
                    targetArea = players[i].ChunkPos * Resc.ChunkSize + players[i].Pos;
                    t = true;
                }
            }
            return t;
        }

        private bool MoveToTarget(float deltaTime)
        {
            bool t = false;
            Vect2 v0 = new Vect2(ChunkPos.X * Resc.ChunkSize + Pos.X, ChunkPos.Y * Resc.ChunkSize + Pos.Y);
            Vect2 v1 = new Vect2(targetArea.X, targetArea.Y);
            Rotation = Vect2.Angle(v0, v1);
            Vect2 m = Vect2.Normalize(v0 - v1) * deltaTime * 5;
            if (Vect2.Distance(v0, v1) > Vect2.Distance(v0, m))
            {
                Pos += new Vector2(m.X, m.Y);
            }
            else
            {
                Pos += (targetArea - ChunkPos * Resc.ChunkSize - Pos);
            }
            return t;
        }

    }
}
