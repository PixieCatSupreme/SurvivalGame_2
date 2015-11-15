using Mentula.Content;
using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Client
{
    public class NPC : Actor
    {
        public string Name;
        public int TextureId;
        public byte HealthPrec;

        public NPC()
        {
            Name = "The Unnameable";
            TextureId = 9997;
            HealthPrec = 100;
        }

        public NPC(IntVector2 chunk, Vector2 tile, float rotation, byte health, string name)
            :base(tile, chunk)
        {
            Rotation = rotation;
            HealthPrec = health;
            Name = name;
        }

        public void Update(IntVector2 chunk, Vector2 tile, float rot, byte health)
        {
            ChunkPos = chunk;
            Pos = tile;
            Rotation = rot;
            HealthPrec = health;
        }
    }
}