using Mentula.Utilities;

namespace Mentula.Utilities
{
    public class Tile
    {
        public int Tex;
        public IntVector2 Pos;

        public Tile(int texture, IntVector2 position)
        {
            Tex = texture;
            Pos = position;
        }
    }
}