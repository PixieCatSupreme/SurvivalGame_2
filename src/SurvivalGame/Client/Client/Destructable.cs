using Mentula.Utilities;

namespace Mentula.Client
{
    public class Destructable : Tile
    {
        public float Health;

        public Destructable(int id, IntVector2 tilePos, float health)
            :base(id, tilePos)
        {
            Health = health;
        }
    }
}