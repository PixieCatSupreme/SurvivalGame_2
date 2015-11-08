using Mentula.Content;
using Microsoft.Xna.Framework.Content;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        public Creature GetCreature(string name)
        {
            if (creatures == null)
            {
                if ((creatures = Load<Creature[]>("Creatures")) == null) return null;
            }

            for (uint i = 0; i < creatures.Length; i++)
            {
                Creature cur = creatures[i];

                if (cur.Name.ToUpper() == name.ToUpper()) return cur;
            }

            return null;
        }
    }
}