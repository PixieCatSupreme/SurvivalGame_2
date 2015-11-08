using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Server
{
    public class BodyPart
    {
        public List<BodyPart> BodyParts;
        public UInt16 Size;
        public UInt16 Health;
        public UInt16 Maxhealth;
        public HealthSystems h;
    }
}
