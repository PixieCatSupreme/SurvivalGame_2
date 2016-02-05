using Mentula.Content;
using Microsoft.Xna.Framework.Content;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        public Metal GetMetal(ulong id)
        {
            if (metals == null)
            {
                if ((metals = Load<Metal[]>("Databases/Metals")) == null) return null;
            }

            for (uint i = 0; i < metals.Length; i++)
            {
                Metal cur = metals[i];

                if (cur.Id == id) return cur;
            }

            return null;
        }

        public Biomass GetBiomass(ulong id)
        {
            if (biomasses == null)
            {
                if ((biomasses = Load<Biomass[]>("Databases/Biomass")) == null) return null;
            }

            for (uint i = 0; i < biomasses.Length; i++)
            {
                Biomass cur = biomasses[i];

                if (cur.Id == id) return cur;
            }

            return null;
        }
    }
}