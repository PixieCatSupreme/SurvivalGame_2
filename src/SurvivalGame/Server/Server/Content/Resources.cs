﻿using Mentula.Content;
using Mentula.Content.MM;
using Mentula.Utilities.MathExtensions;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        private R dbKeys;
        public R Slots;

        private Metal[] metals;
        private Biomass[] biomasses;

        public Resources()
            : base(new ServiceContainer(), "Content")
        {
            dbKeys = Load<R>("DBKeys");
            Slots = Load<R>("Slots");
        }

        public override T Load<T>(string assetName)
        {
            Unload();

            try { return base.Load<T>(assetName); }
            catch (ContentLoadException) { return default(T); }
        }

        public Item GetItem(string dataBase, ulong id, ulong volumeModifier = 100)
        {
            Variables.IdBuffer.Push(id);
            ItemManifest mani = Load<ItemManifest>(dataBase);

            if (mani.IsBase)
            {
                Material material = null;

                switch (mani.material.Value)
                {
                    case (0):   // Metals
                        material = GetMetal(mani.material.Key);
                        break;
                    case (1):   // Biomass
                        material = GetBiomass(mani.material.Key);
                        break;
                }

                return new Item(
                    id,
                    mani.name,
                    material,
                    MathEX.ApplyPercentage((long)mani.volume, volumeModifier),
                    mani.tags.ToArray());
            }
            else
            {
                int prtsCount = 0;
                foreach (var database in mani.parts)
                {
                    prtsCount += database.Value.Length;
                }

                Item[] parts = new Item[prtsCount];
                int index = 0;
                foreach (var database in mani.parts)
                {
                    foreach (KeyValuePair<int, string> db in dbKeys)
                    {
                        if (database.Key == (ulong)db.Key)
                        {
                            for (int j = 0; j < database.Value.Length; j++)
                            {
                                KeyValuePair<ulong, ulong> part = database.Value[j];
                                parts[index++] = GetItem(db.Value, part.Key, MathEX.ApplyPercentage((long)part.Value, volumeModifier));
                            }
                        }
                    }
                }

                return new Item(
                    id,
                    mani.name,
                    parts,
                    mani.tags.ToArray());
            }
        }
    }
}