using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Mentula.Content.MM
{
    [ContentTypeWriter]
    internal class MCreatureWriter : ContentTypeWriter<Creature[]>
    {
        protected override void Write(ContentWriter output, Creature[] value)
        {
            output.Write(value.Length); //TODO: ToHeaderKey;

            for (int i = 0; i < value.Length; i++)
            {
                Creature cur = value[i];
                output.WriteString(cur.Name);

                output.Write(cur.Stats.Agility);
                output.Write(cur.Stats.Endurance);
                output.Write(cur.Stats.Intelect);
                output.Write(cur.Stats.Perception);
                output.Write(cur.Stats.Strength);

                output.Write(cur.TextureId);
                output.Write(cur.IsBio);

                output.Write(cur.Parts.Length);
                for (int j = 0; j < cur.Parts.Length; j++)
                {
                    Item item = cur.Parts[i];
                    output.WriteKey(item.Key);
                    output.Write(item.Volume);
                }
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Creature[]).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Mentula.Content.MM.MCreatureReader, MentulaContent, Version=1.0.0.0, Culture=neutral";
        }
    }
}