using Mentula.Utilities;
using System.Collections.Generic;

namespace Mentula.Content
{
    [MMEditable]
    public class Item
    {
        [MMIgnore]
        public readonly byte[] Key;
        [MMOptional]
        public readonly ulong Volume;
        public readonly ulong Weight;
        [MMOptional]
        public byte Durability;

        [MMOptional]
        public readonly Tag[] Tags;
        [MMOptional]
        public readonly Item[] Parts;
        [MMOptional]
        public readonly IMaterial Material;

        internal Item()
        {
            /* Set Weight */
            Parts = new Item[0];
            #region SetWeight
            ulong result = 0, remainingVolume = Volume;

            //for (uint i = 0; i < Parts.Length; i++)
            //{
            //    result += Parts[i].Weight;
            //    remainingVolume -= Parts[i].Volume;
            //}

            //Weight = (ulong)(result + Material.Density * remainingVolume);
            #endregion
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

        public ulong GetTotalVolume()
        {
            ulong volume = Volume;

            for (int i = 0; i < Parts.Length; i++)
            {
                volume += Parts[i].GetTotalVolume();
            }

            return volume;
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
            ulong v = RNG.Next(GetTotalVolume());
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
                    v -= Parts[i].GetTotalVolume();

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