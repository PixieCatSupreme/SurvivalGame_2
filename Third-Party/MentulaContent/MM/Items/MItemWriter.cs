using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Collections.Generic;
using System.Linq;
using static Mentula.Content.MM.MItemProcessor;

namespace Mentula.Content.MM
{
    [ContentTypeWriter]
    internal class MItemWriter : ContentTypeWriter<Manifest[]>
    {
        protected override void Write(ContentWriter output, Manifest[] value)
        {
            output.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                Manifest cur = value[i];

                output.Write(cur.id);
                output.Write(cur.GetByteCount());
                output.WriteString(cur.name);

                if (cur.tags == null) output.Write((byte)0);
                else
                {
                    output.Write((byte)cur.tags.Count);
                    for (int j = 0; j < cur.tags.Count; j++)
                    {
                        Tag tag = cur.tags[j];
                        output.Write(tag.Key);
                        output.Write(tag.Value);
                    }
                }

                byte flags = (byte)(cur.IsBase ? 0x0 : 0x80);
                if (!cur.IsBase)
                {
                    flags |= (byte)cur.parts.Count;
                    output.Write(flags);

                    for (int j = 0; j < cur.parts.Count; j++)
                    {
                        KeyValuePair<ulong, KeyValuePair<ulong, ulong>[]> databank = cur.parts.ElementAt(j);
                        output.Write(databank.Key);
                        output.Write(databank.Value.Length);

                        for (int k = 0; k < databank.Value.Length; k++)
                        {
                            KeyValuePair<ulong, ulong> cell = databank.Value[k];
                            output.Write(cell.Key);
                            output.Write(cell.Value);
                        }
                    }
                }
                else
                {
                    output.Write(flags);
                    output.Write(cur.material.Key);
                    output.Write(cur.material.Value);
                }
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Manifest[]).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Mentula.Content.MM.MItemReader, MentulaContent, Version=1.0.0.0, Culture=neutral";
        }
    }
}