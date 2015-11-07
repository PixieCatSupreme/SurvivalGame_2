using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Mentula.Utilities
{
    public class Destructible
    {
        public int Id;
        public int Health;
        public int MaxHealth;
        public Vector2 Pos;
        public IntVector2 ChunkPos;
        public float Rotation;

        public Destructible(IntVector2 chunkPos, Vector2 pos, int id)
        {
            Id = id;
            Health = 100;
            MaxHealth = 100;
            Pos = pos;
            ChunkPos = chunkPos;
        }

        public float GetHealth()
        {
            float perc = MaxHealth * 0.01f;
            return Health * perc;
        }
    }
}
