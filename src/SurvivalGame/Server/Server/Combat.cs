using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mentula.Utilities.MathExtensions;

namespace Mentula.Server
{
    public static class Combat
    {

        public static void OnAttack(ref Creature[] creatures,ref Creature attacker)
        {
            Vector2 angle = MathEX.RadiansToVector(attacker.Rotation);
            for (int i = 0; i < creatures.Length; i++)
            {
                if (true)
                {
                    DoDamage(ref creatures[i],ref attacker);
                    break;
                }
            }
        }

        private static void DoDamage(ref Creature defender, ref Creature attacker)
        {
            Random r = new Random();
            while (true)
            {
                r.Next(6);
                

            }
        }
    }
}
