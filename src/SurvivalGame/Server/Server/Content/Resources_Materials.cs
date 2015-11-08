using Mentula.Content;
using Microsoft.Xna.Framework.Content;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        public Metal GetMetal(int id)
        {
            if (metals == null)
            {
                if ((metals = Load<Metal[]>("Metals")) == null) return null;
            }

            for (uint i = 0; i < metals.Length; i++)
            {
                Metal cur = metals[i];

                if (cur.Id == id) return cur;
            }

            return null;
        }

        public Biomass GetBiomass(int id)
        {
            if (biomasses == null)
            {
                if ((biomasses = Load<Biomass[]>("Biomass")) == null) return null;
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