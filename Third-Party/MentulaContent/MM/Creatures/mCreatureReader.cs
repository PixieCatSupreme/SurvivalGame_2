using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Mentula.Content.MM
{
    internal class MCreatureReader : ContentTypeReader<Creature[]>
    {
        protected override Creature[] Read(ContentReader input, Creature[] existingInstance)
        {
            return null;
        }
    }
}