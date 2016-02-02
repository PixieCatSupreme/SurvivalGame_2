using Mentula.Content;
using Mentula.Content.MM;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        public Creature GetCreature(string dataBase, ulong id)
        {
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
                foreach (KeyValuePair<int, string> db in dbKeys.Values)
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

            return new Creature(mani.id, mani.name, mani.textureId, mani.isBio, mani.stats, parts);
        }
    }
}