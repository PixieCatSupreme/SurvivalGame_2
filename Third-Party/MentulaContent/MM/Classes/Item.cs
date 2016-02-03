using Mentula.Utilities;
using Mentula.Utilities.Udp;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mentula.Content
{
    [MMEditable]
    [DebuggerDisplay("{Name}")]
    public class Item
    {
        [MMIgnore]
        public readonly ulong Id;
        public readonly string Name;
        [MMOptional]
        public readonly ulong Volume;
        [MMOptional]
        public byte Durability;

        [MMOptional]
        public readonly Tag[] Tags;
        [MMOptional]
        public readonly Item[] Parts;
        [MMOptional]
        public readonly Material Material;

        internal Item()
        {
            Durability = 100;
            Tags = new Tag[0];
            Parts = new Item[0];
        }

        public Item(ulong id, string name, Material material, ulong volume)
            : this()
        {
            Id = id;
            Volume = volume;
            Name = name;
            Material = material;
        }

        public Item(ulong id, string name, Material material, ulong volume, Tag[] tags)
            : this()
        {
            Id = id;
            Volume = volume;
            Name = name;
            Material = material;
            Tags = tags;
        }

        public Item(ulong id, string name, Item[] parts)
            : this()
        {
            Id = id;
            Name = name;
            Parts = parts;
        }

        public Item(ulong id, string name, Item[] parts, Tag[] tags)
            : this()
        {
            Id = id;
            Name = name;
            Parts = parts;
            Tags = tags;
        }

        public Tag[] GetAllTags()
        {
            List<Tag> result = new List<Tag>();

            for (int i = 0; i < Parts.Length; i++)
            {
                Tag[] childTags = Parts[i].GetAllTags();

                for (int j = 0; j < childTags.Length; j++)
                {
                    Tag current = childTags[j];
                    bool found = false;

                    for (int k = 0; k < result.Count; k++)
                    {
                        if (result[k].Key == current.Key)
                        {
                            result[k] += current;
                            found = true;
                            break;
                        }
                    }

                    if (!found) result.Add(current);
                }
            }

            return result.ToArray();
        }

        public Tag[] GetAllTagsWithDurability()
        {
            List<Tag> result = new List<Tag>();

            for (int i = 0; i < Parts.Length; i++)
            {
                Tag[] childTags = Parts[i].GetAllTagsWithDurability();

                for (int j = 0; j < childTags.Length; j++)
                {
                    Tag current = childTags[j];
                    current.Value *= Durability;
                    bool found = false;

                    for (int k = 0; k < result.Count; k++)
                    {
                        if (result[k].Key == current.Key)
                        {
                            result[k] += current;
                            found = true;
                            break;
                        }
                    }

                    if (!found) result.Add(current);
                }
            }

            return result.ToArray();
        }

        public float CalcWeight()
        {
            float weight = 0;

            if (Material != null)
            {
                weight = Material.Density * Volume;
            }

            for (int i = 0; i < Parts.Length; i++)
            {
                weight += Parts[i].CalcWeight();
            }

            return weight;
        }
    }
}