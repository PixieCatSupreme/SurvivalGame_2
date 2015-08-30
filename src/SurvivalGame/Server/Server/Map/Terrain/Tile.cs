using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;

namespace Mentula.Server
{
    public class Tile
    {
        public int Tex;
        public IntVector2 Pos;
        public int Layer;

        public Tile(int texture, IntVector2 position, int layer)
        {
            Tex = texture;
            Pos = position;
            Layer = layer;
        }
    }
}
