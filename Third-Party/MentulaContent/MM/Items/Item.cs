using Mentula.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Content
{
    public class Item
    {
        public readonly byte[] Key;
        public readonly ulong Volume;
        public readonly ulong Weight;
        public byte Durability;

        public readonly Tag[] Tags;
        public readonly Item[] Parts;
        public readonly IMaterial Material;

        internal Item()
        {


            /* Set Weight */
            #region SetWeight
            ulong result = 0, remainingVolume = Volume;

            for (uint i = 0; i < Parts.Length; i++)
            {
                result += Parts[i].Weight;
                remainingVolume -= Parts[i].Volume;
            }

            Weight = (ulong)(result + Material.Density * remainingVolume);
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
    }
}