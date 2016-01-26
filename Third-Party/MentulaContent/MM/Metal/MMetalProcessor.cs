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
            Utils.CheckProcessorType("Metals", input);

            Metal[] result = new Metal[input.Container.Childs.Length];

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

                    mani.States.X = cur.GetFloatValue("MeltingPoint");
                    mani.States.Y = cur.GetFloatValue("VaporizationPoint");
                    mani.States.Z = cur.GetFloatValue("IonizationPoint");

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
            public ulong Id;
            public string Name;
            public Vector3 Values;
            public Vector3 States;

            public Manifest(ulong id, string name)
            {
                Id = id;
                Name = name;
                Values = default(Vector3);
                States = default(Vector3);
            }
        }
    }
}