namespace Mentula.Server
{
    public class Stats
    {
        public short Strength;
        public short Intelect;
        public short Endurance;
        public short Agility;
        public short Perception;

        public Stats()
        {
            Strength = 10;
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