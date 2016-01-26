using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content.MM.Items
{
    [ContentProcessor(DisplayName = "Mentula Item Processor")]
    internal class MItemProcessor : ContentProcessor<MMSource, MItemProcessor.Manifest[]>
    {
        public override Manifest[] Process(MMSource input, ContentProcessorContext context)
        {
            Manifest[] result = new Manifest[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container cur = input.Container.Childs[i];

                try
                {
                    Manifest mani = new Manifest();
                    string rawValue = string.Empty;

                    mani.id = cur.GetUInt64Value("DEFAULT");

                    if (!string.IsNullOrWhiteSpace(cur.Name)) mani.name = cur.Name;
                    else throw new ParameterNullException("Container Name");

                    Container child;
                    if (cur.TryGetChild("Tags", out child))
                    {
                        mani.tags = new List<Tag>();

                        for (int j = 0; j < child.Childs.Length; j++)
                        {
                            Container tag = child.Childs[j];
                            mani.tags.Add(new Tag
                            {
                                Key = tag.GetInt16Value("Id"),
                                Value = tag.GetInt16Value("Value")
                            });
                        }
                    }

                    if (cur.TryGetChild("Material", out child))
                    {
                        if (child.Childs.Length < 1) throw new ParameterNullException("Material Pointer");

                        mani.volume = child.GetUInt64Value("Volume");
                        Container matPtr = child.Childs[0];
                        mani.material = new KeyValuePair<ulong, string>(
                            matPtr.GetUInt64Value("Id"),
                            matPtr.GetStringValue("DEFAULT"));
                    }

                    if (cur.TryGetChild("Items", out child))
                    {
                        mani.parts = new Dictionary<ulong, ulong[]>();

                        for (int j = 0; j < child.Childs.Length; j++)
                        {
                            Container database = child.Childs[j];
                            ulong dbId = database.GetUInt64Value("DEFAULT");

                            ulong[] parts = new ulong[database.Values.Count + database.Childs.Length];
                            for (int k = 0; k < database.Values.Count; k++)
                            {
                                KeyValuePair<string, string> value = database.Values.ElementAt(k);
                                parts[k] = Utils.ConvertToUInt64("Value", value.Value);
                            }

                            for (int k = 0; k < database.Childs.Length; k++)
                            {
                                Container itemPtr = database.Childs[k];
                                parts[k + database.Values.Count] = itemPtr.GetUInt64Value("Id");
                            }

                            mani.parts.Add(dbId, parts);
                        }
                    }

                    result[i] = mani;
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
            public bool IsValid { get { return !default(KeyValuePair<ulong, string>).Equals(material) || (parts != null && parts.Count > 0); } }

            public ulong id;
            public string name;
            public ulong volume;
            public KeyValuePair<ulong, string> material;
            public List<Tag> tags;
            public Dictionary<ulong, ulong[]> parts;
        }
    }
}