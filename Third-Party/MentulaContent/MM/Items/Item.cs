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
    }
}