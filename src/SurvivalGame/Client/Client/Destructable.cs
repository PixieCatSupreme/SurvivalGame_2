using Mentula.Utilities;

namespace Mentula.Client
{
    public class Destructable : Tile
    {
        public float Health;
        public bool Walkable;

        public Destructable(int id, IntVector2 tilePos, float health)
            :base(id, tilePos)
        {
            Health = health;
            Walkable = false;
        }

        public static implicit operator bool(Destructable d)
        {
            return d != null && !d.Walkable;
        }
    }
}