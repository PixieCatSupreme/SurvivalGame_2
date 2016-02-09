using System;
using System.Collections.Generic;
using Mentula.Utilities;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using static Mentula.Utilities.Resources.Res;

namespace Mentula.Server
{
    public class Map
    {
        public List<Chunk> LoadedChunks;
        private List<Chunk> ChunkList;
        public List<NPC> LoadedNPCs;
        public List<NPC> LoadedDeadNPCs;
        private List<MegaChunk> MegaChunks;


        public const int Range_S = Res.Range_C + 1;

        public Map()
        {
            LoadedChunks = new List<Chunk>();
            ChunkList = new List<Chunk>();
            LoadedNPCs = new List<NPC>();
            MegaChunks = new List<MegaChunk>();
            MegaChunks.Add(new MegaChunk(new IntVector2(0, 0)));
        }

        public bool Generate(IntVector2 pos, Resources content)
        {
            bool gen = false;

            for (int y = -Range_S; y <= Range_S; y++)
            {
                for (int x = -Range_S; x <= Range_S; x++)
                {
                    bool chunkexists = false;

                    for (int i = 0; i < ChunkList.Count; i++)
                    {
                        if (ChunkList[i].ChunkPos.X == x + pos.X & ChunkList[i].ChunkPos.Y == y + pos.Y)
                        {
                            chunkexists = true;
                            break;
                        }
                    }
                    if (!chunkexists)
                    {
                        List<NPC> n = new List<NPC>();
                        Chunk c = new Chunk(pos + new IntVector2(x, y));
                        ChunkGenerator.GenerateTerrain(ref c);
                        List<Structure> s = GenerateStructures(ref c);
                        ChunkGenerator.GenerateTrees(ref c, s);
                        ChunkGenerator.GenerateWildlife(ref n, ref c, s, content);
                        LoadedChunks.Add(c);
                        ChunkList.Add(c);

                        LoadedNPCs.InsertRange(LoadedNPCs.Count, n);
                        gen = true;
                    }
                }
            }
            return gen;
        }

        public void LoadChunks(IntVector2 pos)
        {
            for (int y = -Range_S; y <= Range_S; y++)
            {
                for (int x = -Range_S; x <= Range_S; x++)
                {
                    for (int i = 0; i < ChunkList.Count; i++)
                    {
                        if (ChunkList[i].ChunkPos.X == x + pos.X & ChunkList[i].ChunkPos.Y == y + pos.Y)
                        {
                            bool chunkisloaded = false;

                            for (int j = 0; j < LoadedChunks.Count; j++)
                            {
                                if (LoadedChunks[j] == ChunkList[i]) chunkisloaded = true;
                            }

                            if (!chunkisloaded) LoadedChunks.Add(ChunkList[i]);
                        }
                    }
                }
            }
            List<NPC> n = new List<NPC>();

            LoadedNPCs.InsertRange(LoadedNPCs.Count, n);
        }

        public Chunk[] GetChunks(IntVector2 pos)
        {
            int c = 0;
            Chunk[] result = new Chunk[(Res.Range_C * 2 + 1) * (Res.Range_C * 2 + 1)];

            for (int y = -Res.Range_C; y <= Res.Range_C; y++)
            {
                for (int x = -Res.Range_C; x <= Res.Range_C; x++)
                {
                    for (int i = 0; i < LoadedChunks.Count; i++)
                    {
                        if (LoadedChunks[i].ChunkPos.X == x + pos.X && LoadedChunks[i].ChunkPos.Y == y + pos.Y)
                        {
                            result[c] = LoadedChunks[i];
                            c++;
                        }
                    }
                }
            }

            return result;
        }

        public Chunk[] GetChunks(IntVector2 oldPos, IntVector2 newPos)
        {
            List<Chunk> r = new List<Chunk>();

            for (int x = -Res.Range_C; x <= Res.Range_C; x++)
            {
                for (int y = -Res.Range_C; y <= Res.Range_C; y++)
                {
                    for (int i = 0; i < ChunkList.Count; i++)
                    {
                        bool isloaded = false;
                        for (int j = 0; j < r.Count; j++)
                        {
                            if (r[j] == ChunkList[i]) isloaded = true;
                        }
                        //if its not next to the old tilePos
                        //and it is next to the new tilePos
                        //and it is not already loaded
                        if ((Math.Abs(ChunkList[i].ChunkPos.X - oldPos.X) > Res.Range_C | Math.Abs(ChunkList[i].ChunkPos.Y - oldPos.Y) > Res.Range_C) &
                            (Math.Abs(ChunkList[i].ChunkPos.X - newPos.X) <= Res.Range_C & Math.Abs(ChunkList[i].ChunkPos.Y - newPos.Y) <= Res.Range_C) &
                            !isloaded)
                        {
                            r.Add(ChunkList[i]);
                        }
                    }
                }
            }

            return r.ToArray();
        }

