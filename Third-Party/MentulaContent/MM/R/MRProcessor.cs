using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;

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

                try
                {
                    string tagCategory = cur.Name;
                    KeyValuePair<int, string>[] values = new KeyValuePair<int, string>[cur.Values.Count + cur.Childs.Length];

                    int index = 0;
                    foreach (KeyValuePair<string, string> tag in cur.Values)
                    {
                        values[index++] = new KeyValuePair<int, string>(Utils.ConvertToInt32("Id", tag.Value), tag.Key);
                    }

                    for (int j = 0; j < cur.Childs.Length; j++)
                    {
                        Container tag = cur.Childs[j];
                        values[index++] = new KeyValuePair<int, string>(
                            tag.GetInt32Value("Id"),
                            tag.GetStringValue("Name"));
                    }

                    result[i] = new KeyValuePair<string, KeyValuePair<int, string>[]>(tagCategory, values);
                }
                catch (Exception e)
                {
                    throw new ContainerException(cur.Name, e);
                }
            }

            return new R(result);
        }
    }
}