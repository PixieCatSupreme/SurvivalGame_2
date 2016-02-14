using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mentula.Content.MM
{
    public static class Variables
    {
        public static Stack<ulong> IdBuffer = new Stack<ulong>();
    }

    internal class MItemReader : ContentTypeReader<ItemManifest>
    {
        protected override ItemManifest Read(ContentReader input, ItemManifest existingInstance)
        {
            int length = input.ReadInt32();
            ulong toFind = Variables.IdBuffer.Pop();

            for (int i = 0; i < length; i++)
            {
                ulong id = input.ReadUInt64();

                if (id == toFind)
                {
                    input.BaseStream.Seek(sizeof(ulong), SeekOrigin.Current);
                    string name = input.ReadCString();
                    ulong volume = input.ReadUInt64();

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

                    return new ItemManifest
                    {
                        id = id,
                        name = name,
                        volume = volume,
                        tags = tags.ToList(),
                        material = material,
                        parts = parts
                    };
                }
                else
                {
                    ulong byteLength = input.ReadUInt64();
                    input.BaseStream.Seek((long)byteLength, SeekOrigin.Current);
                }
            }

            throw new KeyNotFoundException("Cannot find specified item.");
        }
    }
}