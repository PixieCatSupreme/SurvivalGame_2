using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Server
{
    public class NPC:Creature
    {
        private Vector2 targetArea;
        private double lastAttackTime;

        public void dostuff(float deltaTime,Creature[] players)
        {
            GetTarget();
        }

        private bool GetTarget()
        {
            bool t = false;

            return t;
        }

    }
}
