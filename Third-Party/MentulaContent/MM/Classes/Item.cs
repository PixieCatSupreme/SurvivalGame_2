﻿using Mentula.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mentula.Content
{
    [MMEditable]
    [DebuggerDisplay("{Name}")]
    public class Item
    {
        public readonly ulong Id;
        public readonly string Name;

        public readonly ulong Volume;
        public byte Durability;

        public readonly Tag[] Tags;
        public readonly Item[] Parts;
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
            List<Tag> result = new List<Tag>(Tags);

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
            float result = 0;

            if (Material != null)
            {
                result = Material.Density * Volume;
            }

            for (int i = 0; i < Parts.Length; i++)
            {
                result += Parts[i].CalcWeight();
            }

            return result;
        }

        public ulong CalcVolume()
        {
            ulong result = Volume;

            for (int i = 0; i < Parts.Length; i++)
            {
                result += Parts[i].CalcVolume();
            }

            return result;
        }

        public float GetHealth()
        {
            return Durability * Material.Ultimate_Tensile_Strength * Volume;
        }

        public float GetTotalHealth()
        {
            float health = Durability * Material.Ultimate_Tensile_Strength * Volume;

            for (int i = 0; i < Parts.Length; i++)
            {
                health += Parts[i].GetTotalHealth();
            }

            return health;
        }

        public void DealDamage(float damage)
        {
            ulong v = RNG.Next(CalcVolume());
            float remainingDamage = damage;

            if (Volume > 0)
            {
                v -= Volume;
                if (v < 0)
                {
                    if (GetHealth() <= remainingDamage)
                    {
                        remainingDamage -= GetHealth();
                        Durability = 0;
                    }
                    else
                    {
                        Durability -= (byte)(remainingDamage / Material.Ultimate_Tensile_Strength / Volume);
                        remainingDamage = 0;
                    }
                }
            }

            while (remainingDamage > 0 && GetTotalHealth() > 0)
            {
                for (int i = 0; i < Parts.Length; i++)
                {
                    v -= Parts[i].CalcVolume();

                    if (v < 0)
                    {
                        float partHealth = Parts[i].GetTotalHealth();
                        if (remainingDamage > partHealth)
                        {
                            remainingDamage -= partHealth;
                            Parts[i].DealDamage(partHealth);
                        }
                        else
                        {
                            Parts[i].DealDamage(remainingDamage);
                            remainingDamage = 0;
                        }
                    }
                }
            }
        }
    }
}