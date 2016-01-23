using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;

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

                try
                {
                    Manifest mani = new Manifest();
                    string rawValue = "";

                    const string ID = "DEFAULT";
                    if (cur.TryGetValue(ID, out rawValue))
                    {
                        ulong raw = 0;

                        if (ulong.TryParse(rawValue, out raw)) mani.Id = raw;
                        else throw new ParameterException(ID, rawValue, typeof(ulong));
                    }
                    else throw new ParameterNullException(ID);

                    if (!string.IsNullOrWhiteSpace(cur.Name)) mani.Name = cur.Name;
                    else throw new ArgumentNullException("Container Name");

                    const string UTS = "UTS";
                    if (cur.TryGetValue(UTS, out rawValue))
                    {
                        float raw = 0;

                        if (Utils.TryParse(rawValue, out raw)) mani.Values.X = raw;
                        else throw new ParameterException(UTS, rawValue, typeof(float));
                    }
                    else throw new ParameterNullException(UTS);

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
                catch(Exception e)
                {
                    throw new ContainerException(cur.Name, e);
                }
            }

            return result;
        }

        internal struct Manifest
        {
            public ulong Id;
            public string Name;
            public Vector3 Values;
            public float Burn;
            public float Nutr;
        }
    }
}