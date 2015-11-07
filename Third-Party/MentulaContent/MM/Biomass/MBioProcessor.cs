using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula Biomass Processor")]
    internal class MBioProcessor : ContentProcessor<MMSource, Biomass[]>
    {
        public override Biomass[] Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("Biomass", input.Container.Values["DEFAULT"]);
            
            Biomass[] result = new Biomass[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container cur = input.Container.Childs[i];
                Manifest mani = new Manifest();
                string rawValue = "";

                const string ID = "Id";
                if (cur.TryGetValue(ID, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Id = raw;
                    else throw new ParameterException(ID, rawValue, typeof(int));
                }
                else throw new ParameterNullException(ID);

                const string NAME = "Name";
                if (cur.TryGetValue(NAME, out rawValue)) mani.Name = rawValue;
                else throw new ParameterNullException(NAME);

                const string UTS = "UTS";
                if (cur.TryGetValue(UTS, out rawValue))
                {
                    float raw = 0;

                    if (Utils.TryParse(rawValue, out raw)) mani.Values.X = raw;
                    else throw new ParameterException(UTS, rawValue, typeof(float));
                }
                else throw new ParameterNullException(NAME);

                const string TSAY = "TSAY";
                if (cur.TryGetValue(TSAY, out rawValue))
                {
                    float raw = 0;

                    if (Utils.TryParse(rawValue, out raw)) mani.Values.Y = raw;
                    else throw new ParameterException(TSAY, rawValue, typeof(float));
                }
                else throw new ParameterNullException(TSAY);

                const string DENS = "Density";
                if (cur.TryGetValue(DENS, out rawValue))
                {
                    float raw = 0;

                    if (Utils.TryParse(rawValue, out raw)) mani.Values.Z = raw;
                    else throw new ParameterException(DENS, rawValue, typeof(float));
                }
                else throw new ParameterNullException(DENS);

                const string BURN = "BurnTemperature";
                if (cur.TryGetValue(BURN, out rawValue))
                {
                    float raw = 0;

                    if (Utils.TryParse(rawValue, out raw)) mani.Burn = raw;
                    else throw new ParameterException(BURN, rawValue, typeof(float));
                }
                else throw new ParameterNullException(BURN);

                const string NUTR = "NutritiousValue";
                if (cur.TryGetValue(NUTR, out rawValue))
                {
                    float raw = 0;

                    if (Utils.TryParse(rawValue, out raw)) mani.Nutr = raw;
                    else throw new ParameterException(NUTR, rawValue, typeof(float));
                }
                else throw new ParameterNullException(NUTR);

                result[i] = new Biomass(mani.Burn, mani.Nutr, mani.Id, mani.Name, mani.Values);
            }

            return result;
        }

        internal struct Manifest
        {
            public int Id;
            public string Name;
            public Vector3 Values;
            public float Burn;
            public float Nutr;
        }
    }
}