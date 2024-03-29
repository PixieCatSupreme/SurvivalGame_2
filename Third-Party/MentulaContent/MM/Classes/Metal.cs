﻿using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    [MMEditable]
    public class Metal : Material
    {
        public readonly StateOfMatter States;

        internal Metal()
            :base()
        {
            States = new StateOfMatter(new Vector3(float.PositiveInfinity));
        }

        internal Metal(StateOfMatter states, ulong id, string name, Vector3 values)
            : base(id, name, values.X, values.Y, values.Z)
        {
            States = states;
        }

        public override string ToString()
        {
            return "States=" + States.ToString() + " Material=" + base.ToString();
        }
    }
}
