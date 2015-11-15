using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula Metals Processor")]
    internal class MMetalProcessor : ContentProcessor<MMSource, Metal[]>
    {
        public override Metal[] Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("Metals", input.Container.Values["DEFAULT"]);

            Metal[] result = new Metal[input.Container.Childs.Length];

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
                        int raw = 0;

                        if (int.TryParse(rawValue, out raw)) mani.Id = raw;
                        else throw new ParameterException(ID, rawValue, typeof(int));
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

                    const string MELT = "MeltingPoint";
                    if (cur.TryGetValue(MELT, out rawValue))
                    {
                        float raw = 0;

                        if (Utils.TryParse(rawValue, out raw)) mani.States.X = raw;
                        else throw new ParameterException(MELT, rawValue, typeof(float));
                    }
                    else throw new ParameterNullException(MELT);

                    const string VAP = "VaporizationPoint";
                    if (cur.TryGetValue(VAP, out rawValue))
                    {
                        float raw = 0;

                        if (Utils.TryParse(rawValue, out raw)) mani.States.Y = raw;
                        else throw new ParameterException(VAP, rawValue, typeof(float));
                    }
                    else throw new ParameterNullException(VAP);

                    const string ION = "IonizationPoint";
                    if (cur.TryGetValue(ION, out rawValue))
                    {
                        float raw = 0;

                        if (Utils.TryParse(rawValue, out raw)) mani.States.Z = raw;
                        else throw new ParameterException(VAP, rawValue, typeof(float));
                    }
                    else throw new ParameterNullException(VAP);

                    result[i] = new Metal(new StateOfMatter(mani.States), mani.Id, mani.Name, mani.Values);
                }
                catch (Exception e)
                {
                    throw new ContainerException(cur.Name, e);
                }
            }

            return result;
        }

        internal struct Manifest
        {
            public int Id;
            public string Name;
            public Vector3 Values;
            public Vector3 States;

            public Manifest(int id, string name)
            {
                Id = id;
                Name = name;
                Values = default(Vector3);
                States = default(Vector3);
            }
        }
    }
}