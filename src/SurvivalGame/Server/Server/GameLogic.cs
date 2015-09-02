using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Server
{
    public class GameLogic
    {
        public Map Map { get; private set; }

        public GameLogic()
        {
            Map = new Map();
        }
    }
}