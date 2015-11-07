using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Content.MM.Creatures
{
    [ContentProcessor(DisplayName = "Mentula Creature Processor")]
    internal class MCreatureProcessor : ContentProcessor<MMSource, Creature[]>
    {
        public override Creature[] Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("Creatures", input.Container.Values["DEFAULT"]);

            Creature[] result = new Creature[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container cur = input.Container.Childs[i];
                Manifest mani = new Manifest() { IsAlive = true };
                string rawValue = "";

                const string ID = "Id";
                if (cur.TryGetValue(ID, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.TextureId = raw;
                    else throw new ParameterException(ID, rawValue, typeof(int));
                }
                else throw new ParameterNullException(ID);

                const string NAME = "Name";
                if (cur.TryGetValue(ID, out rawValue)) mani.Name = rawValue;
                else throw new ParameterNullException(NAME);

                const string MAXHEALTH = "MaxHealth";
                if (cur.TryGetValue(MAXHEALTH, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.MaxHealth = raw;
                    else throw new ParameterException(MAXHEALTH, rawValue, typeof(int));
                }
                else throw new ParameterNullException(MAXHEALTH);

                const string STRENGTH = "Strength";
                if (cur.TryGetValue(STRENGTH, out rawValue))
                {
                    short raw = 0;

                    if (short.TryParse(rawValue, out raw)) mani.Strength = raw;
                    else throw new ParameterException(STRENGTH, rawValue, typeof(short));
                }
                else throw new ParameterNullException(STRENGTH);

                const string AGILITY = "Agility";
                if (cur.TryGetValue(AGILITY, out rawValue))
                {
                    short raw = 0;

                    if (short.TryParse(rawValue, out raw)) mani.Agility = raw;
                    else throw new ParameterException(AGILITY, rawValue, typeof(short));
                }
                else throw new ParameterNullException(AGILITY);

                const string ENDURANCE = "Endurance";
                if (cur.TryGetValue(ENDURANCE, out rawValue))
                {
                    short raw = 0;

                    if (short.TryParse(rawValue, out raw)) mani.Endurance = raw;
                    else throw new ParameterException(ENDURANCE, rawValue, typeof(short));
                }
                else throw new ParameterNullException(ENDURANCE);

                const string INTELECT = "Intelect";
                if (cur.TryGetValue(INTELECT, out rawValue))
                {
                    short raw = 0;

                    if (short.TryParse(rawValue, out raw)) mani.Intelect = raw;
                    else throw new ParameterException(INTELECT, rawValue, typeof(short));
                }
                else throw new ParameterNullException(INTELECT);

                const string PERCEPTION = "Perception";
                if (cur.TryGetValue(PERCEPTION, out rawValue))
                {
                    short raw = 0;

                    if (short.TryParse(rawValue, out raw)) mani.Perception = raw;
                    else throw new ParameterException(PERCEPTION, rawValue, typeof(short));
                }
                else throw new ParameterNullException(PERCEPTION);

                const string HEALTH = "Health";
                if (cur.TryGetValue(HEALTH, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Health = raw;
                    else throw new ParameterException(HEALTH, rawValue, typeof(int));
                }
                else mani.IsAlive = false;

                const string ROTATION = "Rotation";
                if (cur.TryGetValue(ROTATION, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Rotation = raw;
                    else throw new ParameterException(ROTATION, rawValue, typeof(int));
                }
                else if (mani.IsAlive) throw new ParameterNullException(ROTATION);

                const string CHUNKPOSX = "ChunkPosition.X";
                if (cur.TryGetValue(CHUNKPOSX, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.ChunkPos.X = raw;
                    else throw new ParameterException(CHUNKPOSX, rawValue, typeof(int));
                }
                else if (mani.IsAlive) throw new ParameterNullException(CHUNKPOSX);

                const string CHUNKPOSY = "ChunkPosition.Y";
                if (cur.TryGetValue(CHUNKPOSY, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.ChunkPos.Y = raw;
                    else throw new ParameterException(CHUNKPOSY, rawValue, typeof(int));
                }
                else if (mani.IsAlive) throw new ParameterNullException(CHUNKPOSY);

                const string POSX = "Position.X";
                if (cur.TryGetValue(POSX, out rawValue))
                {
                    float raw = 0;

                    if (float.TryParse(rawValue, out raw)) mani.Pos.X = raw;
                    else throw new ParameterException(POSX, rawValue, typeof(float));
                }
                else if (mani.IsAlive) throw new ParameterNullException(POSX);

                const string POSY = "Position.Y";
                if (cur.TryGetValue(POSY, out rawValue))
                {
                    float raw = 0;

                    if (float.TryParse(rawValue, out raw)) mani.Pos.Y = raw;
                    else throw new ParameterException(POSY, rawValue, typeof(float));
                }
                else if (mani.IsAlive) throw new ParameterNullException(POSY);

                result[i] = new Creature(
                    mani.Name,
                    new Stats(
                        mani.Strength,
                        mani.Intelect,
                        mani.Endurance,
                        mani.Agility,
                        mani.Perception),
                    mani.MaxHealth,
                    mani.IsAlive ? mani.Health : mani.MaxHealth,
                    mani.IsAlive ? mani.Pos : Vector2.Zero,
                    mani.IsAlive ? mani.ChunkPos : IntVector2.Zero);
            }

            return result;
        }

        internal struct Manifest
        {
            public string Name;
            public int TextureId;
            public int Health;
            public int MaxHealth;
            public short Agility;
            public short Endurance;
            public short Intelect;
            public short Perception;
            public short Strength;
            public bool IsAlive;
            public float Rotation;
            public IntVector2 ChunkPos;
            public Vector2 Pos;
        }
    }
}