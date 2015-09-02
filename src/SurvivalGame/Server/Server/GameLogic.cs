using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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
            for (int i = 0; i < index; i++)
            {
                if (Players[i].Key == id || Players[i].Value.Name == name) return true;
            }

            return false;
        }

        public Creature GetPlayer(long id)
        {
            for (int i = 0; i < index; i++)
            {
                if (Players[i].Key == id) return Players[i].Value;
            }

            return null;
        }

        public void AddPlayer(long id, string name)
        {
            if (index < Players.Length)
            {
                Players[index] = new KeyValuePair<long, Creature>(id, new Creature(name, new Vector2(), new IntVector2()));

                Map.Generate(Players[index].Value.ChunkPos);
                index++;
            }
        }

        public unsafe bool UpdatePlayer(long id, IntVector2* chunk, Vector2* tile)
        {
            bool legal = true;

            // Check if move possible.
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id) Players[i].Value.UpdatePos(chunk, tile);
            }

            return legal;
        }

        public void RemovePlayer(long id)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id)
                {
                    if (Players.Length == 1) Players[i] = new KeyValuePair<long, Creature>();
                    else Players[i] = Players[index - 1];
                }
            }
        }

        public void Update(float delta)
        {
            IntVector2[] posses = new IntVector2[Players.Length];

            for (int i = 0; i < index; i++)
            {
                posses[i] = Players[i].Value.ChunkPos;
                Map.LoadChunks(posses[i]);
            }

            Map.UnloadChunks(posses);
        }
    }
}