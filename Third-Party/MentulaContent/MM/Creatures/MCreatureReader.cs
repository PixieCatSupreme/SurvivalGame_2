using Microsoft.Xna.Framework.Content;

namespace Mentula.Content.MM.Creatures
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

                mani.Name = input.ReadCString();
                mani.TextureId = input.ReadInt32();
                mani.MaxHealth = input.ReadInt32();
                mani.Agility = input.ReadInt16();
                mani.Endurance = input.ReadInt16();
                mani.Intelect = input.ReadInt16();
                mani.Perception = input.ReadInt16();
                mani.Strength = input.ReadInt16();
                mani.IsAlive = input.ReadBoolean();

                if (mani.IsAlive)
                {
                    mani.Health = input.ReadInt32();
                    mani.Rotation = input.ReadSingle();
                    mani.ChunkPos = input.ReadIntVector2();
                    mani.Pos = input.ReadVector2();
                }

                result[i] = new Creature(
                    mani.Name, 
                    new Stats(
                        mani.Strength, 
                        mani.Intelect, 
                        mani.Endurance, 
                        mani.Agility, 
                        mani.Perception), 
                    mani.IsAlive ? mani.Health : mani.MaxHealth, 
                    mani.MaxHealth, 
                    mani.Pos, 
                    mani.ChunkPos);
            }

            return result;
        }
    }
}