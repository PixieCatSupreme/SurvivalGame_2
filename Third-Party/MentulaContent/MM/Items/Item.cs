using Mentula.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Content.MM.Items
{
    public class Item
    {
        public ulong Id;
        public uint Volume;
        public ushort Durability;

        public Pair<string, short>[] Tags;
        public Item[] Parts;
        public Material[] Materials;
    }
}