        public NPC[] GetNPC(IntVector2 oldpos, IntVector2 newpos)
        {
            List<NPC> n = new List<NPC>();
            for (int i = 0; i < LoadedNPCs.Count; i++)
            {
                if (MathEX.GetMaxDiff(LoadedNPCs[i].ChunkPos, oldpos) > Res.Range_C)
                {
                    if (MathEX.GetMaxDiff(LoadedNPCs[i].ChunkPos, newpos) <= Res.Range_C)
                    {
                        n.Add(LoadedNPCs[i]);
                    }
                }

            }
            return n.ToArray();
        }

        public NPC[] GetNPC(IntVector2 pos)
        {
            List<NPC> n = new List<NPC>();
            for (int i = 0; i < LoadedNPCs.Count; i++)
            {
                if (MathEX.GetMaxDiff(LoadedNPCs[i].ChunkPos, pos) <= Res.Range_C)
                {
                    n.Add(LoadedNPCs[i]);
                }
            }
            return n.ToArray();
        }

        public NPC[] GetDeadNPC(IntVector2 oldpos, IntVector2 newpos)
        {
            List<NPC> n = new List<NPC>();
            for (int i = 0; i < LoadedDeadNPCs.Count; i++)
            {
                if (MathEX.GetMaxDiff(LoadedDeadNPCs[i].ChunkPos, oldpos) > Res.Range_C)
                {
                    if (MathEX.GetMaxDiff(LoadedDeadNPCs[i].ChunkPos, newpos) <= Res.Range_C)
                    {
                        n.Add(LoadedDeadNPCs[i]);
                    }
                }

            }
            return n.ToArray();
        }

        public NPC[] GetDeadNPC(IntVector2 pos)
        {
            List<NPC> n = new List<NPC>();
            for (int i = 0; i < LoadedDeadNPCs.Count; i++)
            {
                if (MathEX.GetMaxDiff(LoadedDeadNPCs[i].ChunkPos, pos) <= Res.Range_C)
                {
                    n.Add(LoadedDeadNPCs[i]);
                }
            }
            return n.ToArray();
        }

        public void UnloadChunks(IntVector2 pos)
        {
            for (int i = 0; i < LoadedChunks.Count;)
            {
                if (Math.Abs(LoadedChunks[i].ChunkPos.X - pos.X) > Range_S | Math.Abs(LoadedChunks[i].ChunkPos.Y - pos.Y) > Range_S) LoadedChunks.RemoveAt(i);
                else i++;
            }
        }

        public void UnloadChunks(IntVector2[] pos)
        {
            for (int i = 0; i < LoadedChunks.Count;)
            {
                bool isnearplayer = false;
                for (int p = 0; p < pos.Length; p++)
                {
                    if (Math.Abs(LoadedChunks[i].ChunkPos.X - pos[p].X) <= Range_S && Math.Abs(LoadedChunks[i].ChunkPos.Y - pos[p].Y) <= Range_S) isnearplayer = true;
                }

                if (!isnearplayer) LoadedChunks.RemoveAt(i);
                else i++;
            }
        }

        private List<Structure> GenerateStructures(ref Chunk c)
        {
            List<Structure> result = new List<Structure>();
            IntVector2 mcp = new IntVector2(c.ChunkPos.X / MegaChunkSize, c.ChunkPos.Y / MegaChunkSize);
            MegaChunk m = null;
            Rectangle chunkSpace = new Rectangle(c.ChunkPos.X * ChunkSize, c.ChunkPos.Y * ChunkSize, ChunkSize, ChunkSize);
            int cx = c.ChunkPos.X * ChunkSize;
            int cy = c.ChunkPos.Y * ChunkSize;

            for (int i = 0; i < MegaChunks.Count; i++)
            {
                if (mcp == MegaChunks[i].Pos)
                {
                    m = MegaChunks[i];
                }
            }

            if (m == null)
            {
                m = new MegaChunk(mcp);
                MegaChunks.Add(m);
            }

            for (int i = 0; i < m.Structures.Count; i++)
            {
                if (!Rectangle.Intersect(m.Structures[i].Space, chunkSpace).IsEmpty)
                {
                    Structure s = m.Structures[i];
                    result.Add(s);
                    int sx = s.Space.X + m.Pos.X * MegaChunkSize;
                    int sy = s.Space.Y + m.Pos.Y * MegaChunkSize;

                    for (int j = 0; j < s.Tiles.Count; j++)
                    {
                        int x = sx + s.Tiles[j].Pos.X - cx;
                        int y = sy + s.Tiles[j].Pos.Y - cy;
                        int indexv = x + y * ChunkSize;

                        if (x >= 0 && x < ChunkSize && y >= 0 && y < ChunkSize)
                        {

                            c.Tiles[indexv] = new Tile(s.Tiles[j].Tex, new IntVector2(x, y));
                        }
                    }

                    for (int j = 0; j < s.Destructibles.Count; j++)
                    {
                        int x = sx + (int)s.Destructibles[j].Pos.X - cx;
                        int y = sy + (int)s.Destructibles[j].Pos.Y - cy;
                        int indexv = x + y * ChunkSize;

                        if (x >= 0 && x < ChunkSize && y >= 0 && y < ChunkSize)
                        {
                            c.Destructibles.Add(new Destructible(c.ChunkPos, new Vector2(x, y), s.Destructibles[j].Id));
                        }
                    }
                }
            }
            return result;
        }

    }
}
