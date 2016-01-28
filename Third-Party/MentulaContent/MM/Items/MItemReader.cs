using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mentula.Content.MM.MItemProcessor;

namespace Mentula.Content.MM
{
    internal class MItemReader : ContentTypeReader<Manifest[]>
    {
        protected override Manifest[] Read(ContentReader input, Manifest[] existingInstance)
        {
            int length = input.ReadInt32();
            Manifest[] result = new Manifest[length];

            for (int i = 0; i < length; i++)
            {
                ulong id = input.ReadUInt64();
                input.ReadUInt64();                     // Read byte count, only used for retro loading.
                string name = input.ReadCString();

                byte tagLength = input.ReadByte();
                Tag[] tags = new Tag[tagLength];
                for (int j = 0; j < tagLength; j++)
                {
                    tags[j] = new Tag(input.ReadInt16(), input.ReadInt16());
                }

                byte flags = input.ReadByte();

                Dictionary<ulong, KeyValuePair<ulong, ulong>[]> parts = new Dictionary<ulong, KeyValuePair<ulong, ulong>[]>();
                KeyValuePair<ulong, ulong> material = new KeyValuePair<ulong, ulong>();
                if ((flags & 0x80) == 0x80)    
                {
                    byte partLength = (byte)(flags & 0x7F);
                    for (int j = 0; j < partLength; j++)
                    {
                        ulong dbId = input.ReadUInt64();
                        int dbLength = input.ReadInt32();

                        KeyValuePair<ulong, ulong>[] dataBank = new KeyValuePair<ulong, ulong>[dbLength];
                        for (int k = 0; k < dbLength; k++)
                        {
                            dataBank[k] = new KeyValuePair<ulong, ulong>(
                                input.ReadUInt64(),
                                input.ReadUInt64());
                        }

                        parts.Add(dbId, dataBank);
                    }
                }
                else
                {
                    material = new KeyValuePair<ulong, ulong>(
                        input.ReadUInt64(),
                        input.ReadUInt64());
                }

                result[i] = new Manifest
                {
                    id = id,
                    name = name,
                    volume = 0,
                    tags = tags.ToList(),
                    material = material,
                    parts = parts
                };
            }

            return result;
        }
    }
}