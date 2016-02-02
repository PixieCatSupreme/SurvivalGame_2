using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula Creature Processor")]
    internal class MCreatureProcessor : ContentProcessor<MMSource, CreatureManifest[]>
    {
        public override CreatureManifest[] Process(MMSource input, ContentProcessorContext context)
        {
            CreatureManifest[] result = new CreatureManifest[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container cur = input.Container.Childs[i];

                try
                {
                    CreatureManifest mani = new CreatureManifest { parts = new Dictionary<ulong, KeyValuePair<ulong, ulong>[]>() };
                    mani.id = cur.GetUInt64Value("DEFAULT");
                    mani.name = cur.GetStringValue("Name");
                    mani.textureId = cur.GetInt32Value("TextureId");
                    mani.isBio = Convert.ToBoolean(cur.GetUInt16Value("IsBio"));

                    Container stats;
                    if (!cur.TryGetChild("Stats", out stats)) throw new ParameterNullException("Stats");

                    mani.stats = new Stats(
                        stats.GetInt16Value("Strength"),
                        stats.GetInt16Value("Intelect"),
                        stats.GetInt16Value("Endurance"),
                        stats.GetInt16Value("Agility"),
                        stats.GetInt16Value("Perception"));

                    Container parts;
                    if (!cur.TryGetChild("Parts", out parts)) throw new ParameterNullException("Parts");

                    for (int j = 0; j < parts.Childs.Length; j++)
                    {
                        Container db = parts.Childs[j];
                        ulong dbId = db.GetUInt64Value("DEFAULT");

                        KeyValuePair<ulong, ulong>[] items = new KeyValuePair<ulong, ulong>[db.Values.Count + db.Childs.Length];
                        int index = 0;

                        foreach (KeyValuePair<string, string> nonVolumePart in db.Values)
                        {
                            items[index++] = new KeyValuePair<ulong, ulong>(
                                Utils.ConvertToUInt64("DEFAULT", nonVolumePart.Value),
                                1);
                        }

                        for (int k = 0; k < db.Childs.Length; k++)
                        {
                            Container item = db.Childs[k];
                            items[index++] = new KeyValuePair<ulong, ulong>(
                                item.GetUInt64Value("Id"),
                                item.GetUInt64Value("Volume"));
                        }

                        mani.parts.Add(dbId, items);
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
    }

    public struct CreatureManifest
    {
        public ulong id;
        public string name;
        public int textureId;
        public bool isBio;
        public Stats stats;
        public Dictionary<ulong, KeyValuePair<ulong, ulong>[]> parts;

        public unsafe ulong GetByteCount()
        {
            ulong result = (ulong)Encoding.ASCII.GetBytes(name).LongLength;      // Name Byte count
            result += sizeof(int);                                              // Name Length specifier

            result += sizeof(int);                                             // Texture Id
            result += sizeof(bool);                                             // IsBio
            result += (ulong)sizeof(Stats);                                     // Stats

            foreach (KeyValuePair<ulong, KeyValuePair<ulong, ulong>[]> dataBank in parts)
            {
                result += sizeof(ulong);                                        // Database id
                result += sizeof(int);                                          // DataBank length
                result += (ulong)((sizeof(ulong) << 1) * dataBank.Value.Length);// DataBank
            }

            return result;
        }
    }
}