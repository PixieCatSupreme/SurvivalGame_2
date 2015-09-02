using Mentula.Utilities;
using System.Diagnostics;

namespace Mentula.Utilities
{
    [DebuggerDisplay("{Pos}")]
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