using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Server;

namespace Mentula.Server
{
    public class Map
    {
        public List<Chunk> LoadedChunks;
        public List<Chunk> Chunks;
        public int LR = 1;
        public int GR = 2;

    }
}
