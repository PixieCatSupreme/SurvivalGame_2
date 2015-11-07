namespace Mentula.Content
{
    public struct Stats
    {
        public readonly short Strength;
        public readonly short Intelect;
        public readonly short Endurance;
        public readonly short Agility;
        public readonly short Perception;

        internal Stats(short n)
        {
            Strength = n;
            Intelect = n;
            Endurance = n;
            Agility = n;
            Perception = n;
        }

        internal Stats(short strength, short intelect, short endurance, short agility, short perception)
        {
            Strength = strength;
            Intelect = intelect;
            Endurance = endurance;
            Agility = agility;
            Perception = perception;
        }

        internal Stats(Stats s)
        {
            Strength = s.Strength;
            Intelect = s.Intelect;
            Endurance = s.Endurance;
            Agility = s.Agility;
            Perception = s.Perception;
        }

        public static Stats operator +(Stats l, Stats r)
        {
            return new Stats(
                (short)(l.Strength + r.Strength),
                (short)(l.Intelect + r.Intelect),
                (short)(l.Endurance + r.Endurance),
                (short)(l.Agility + r.Agility),
                (short)(l.Perception + r.Perception));
        }
    }
}