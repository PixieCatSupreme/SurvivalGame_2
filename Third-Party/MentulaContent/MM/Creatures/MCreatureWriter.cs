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