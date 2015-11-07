using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Creature : Actor
    {
        public readonly string Name;
        public readonly Stats Stats;
        public int Health;
        public readonly int MaxHealth;
        public bool IsAlive;
        public readonly int TextureId;

        internal Creature()
        {
            Name = "The Unnameable";
            Stats = new Stats();
            Health = 100;
            MaxHealth = 100;
            IsAlive = true;
        }

        internal Creature(string n, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = new Stats();
            Health = 100;
            MaxHealth = 100;
            IsAlive = true;
        }

        internal Creature(string n, Stats s, int mh, int h, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = s;
            Health = h;
            MaxHealth = mh;
            IsAlive = true;
        }

        public static Creature CreatePlayer(string name)
        {
            return new Creature(name, Vector2.Zero, IntVector2.Zero);
        }
    }
}
