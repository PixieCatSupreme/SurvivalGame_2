using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Server;
using Mentula.Utilities;
using Mentula.Utilities.MathExtensions;

namespace Mentula.Server
{
    public class Map
    {
        public List<Chunk> LoadedChunks;
        public List<Chunk> ChunkList;
        public const int Range_S = 2;
        public const int Range_C = 1;

        public Map()
        {
            LoadedChunks = new List<Chunk>();
            ChunkList = new List<Chunk>();
        }

        public bool Generate(IntVector2 pos)
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
                        Chunk c = new Chunk(pos + new IntVector2(x, y));
                        ChunkGenerator.Generate(ref c);
                        LoadedChunks.Add(c);
                        ChunkList.Add(c);
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
        }

        public Chunk[] GetChunks(IntVector2 pos)
        {
            int c = 0;
            Chunk[] result = new Chunk[(Range_C * 2 + 1) * (Range_C * 2 + 1)];

            for (int y = -Range_C; y <= Range_C; y++)
            {
                for (int x = -Range_C; x <= Range_C; x++)
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

        public List<Chunk> GetChunks(IntVector2 oldPos, IntVector2 newPos)
        {
            List<Chunk> r = new List<Chunk>();
            for (int x = -Range_C; x <= Range_C; x++)
            {
                for (int y = -Range_C; y <= Range_C; y++)
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
                        if ((Math.Abs(ChunkList[i].ChunkPos.X - oldPos.X) > Range_C | Math.Abs(ChunkList[i].ChunkPos.Y - oldPos.Y) > Range_C) &
                            (Math.Abs(ChunkList[i].ChunkPos.X - newPos.X) <= Range_C & Math.Abs(ChunkList[i].ChunkPos.Y - newPos.Y) <= Range_C) &
                            !isloaded)
                        {
                            r.Add(ChunkList[i]);
                        }
                    }
                }
            }
            return r;
        }

        public void UnloadChunks(IntVector2 pos)
        {
            for (int i = 0; i < LoadedChunks.Count; )
            {
                if (Math.Abs(LoadedChunks[i].ChunkPos.X - pos.X) > Range_S | Math.Abs(LoadedChunks[i].ChunkPos.Y - pos.Y) > Range_S) LoadedChunks.RemoveAt(i);
                else i++;
            }
        }

        public void UnloadChunks(IntVector2[] pos)
        {
            for (int i = 0; i < LoadedChunks.Count; )
            {
                bool isnearplayer = false;
                for (int p = 0; p < pos.Length; p++)
                {
                    if (Math.Abs(LoadedChunks[i].ChunkPos.X - pos[p].X) <= Range_S & Math.Abs(LoadedChunks[i].ChunkPos.Y - pos[p].Y) <= Range_S) isnearplayer = true;
                }

                if (!isnearplayer) LoadedChunks.RemoveAt(i);
                else i++;
            }
        }



    }
}
