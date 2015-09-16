using System;
using System.Collections.Generic;

namespace Mentula.Content
{
    public class Item
    {
        public KeyValuePair<Type, object>[] materials;
        public Item[] itemMaterials;
        public Flag[] flags;
        public string name;

        internal Item(KeyValuePair<Type, object>[] materials, Item[] itemMaterials, Flag[] flags, string name)
        {
            this.materials = materials;
            this.itemMaterials = itemMaterials;
            this.flags = flags;
            this.name = name;
        }
    }
}
