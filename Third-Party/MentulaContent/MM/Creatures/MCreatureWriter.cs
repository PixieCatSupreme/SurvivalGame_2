using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Mentula.Content.MM
{
    [ContentTypeWriter]
    internal class MCreatureWriter : ContentTypeWriter<CreatureManifest[]>
    {
        protected override void Write(ContentWriter output, CreatureManifest[] value)
        {
            output.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                CreatureManifest cur = value[i];

                output.Write(cur.id);
                output.Write(cur.GetByteCount());
                output.WriteString(cur.name);
                output.Write(cur.textureId);
                output.Write(cur.isBio);

                output.Write(cur.stats.Strength);
                output.Write(cur.stats.Intelect);
                output.Write(cur.stats.Endurance);
                output.Write(cur.stats.Agility);
                output.Write(cur.stats.Perception);

                output.Write(cur.parts.Count);
                foreach (var database in cur.parts)
                {
                    output.Write(database.Key);
                    output.Write(database.Value.Length);

                    for (int k = 0; k < database.Value.Length; k++)
                    {
                        output.Write(database.Value[k].Key);
                        output.Write(database.Value[k].Value);
                    }
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