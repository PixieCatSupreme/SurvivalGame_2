using Mentula.Content;
using Mentula.Content.MM;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        public Item GetItem(ulong id, ulong volumeModifier = 1)   //TODO: Replace woth propper loading
        {
            if (items == null)
            {
                Manifest[] manifests = Load<Manifest[]>("BodyParts");
                items = new Item[manifests.Length];

                for (int i = 0; i < manifests.Length; i++)
                {
                    Manifest cur = manifests[i];

                    if (cur.IsBase)
                    {
                        IMaterial material = null;
                        if (cur.material.Value == 1)    // Biomass
                        {
                            material = GetBiomass(cur.material.Key);
                        }

                        items[i] = new Item(
                            cur.id,
                            cur.name,
                            material,
                            cur.volume * volumeModifier,
                            cur.tags.ToArray());
                    }
                    else
                    {
                        int prtsCount = 0;
                        foreach (var database in cur.parts)
                        {
                            prtsCount += database.Value.Length;
                        }

                        Item[] parts = new Item[prtsCount];
                        int index = 0;
                        foreach (var database in cur.parts)
                        {
                            if (database.Key == 3) // BodyPart
                            {
                                for (int j = 0; j < database.Value.Length; j++)
                                {
                                    KeyValuePair<ulong, ulong> part = database.Value[j];
                                    parts[index++] = GetItem(part.Key, part.Value * volumeModifier);
                                }
                            }
                        }

                        items[i] = new Item(
                            cur.id,
                            cur.name,
                            parts,
                            cur.tags.ToArray());
                    }
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                Item cur = items[i];
                if (cur == null) continue;
                if (cur.Id == id) return cur;
            }

            return null;
        }
    }
}