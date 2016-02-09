using Mentula.Content;
using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Mentula.Server
{
    public class GameLogic
    {
        public Map Map;
        public KeyValuePair<long, Creature>[] Players;

        public int Index { get; private set; }

        private Server server;
        private Resources content;
        private int NPCClock;

        public GameLogic(Server server)
        {
            Map = new Map();
            Players = new KeyValuePair<long, Creature>[Res.MaxPlayers];
            content = new Resources();
            Index = 0;
            NPCClock = 0;
            this.server = server;
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

        public Creature AddPlayer(long id, string name)
        {
            if (Index < Players.Length)
            {
                Players[Index] = new KeyValuePair<long, Creature>(id, content.GetCreature("Databases/Creatures", 0, name));
                Players[Index].Value.Pos = new Vector2(1523, 2166);
                Players[Index].Value.FormatPos();
                Map.Generate(Players[Index].Value.ChunkPos, content);

                return Players[Index++].Value;
            }

            return null;
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
                    if (Index <= 1) Players[i] = new KeyValuePair<long, Creature>();
                    else Players[i] = Players[Index - 1];
                    if (Index > 0) Index--;
                }
            }
        }

        public void Update(float delta)
        {
            IntVector2[] posses = new IntVector2[Players.Length];

            for (int i = 0; i < Index; i++)
            {
                posses[i] = Players[i].Value.ChunkPos;
                Map.Generate(posses[i], content);
                Map.LoadChunks(posses[i]);
            }

            Map.UnloadChunks(posses);
            if (NPCClock == 64)
            {
                NPCClock = 0;
            }
            else
            {
                NPCClock++;
            }
            if (Index > 0)
            {
                for (int i = 0; i < Map.LoadedNPCs.Count; i += 64)
                {

                }
            }
            for (int i = 0; i < Map.LoadedNPCs.Count; i++)
            {
                Map.LoadedNPCs[i].WalkPath(delta);
            }
        }

        public void PlayerAttack(long id)
        {
            int index = 0;
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Key == id)
                {
                    index = i;
                }
            }
            Combat.OnMelee(Players[index].Value, ref Map.LoadedNPCs, ref Map.LoadedDeadNPCs);
        }
    }
}