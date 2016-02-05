using Mentula.Content;
using Mentula.Content.MM;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        private List<Creature> cachesCreatures = new List<Creature>();

        public Creature GetCreature(string dataBase, ulong id, bool cache, string name = null)
        {
            for (int i = 0; i < cachesCreatures.Count; i++)
            {
                Creature cur = cachesCreatures[i];
                if (cur.Id == id) return cur;
            }

            Variables.IdBuffer.Push(id);
            CreatureManifest mani = Load<CreatureManifest>(dataBase);

            int prtsCount = 0;
            foreach (var db in mani.parts)
            {
                prtsCount += db.Value.Length;
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
                            parts[index++] = GetItem(db.Value, part.Key, part.Value);
                        }
                    }
                }
            }


            Creature c = new Creature(mani.id, !string.IsNullOrEmpty(name) ? name : mani.name, mani.textureId, mani.isBio, mani.stats, parts);
            if (cache) cachesCreatures.Add(c);

            return c;
        }
    }
}