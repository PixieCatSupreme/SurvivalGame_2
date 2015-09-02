using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Mentula.Server
{
    public class GameLogic
    {
        public Map Map { get; private set; }
        public KeyValuePair<long, Creature>[] Players;

        private int index;

        public GameLogic()
        {
            Map = new Map();
            Players = new KeyValuePair<long, Creature>[Res.MaxPlayers];
            index = 0;
        }

        public bool PlayerExists(long id, string name)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id || Players[i].Creature.Name == name) return true;
            }

            return false;
        }

        public Creature GetPlayer(long id)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id) return Players[i].Value;
            }

            return string.Empty;
        }

        public void AddPlayer(long id, string name)
        {
            if (index < Players.Length)
            {
                Players[index] = new KeyValuePair<long, string>(id, new Creature(name, new Stats(), 100));
                index++;

                Map.Generate(IntVector2.Zero);
            }
        }

        public bool UpdatePlayer(long id, ref IntVector2 chunk, ref Vector2 tile)
        {
            bool legal = true;

            // Check if move possible.
            // Update player pos.

            return legal;
        }

        public void RemovePlayer(long id)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id)
                {
                    if (Players.Length == 1) Players[i] = new KeyValuePair<long,string>();
                    else Players[i] = Players[index - 1];
                }
            }
        }

        public void Update(float delta)
        {
        }
    }
}