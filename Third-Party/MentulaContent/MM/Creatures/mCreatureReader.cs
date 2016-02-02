using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Mentula.Content.MM
{
    internal class MCreatureReader : ContentTypeReader<CreatureManifest>
    {
        protected override CreatureManifest Read(ContentReader input, CreatureManifest existingInstance)
        {
            int length = input.ReadInt32();
            ulong toFind = Variables.IdBuffer.Pop();

            for (int i = 0; i < length; i++)
            {
                ulong id = input.ReadUInt64();

                if (id == toFind)
                {
                    input.ReadUInt64();
                    string name = input.ReadCString();
                    int textureId = input.ReadInt32();
                    bool isBio = input.ReadBoolean();

                    Stats stats = new Stats(
                        input.ReadInt16(),
                        input.ReadInt16(),
                        input.ReadInt16(),
                        input.ReadInt16(),
                        input.ReadInt16());

                    Dictionary<ulong, KeyValuePair<ulong, ulong>[]> parts = new Dictionary<ulong, KeyValuePair<ulong, ulong>[]>();
                    int partCount = input.ReadInt32();

                    for (int j = 0; j < partCount; j++)
                    {
                        ulong dbKey = input.ReadUInt64();
                        KeyValuePair<ulong, ulong>[] cells = new KeyValuePair<ulong, ulong>[input.ReadInt32()];

                        for (int k = 0; k < cells.Length; k++)
                        {
                            cells[k] = new KeyValuePair<ulong, ulong>(
                                input.ReadUInt64(),
                                input.ReadUInt64());
                        }

                        parts.Add(dbKey, cells);
                    }

                    return new CreatureManifest
                    {
                        id = id,
                        name = name,
                        textureId = textureId,
                        isBio = isBio,
                        stats = stats,
                        parts = parts
                    };
                }
                else
                {
                    ulong byteLength = input.ReadUInt64();
                    input.BaseStream.Position += (long)byteLength;
                }
            }

            throw new KeyNotFoundException("Cannot find specified creature!");
        }
    }
}