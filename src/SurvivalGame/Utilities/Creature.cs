using Microsoft.Xna.Framework;

namespace Mentula.Utilities
{
    public class Creature : Actor
    {
        
        public string Name;
        public Stats Stats;
        public int[] Health;
        public int[] MaxHealth;

        public Creature()
            : base()
        {
            Name = "The Unnameable";
            Stats = new Stats();
            Health = new int[6] { dh, dh, dh, dh, dh, dh };
            MaxHealth = new int[6] { dh, dh, dh, dh, dh, dh };
        }

        public Creature(string n, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = new Stats();
            Health = new int[6] { dh, dh, dh, dh, dh, dh };
            MaxHealth = new int[6] { dh, dh, dh, dh, dh, dh };
        }

        public Creature(string n, Stats s, int h, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = s;
            Health = new int[6] { h, h, h, h, h, h };
            MaxHealth = new int[6] { h, h, h, h, h, h };
        }

        private const int dh = 50;
    }
}
