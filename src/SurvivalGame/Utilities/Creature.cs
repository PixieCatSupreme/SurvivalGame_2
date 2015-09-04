using Microsoft.Xna.Framework;

namespace Mentula.Utilities
{
    public class Creature : Actor
    {
        
        public string Name;
        public Stats Stats;
        public int[] Health;
        public int[] MaxHealth;

        public int animationName;
        public float animationSpeed;
        public int animationFrame;

        public Creature()
            : base()
        {
            Name = "The Unnameable";
            Stats = new Stats();
            Health = new int[6] { dh, dh, dh, dh, dh, dh };
            MaxHealth = new int[6] { dh, dh, dh, dh, dh, dh };

            animationName = 0;
            animationSpeed =0;
            animationFrame = 0;
        }

        public Creature(string n, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = new Stats();
            Health = new int[6] { dh, dh, dh, dh, dh, dh };
            MaxHealth = new int[6] { dh, dh, dh, dh, dh, dh };

            animationName = 0;
            animationSpeed = 0;
            animationFrame = 0;
        }

        public Creature(string n, Stats s, int h, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = s;
            Health = new int[6] { h, h, h, h, h, h };
            MaxHealth = new int[6] { h, h, h, h, h, h };

            animationName = 0;
            animationSpeed = 0;
            animationFrame = 0;
        }

        private const int dh = 50;
    }
}
