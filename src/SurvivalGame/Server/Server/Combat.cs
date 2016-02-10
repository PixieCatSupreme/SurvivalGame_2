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
        public static bool OnMelee(Creature attacker,ref List<NPC> defender,ref List<NPC> ded)
        {
            bool killed = false;
            float range = 2;//todo generate range
            float arc = 30;
            Vector2 attackerPos = attacker.Pos + attacker.ChunkPos * ChunkSize;
            float attackerRot = attacker.Rotation / (float)Math.PI * 180;

            for (int i = 0; i < defender.Count; i++)
            {
                Vector2 defenderPos = defender[i].Pos + defender[i].ChunkPos * ChunkSize;
                if (Vector2.Distance(attackerPos, defenderPos) < range)
                {
                    Vector2 defenderAngle1 = defenderPos - attackerPos;
                    defenderAngle1.Normalize();
                    float defenderDeg = MathEX.VectorToDegrees(defenderAngle1);
                    if (MathEX.DifferenceBetweenDegrees(defenderDeg,attackerRot)<arc)
                    {
                        defender[i].DealDamage(2000000);
                        defender[i].CalcSystemsWithDur();
                        if (!defender[i].CalcIsAlive())
                        {
                            ded.Add(defender[i]);
                            defender.RemoveAt(i);
                            killed = true;
                        }
                    }
                }
            }
            return killed;
        }
    }
}
