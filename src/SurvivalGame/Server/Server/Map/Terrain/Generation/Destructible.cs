using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Mentula.Utilities
{
    public class Destructible : Actor
    {
        public int Id;
        public int Health;
        public int MaxHealth;

        public Destructible(IntVector2 chunkPos,Vector2 pos,int id)
            :base(pos,chunkPos)
        {
            Id = id;
            Health = 100;
            MaxHealth = 100;
        }


    }
}
