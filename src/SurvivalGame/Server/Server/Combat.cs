using Mentula.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;

namespace Mentula.Server
{
    public static class Combat
    {
        public static void OnMelee(Creature attacker,ref List<NPC> defender)
        {
            float range = 2;//todo generate range
            float arc = 30;
            Vector2 attackerPos = attacker.Pos + attacker.ChunkPos * ChunkSize;
            float attackerRot = attacker.Rotation / (float)Math.PI * 180;
            Vector2 rot = MathEX.DegreesToVector(attackerRot);
            rot /= 2;

            for (int i = 0; i < defender.Count; i++)
            {
                Vector2 defenderPos = defender[i].creature.Pos + defender[i].creature.ChunkPos * ChunkSize;
                if (Vector2.Distance(attackerPos, defenderPos) < range)
                {
                    Vector2 defenderAngle1 = defenderPos - attackerPos;
                    defenderAngle1.Normalize();
                    float defenderDeg = MathEX.VectorToDegrees(defenderAngle1);
                    if (MathEX.DifferenceBetweenDegrees(defenderDeg,attackerRot)<arc)
                    {
                        defender[i].creature.DealDamage(1000000000);
                        defender[i].creature.CalcSystemsWithDur();
                    }

                }
            }
        }
    }
}
