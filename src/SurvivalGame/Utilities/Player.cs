using Microsoft.Xna.Framework;

namespace Mentula.Utilities
{
    public class Player : Actor
    {
        public string Name;
        public float HealthPrec;

        public Player(IntVector2 chunk, Vector2 tile, float rotation, float health, string name)
            :base(tile, chunk)
        {
            Rotation = rotation;
            HealthPrec = health;
            Name = name;
        }
    }
}