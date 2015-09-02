using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Server
{
    public class Stats
    {
        public byte Strength;
        public byte Intelect;
        public byte Endurance;
        public byte Agility;
        public byte Perception;

        public Stats()
        {
            Strength = 0;
            Intelect = 0;
            Endurance = 0;
            Agility = 0;
            Perception = 0;
        }

        public Stats(Stats s)
        {
            Strength = s.Strength;
            Intelect = s.Intelect;
            Endurance = s.Endurance;
            Agility = s.Agility;
            Perception = s.Perception;
        }

        public static Stats operator +(Stats s, Stats S)
        {
            s.Strength += S.Strength;
            s.Intelect += S.Intelect;
            s.Endurance += S.Endurance;
            s.Agility += S.Agility;
            s.Perception += S.Perception;
            return s;
        }
    }
}
