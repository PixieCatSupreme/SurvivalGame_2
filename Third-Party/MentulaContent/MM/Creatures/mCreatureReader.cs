using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Mentula.Content.MM
{
    internal class MCreatureReader : ContentTypeReader<Creature[]>
    {
        protected override Creature[] Read(ContentReader input, Creature[] existingInstance)
        {
            int length = input.ReadInt32();
            Creature[] result = new Creature[length];

            for (int i = 0; i < length; i++)
            {
                MCreatureProcessor.Manifest mani = new MCreatureProcessor.Manifest();
                mani.name = input.ReadCString();

                mani.agility = input.ReadInt16();
                mani.endurance = input.ReadInt16();
                mani.intelect = input.ReadInt16();
                mani.perception = input.ReadInt16();
                mani.strength = input.ReadInt16();

                mani.textId = input.ReadInt32();
                Creature cur = new Creature(mani.name, new Stats(
                    mani.strength,
                    mani.intelect,
                    mani.endurance,
                    mani.agility, mani.perception),
                    mani.textId);

                cur.IsBio = input.ReadBoolean();
                //cur.Parts = new Item[input.ReadInt32()];
            }

            return result;
        }
    }
}