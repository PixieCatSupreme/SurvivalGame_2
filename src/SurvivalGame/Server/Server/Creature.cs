using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Server
{
    public class Creature
    {
        public string Name;
        public Stats Stats;
        public int[] Health;
        public int[] MaxHealth;

        public Creature()
        {
            Name = "The  Unnameable";
            Stats = new Stats();
            Health = new int[6] { 0, 0, 0, 0, 0, 0 };
            MaxHealth = new int[6] { 0, 0, 0, 0, 0, 0 };
        }

        public Creature(string n,Stats s, int h)
        {
            Name = n;
            Stats = s;
            Health = new int[6] { h, h, h, h, h, h };
            MaxHealth = new int[6] { h, h, h, h, h, h };
        }
    }
}
