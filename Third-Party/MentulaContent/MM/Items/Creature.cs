using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Creature : Actor
    {
        public readonly string Name;
        public readonly Stats Stats;
        public readonly int TextureId;
        public Item[] Parts;
        public bool isbio;
        public bool IsAlive { private set; get; }

        internal Creature()
        {
            Name = "The Unnameable";
            Stats = new Stats(short.MaxValue);
        }

        internal Creature(string n, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = new Stats(10);
        }

        internal Creature(string n, Stats s, Vector2 pos, IntVector2 chunkpos)
            : base(pos, chunkpos)
        {
            Name = n;
            Stats = s;
        }

        public static Creature CreatePlayer(string name)
        {
            return new Creature(name, Vector2.Zero, IntVector2.Zero);
        }

        public void CalcIsAlive()
        {
            IsAlive= true;
        }

        public byte GetHealth()
        {
            return 0;
        }
    }
}
