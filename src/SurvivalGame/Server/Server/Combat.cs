using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities;
using Mentula.Content;

namespace Mentula.Server
{
    public static class Combat
    {

        public static bool OnAttackPlayer(ref Creature[] players,ref List<NPC> NPCs, ref Creature attacker, float arc, float range, int index)
        {
            bool hitSomeone = false;
            Vector2 angle = MathEX.RadiansToVector(attacker.Rotation);
            for (int i = 0; i < index; i++)
            {
                float dgr = MathEX.VectorToRadians(new Vector2(players[i].Pos.X - attacker.Pos.X, players[i].Pos.Y - attacker.Pos.Y));
                float dist = Vector2.Distance(players[i].Pos, attacker.Pos);
                if (MathEX.DifferenceBetweenRadians(attacker.Rotation, dgr) < arc && dist < range)
                {
                    DoDamage(players[i], ref attacker);
                    hitSomeone = true;
                }
            }
            for (int i = 0; i < NPCs.Count; i++)
            {
                float dgr = MathEX.VectorToRadians(new Vector2(NPCs[i].creature.Pos.X - attacker.Pos.X, NPCs[i].creature.Pos.Y - attacker.Pos.Y));
                float dist = Vector2.Distance(NPCs[i].creature.Pos, attacker.Pos);
                if (MathEX.DifferenceBetweenRadians(attacker.Rotation, dgr) < arc && dist < range)
                {
                    DoDamage(NPCs[i].creature, ref attacker);
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
                DoDamage(defender, ref attacker);
                hitSomeone = true;
            }
            return hitSomeone;
        }

        private static void DoDamage(Creature defender, ref Creature attacker)
        {
            Random r = new Random();
            if (defender.Health > 0)
            {
                while (true)
                {
                    if (defender.Health > 0)
                    {
                        defender.Health = Math.Max(defender.Health - attacker.Stats.Strength, 0);
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
