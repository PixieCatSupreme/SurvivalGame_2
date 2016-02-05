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
        public static void OnMelee(Creature attacker, List<NPC> defender)
        {
            float range = 2;//todo generate range
            float arc = 30;
            Vector2 attackerPos = attacker.Pos + attacker.ChunkPos * ChunkSize;
            Vector2 rot = MathEX.DegreesToVector(attacker.Rotation);
            for (int i = 0; i < defender.Count; i++)
            {
                Vector2 defenderPos = defender[i].creature.Pos + defender[i].creature.ChunkPos * ChunkSize;
                if (Vector2.Distance(attackerPos, defenderPos) < range)
                {
                    if (MathEX.DifferenceBetweenDegrees(attacker.Rotation, defender[i].creature.Rotation) < 90)
                    {
                        Vector2 defenderPosLeft = defenderPos;
                        defenderPosLeft.X += rot.X;
                        defenderPosLeft.Y -= rot.Y;
                        Vector2 defenderPosRight = defenderPos;
                        defenderPosRight.X -= rot.X;
                        defenderPosRight.Y += rot.Y;
                        float defenderRotLeft = MathEX.VectorToDegrees(defenderPosLeft);
                        float defenderRotRight = MathEX.VectorToDegrees(defenderPosRight);
                        float attackerArcLeft = MathEX.VectorToDegrees(new Vector2(rot.X, -rot.Y));
                        float attackerArcRight = MathEX.VectorToDegrees(new Vector2(-rot.X, rot.Y));
                        if (MathEX.DifferenceBetweenDegrees(defenderRotLeft, attackerArcLeft) < 90 + arc && MathEX.DifferenceBetweenDegrees(defenderRotRight, attackerArcRight) < 90 + arc)
                        {
                            defender[i].creature.DealDamage(1000000);
                        }
                    }
                }
            }
        }
    }
}
