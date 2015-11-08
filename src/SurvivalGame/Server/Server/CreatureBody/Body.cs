using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Server
{
    public class Body
    {
        public bool IsBio;
        public bool IsAlive { get { return IsAlive(); } }
        public float Health { get { return GetHealth(); } }
        
        private bool IsAlive()
        {
            return true;
        }

        private float GetHealth()
        {
            return 100;
        }
    }
}
