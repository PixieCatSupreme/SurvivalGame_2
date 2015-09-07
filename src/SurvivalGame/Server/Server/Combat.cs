using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities;

namespace Mentula.Server
{
    public static class Combat
    {

        public static bool OnAttackPlayer(ref Creature[] creatures, ref Creature attacker, float arc, float range, int index)
        {
            bool hitSomeone = false;
            Vector2 angle = MathEX.RadiansToVector(attacker.Rotation);
            for (int i = 0; i < index; i++)
            {
                float dgr = MathEX.VectorToRadians(new Vector2(creatures[i].Pos.X - attacker.Pos.X, creatures[i].Pos.Y - attacker.Pos.Y));
                float dist = Vector2.Distance(creatures[i].Pos, attacker.Pos);
                if (MathEX.DifferenceBetweenRadians(attacker.Rotation, dgr) < arc && dist < range)
                {
                    DoDamage(ref creatures[i], ref attacker);
                    hitSomeone = true;
                }
            }
            return hitSomeone;
        }

        public static bool OnAttackNPC(ref Creature defender, Creature attacker, float arc, float range)
        {
            bool hitSomeone = false;
            Vector2 angle = MathEX.RadiansToVector(attacker.Rotation);
                float dgr = MathEX.VectorToRadians(new Vector2(defender.Pos.X - attacker.Pos.X, defender.Pos.Y - attacker.Pos.Y));
                float dist = Vector2.Distance(defender.Pos, attacker.Pos);
                if (MathEX.DifferenceBetweenRadians(attacker.Rotation, dgr) < arc && dist < range)
                {
                    DoDamage(ref defender, ref attacker);
                    hitSomeone = true;
                }
            return hitSomeone;
        }

        private static void DoDamage(ref Creature defender, ref Creature attacker)
        {
            Random r = new Random();
            if (defender.Health[0] > 0 && defender.Health[1] > 0)
            {
                while (true)
                {
                    int n = (int)(r.NextDouble() * defender.Health.Length);
                    if (defender.Health[n] > 0)
                    {
                        defender.Health[n] = Math.Max(defender.Health[n] - attacker.Stats.Strength, 0);
                        break;
                    }
                }
            }
            else
            {
                defender.IsAlive = false;
            }
        }

    }
}
