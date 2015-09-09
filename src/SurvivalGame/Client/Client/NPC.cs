using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Client
{
    public class NPC : Actor
    {
        public string Name;
        public int TextureId;
        public float HealthPrec;

        public NPC(IntVector2 chunk, Vector2 tile, float rotation, float health, string name)
            :base(tile, chunk)
        {
            Rotation = rotation;
            HealthPrec = health;
            Name = name;
        }

        public void Update(IntVector2 chunk, Vector2 tile, float rot, float health)
        {
            ChunkPos = chunk;
            Pos = tile;
            Rotation = rot;
            HealthPrec = health;
        }
    }
}