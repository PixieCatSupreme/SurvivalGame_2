using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mentula.Content.MM
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

                    bool isMat = false;
                    if (cur.TryGetChild("Material", out child))
                    {
                        if (child.Childs.Length < 1) throw new ParameterNullException("Material Pointer");
                        isMat = true;

                        mani.volume = child.GetUInt64Value("Volume");
                        Container matPtr = child.Childs[0];
                        ulong dbId;

                        if (child.TryGetValue("DEFUALT", out rawValue)) dbId = Utils.ConvertToUInt64("DEFAULT", rawValue);
                        else dbId = cur.GetUInt64Value("Id");

                        mani.material = new KeyValuePair<ulong, ulong>(
                            matPtr.GetUInt64Value("Id"),
                            dbId);
                    }

                    if (cur.TryGetChild("Items", out child))                                                // Read Items
                    {
                        if (isMat) throw new ArgumentException("An item cannot contain a material and parts!");
                        mani.parts = new Dictionary<ulong, KeyValuePair<ulong, ulong>[]>();

                        ulong dbId = cur.GetUInt64Value("Id");                                              // Get current dbId.

                        KeyValuePair<ulong, ulong>[] databank = new KeyValuePair<ulong, ulong>[child.Values.Count];
                        for (int j = 0; j < databank.Length; j++)                                           // Read databank for local non volume pointers.
                        {
                            databank[j] = new KeyValuePair<ulong, ulong>(
                                Utils.ConvertToUInt64("Id", child.Values.ElementAt(j).Value),               // Get id from local non volume pointer.
                                0);                                                                         // Volume for non specified.
                        }

                        mani.parts.Add(dbId, databank);

                        List<KeyValuePair<ulong, ulong>> localVolSpecParts = new List<KeyValuePair<ulong, ulong>>();
                        for (int j = 0; j < child.Childs.Length; j++)                                       // Read (Local volume specified pointers) && (non local pointers).
                        {
                            if (child.Childs[j].Childs.Length > 0)                                          // If current child is database pointer.
                            {
                                Container database = child.Childs[j];
                                dbId = database.GetUInt64Value("DEFAULT");                                  // Get specified dbId.

                                databank = new KeyValuePair<ulong, ulong>[database.Values.Count];
                                for (int k = 0; k < databank.Length; k++)                                   // Read non volume pointers.
                                {
                                    databank[k] = new KeyValuePair<ulong, ulong>(
                                        Utils.ConvertToUInt64("Id", database.Values.ElementAt(k).Value),    // Get id from non volume pointer.
                                        0);                                                                 // Volume for non specified.
                                }

                                mani.parts.Add(dbId, databank);
                            }
                            else                                                                            // Current child if volume specified part.
                            {
                                Container part = child.Childs[j];
                                localVolSpecParts.Add(new KeyValuePair<ulong, ulong>(
                                    part.GetUInt64Value("Id"),
                                    part.GetUInt64Value("Volume")));
                            }
                        }

                        mani.parts.Add(cur.GetUInt64Value("Id"), localVolSpecParts.ToArray());
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
            public bool IsValid { get { return !default(KeyValuePair<ulong, ulong>).Equals(material) || (parts != null && parts.Count > 0); } }
            public bool IsBase { get { return parts == null; } }

            public ulong id;                                                // Item Id
            public string name;                                             // Item Name
            public ulong volume;                                            // Item Volume
            public KeyValuePair<ulong, ulong> material;                     // (matId, dbId)
            public List<Tag> tags;                                          // (key, value)
            public Dictionary<ulong, KeyValuePair<ulong, ulong>[]> parts;   // (dbId, (partId, volume))

            public ulong GetByteCount()
            {
                ulong result = (ulong)Encoding.UTF8.GetBytes(name).LongLength;          // Name Byte count
                result += sizeof(int);                                                  // Name Length specifier

                result += sizeof(byte);                                                 // Tag length (octet)       
                if (tags != null) result += (ulong)((sizeof(short) << 1) * tags.Count); // Tags

                result += sizeof(ulong);                                                // Volume
                result += sizeof(byte);                                                 // If has item / Length or padbits

                if (IsBase) result += sizeof(ulong) << 1;                               // If Mat add dbId + ItemId
                else
                {
                    foreach (KeyValuePair<ulong, KeyValuePair<ulong, ulong>[]> dataBank in parts)
                    {
                        result += sizeof(ulong);                                        // Database id
                        result += sizeof(int);                                          // DataBank length
                        result += (ulong)((sizeof(ulong) << 1) * dataBank.Value.Length);// DataBank
                    }
                }

                return result;
            }
        }
    }
}