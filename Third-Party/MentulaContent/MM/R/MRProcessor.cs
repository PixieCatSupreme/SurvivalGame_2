using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula R Processor")]
    internal class MRProcessor : ContentProcessor<MMSource, R>
    {
        public override R Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("R", input);

            int length = input.Container.Childs.Length;
            KeyValuePair<string, KeyValuePair<int, string>[]>[] result = new KeyValuePair<string, KeyValuePair<int, string>[]>[length];

            for (int i = 0; i < length; i++)
            {
                Container cur = input.Container.Childs[i];
                Manifest mani = new Manifest();

                try
                {
                    string rawValue = string.Empty;

                    if (cur.TryGetValue("Default", out rawValue)) mani.Name = rawValue;
                    else throw new ParameterNullException("Name");

                    mani.Values = new KeyValuePair<int, string>[(cur.Values.Count - 1) >> 1];

                    int index = 0;
                    for (int j = 1; j < cur.Values.Count; j += 2)
                    {
                        int key = 0;

                        string rawKey = cur.Values.ElementAt(j).Value;
                        if (j + 1 < cur.Values.Count) rawValue = cur.Values.ElementAt(j + 1).Value;
                        else throw new ParameterNullException("Value at key: " + rawKey);

                        if (!int.TryParse(rawKey, out key)) throw new ParameterException("Id", rawKey, typeof(int));

                        mani.Values[index] = new KeyValuePair<int, string>(key, rawValue);
                        index++;
                    }

                    result[i] = new KeyValuePair<string, KeyValuePair<int, string>[]>(mani.Name, mani.Values);
                }
                catch(Exception e)
                {
                    throw new ContainerException(cur.Name, e);
                }
            }

            return new R(result);
        }

        private struct Manifest
        {
            public string Name;
            public KeyValuePair<int, string>[] Values;
        }
    }
}