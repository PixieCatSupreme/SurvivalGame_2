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
            Utils.CheckProcessorType("Biomass", input);
            
            Biomass[] result = new Biomass[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container cur = input.Container.Childs[i];

                try
                {
                    Manifest mani = new Manifest();
                    string rawValue = string.Empty;

                    mani.Id = cur.GetUInt64Value("DEFAULT");

                    if (!string.IsNullOrWhiteSpace(cur.Name)) mani.Name = cur.Name;
                    else throw new ArgumentNullException("Container Name");

                    mani.Values.X = cur.GetFloatValue("UTS");
                    mani.Values.Y = cur.GetFloatValue("TSAY");
                    mani.Values.Z = cur.GetFloatValue("Density");

                    mani.Burn = cur.GetFloatValue("BurnTemperature");
                    mani.Nutr = cur.GetFloatValue("NutritiousValue");

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