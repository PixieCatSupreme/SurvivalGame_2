using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Mentula.Content.MM
{
    [ContentTypeWriter]
    internal class MCreatureWriter : ContentTypeWriter<Creature[]>
    {
        protected override void Write(ContentWriter output, Creature[] value)
        {
            output.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                Creature cur = value[i];
                output.WriteString(cur.Name);
                output.Write(cur.TextureId);
                output.Write(cur.MaxHealth);
                output.Write(cur.Stats.Agility);
                output.Write(cur.Stats.Endurance);
                output.Write(cur.Stats.Intelect);
                output.Write(cur.Stats.Perception);
                output.Write(cur.Stats.Strength);
                output.Write(cur.IsAlive);

                if (cur.IsAlive)
                {
                    output.Write(cur.Health);
                    output.Write(cur.Rotation);
                    output.WriteIntVector2(cur.ChunkPos);
                    output.Write(cur.Pos);
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