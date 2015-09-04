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

        public int Index { get; private set; }

        public GameLogic()
        {
            Map = new Map();
            Players = new KeyValuePair<long, Creature>[Res.MaxPlayers];
            Index = 0;
        }

        public bool PlayerExists(long id, string name)
        {
            for (int i = 0; i < Index; i++)
            {
                if (Players[i].Key == id || Players[i].Value.Name == name) return true;
            }

            return false;
        }

        public Creature GetPlayer(long id)
        {
            for (int i = 0; i < Index; i++)
            {
                if (Players[i].Key == id) return Players[i].Value;
            }

            return null;
        }

        public void AddPlayer(long id, string name)
        {
            if (Index < Players.Length)
            {
                Players[Index] = new KeyValuePair<long, Creature>(id, new Creature(name, new Vector2(), new IntVector2()));

                Map.Generate(Players[Index].Value.ChunkPos);
                Index++;
            }
        }

        public unsafe bool UpdatePlayer(long id, IntVector2* chunk, Vector2* tile, float rotation)
        {
            bool legal = true;

            // Check if move possible.
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id)
                {
                    Players[i].Value.UpdatePos(chunk, tile);
                    Players[i].Value.Rotation = rotation;
                }
            }

            return legal;
        }

        public void RemovePlayer(long id)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id)
                {
                    if (Index == 1) Players[i] = new KeyValuePair<long, Creature>();
                    else Players[i] = Players[Index - 1];
                    Index--;
                }
            }
        }

        public void Update(float delta)
        {
            IntVector2[] posses = new IntVector2[Players.Length];

            for (int i = 0; i < Index; i++)
            {
                posses[i] = Players[i].Value.ChunkPos;
                Map.Generate(posses[i]);
                Map.LoadChunks(posses[i]);
            }

            Map.UnloadChunks(posses);
        }

        public void PlayerAttack(long id)
        {
            Creature[] players = Players.Select(p => p.Value).ToArray();
            Creature attacker = GetPlayer(id);
            Combat.OnAttack(ref players, ref attacker, 1, 1);
            for (int i = 0; i < players.Length; i++)
            {
                Players[i].Value.Health = players[i].Health;
            }
        }

    }
}