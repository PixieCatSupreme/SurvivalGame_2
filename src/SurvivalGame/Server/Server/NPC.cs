//#define YOU_SPIN_ME_RIGHT_ROUND_BABY

using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;
using System.Collections.Generic;
using Mentula.Content;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        public NPC(string n, Stats s, int h, Vector2 pos, IntVector2 chunkpos)
            : base(n, s, h, pos, chunkpos)
        {
        }
    }
}
