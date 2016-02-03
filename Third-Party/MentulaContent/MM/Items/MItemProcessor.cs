using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula Item Processor")]
    internal class MItemProcessor : ContentProcessor<MMSource, ItemManifest[]>
    {
        public override ItemManifest[] Process(MMSource input, ContentProcessorContext context)
        {
            ItemManifest[] result = new ItemManifest[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container cur = input.Container.Childs[i];

                try
                {
                    ItemManifest mani = new ItemManifest();
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

                    bool set = false;
                    if (cur.TryGetChild("Material", out child))
                    {
                        set = true;

                        ulong dbId;
                        if (child.TryGetValue("db", out rawValue)) dbId = Utils.ConvertToUInt64("db", rawValue);
                        else dbId = input.Container.GetUInt64Value("Id");

                        mani.volume = child.GetUInt64Value("Volume");
                        mani.material = new KeyValuePair<ulong, ulong>(
                            child.GetUInt64Value("id"),
                            dbId);
                    }

                    if (cur.TryGetChild("Items", out child))                                                // Read Items
                    {
                        if (set) throw new ArgumentException("An item cannot contain a material and parts!");
                        mani.parts = new Dictionary<ulong, KeyValuePair<ulong, ulong>[]>();
                        set = true;

                        ulong dbId = input.Container.GetUInt64Value("Id");                                  // Get current dbId.
                        List<KeyValuePair<ulong, ulong>> localVolSpecParts = new List<KeyValuePair<ulong, ulong>>();

                        for (int j = 0; j < child.Values.Count; j++)                                        // Read databank for local non volume pointers.
                        {
                            localVolSpecParts.Add(new KeyValuePair<ulong, ulong>(
                                Utils.ConvertToUInt64("Id", child.Values.ElementAt(j).Value),               // Get id from local non volume pointer.
                                100));                                                                        // Volume for non specified.
                        }

                        for (int j = 0; j < child.Childs.Length; j++)                                       // Read (Local volume specified pointers) && (non local pointers).
                        {
                            if (child.Childs[j].Childs.Length > 0)                                          // If current child is database pointer.
                            {
                                Container database = child.Childs[j];
                                dbId = database.GetUInt64Value("DEFAULT");                                  // Get specified dbId.

                                KeyValuePair<ulong, ulong>[] databank = new KeyValuePair<ulong, ulong>[database.Childs.Length];
                                for (int k = 0; k < databank.Length; k++)                                   // Read non volume pointers.
                                {
                                    Container part = database.Childs[k];
                                    databank[k] = new KeyValuePair<ulong, ulong>(
                                        part.GetUInt64Value("Id"),                                          // Get id from non volume pointer.
                                        part.GetUInt64Value("Volume"));
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

                        if (!set) throw new BuildException($"Item found with no base material or base items: {cur.Name}!");
                        mani.parts.Add(input.Container.GetUInt64Value("Id"), localVolSpecParts.ToArray());
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

    [DebuggerDisplay("[{id}] {name}")]
    public struct ItemManifest
    {
        public bool IsValid { get { return !default(KeyValuePair<ulong, ulong>).Equals(material) || (parts != null && parts.Count > 0); } }
        public bool IsBase { get { return parts == null || parts.Count < 1; } }

        public ulong id;                                                // Item Id
        public string name;                                             // Item Name
        public ulong volume;                                            // Item Volume
        public KeyValuePair<ulong, ulong> material;                     // (matId, dbId)
        public List<Tag> tags;                                          // (key, value)
        public Dictionary<ulong, KeyValuePair<ulong, ulong>[]> parts;   // (dbId, (partId, volume))

        public ulong GetByteCount()
        {
            ulong result = (ulong)Encoding.ASCII.GetBytes(name).LongLength;          // Name Byte count
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