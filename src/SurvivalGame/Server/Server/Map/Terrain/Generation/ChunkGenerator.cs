using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Mentula.Content;

namespace Mentula.Server
{
    public static class ChunkGenerator
    {
        private static int cSize = Res.ChunkSize;
        private static float[] rainArray;


        public static Chunk Generate(ref Chunk chunk, ref List<NPC> n)
        {
            GenerateTerrain(ref chunk);
            GenerateLakes(ref chunk);
            GenerateTrees(ref chunk);
            GenerateWildlife(ref n, ref chunk);

            return chunk;
        }

        private static void GenerateTerrain(ref Chunk chunk)
        {
            chunk.Tiles = new Tile[cSize * cSize];
            rainArray = new float[cSize * cSize];
            for (int i = 0; i < cSize * cSize; i++)
            {
                int x = i % cSize + chunk.ChunkPos.X * cSize;
                int y = i / cSize + chunk.ChunkPos.Y * cSize;
                rainArray[i] =
                PerlinNoise.Generate(10, cSize / 4, x, y) +
                PerlinNoise.Generate(30, cSize * 2, x, y) +
                PerlinNoise.Generate(60, cSize * 16, x, y);
                chunk.Tiles[i] = new Tile(ChT(rainArray[i]), new IntVector2(i % cSize, i / cSize));
            }
        }

        private static int ChT(double rain)
        {
            int n = -1;
            if (rain < 25)
            {
                n = 0;
            }
            else if (rain < 50)
            {
                n = 1;
            }
            else if (rain < 75)
            {
                n = 2;
            }
            else if (rain < 100)
            {
                n = 3;
            }
            return n;
        }

        private static void GenerateWildlife(ref List<NPC> n, ref Chunk c)
        {
            Random r = new Random(RNG.RIntFromString(c.ChunkPos.X + Res.Seed + c.ChunkPos.Y));
            for (int i = 0; i < r.NextDouble() * 10; )
            {
                bool canplace = true;
                Vector2 p = new Vector2((int)(r.NextDouble() * Res.ChunkSize), (int)(r.NextDouble() * Res.ChunkSize));
                for (int j = 0; j < n.Count; j++)
                {
                    if (p == n[j].creature.Pos)
                    {
                        canplace = false;
                    }
                }
                for (int j = 0; j < c.Destructibles.Count; j++)
                {
                    if (p == c.Destructibles[j].Pos)
                    {
                        canplace = false;
                    }
                }


                if (canplace)
                {
                    if (c.Tiles[(int)p.X + (int)p.Y * cSize].Tex != 4)
                    {
                        //n.Add(new NPC("Wolf", new Stats(7), 35, p, c.ChunkPos) { TextureId = 9996 });
                        //n[i].creature.Rotation = (float)(r.NextDouble() * Math.PI * 2);
                        i++;
                    }

                }

            }

        }

        private static void GenerateTrees(ref Chunk c)
        {
            Random r = new Random(RNG.RIntFromString(c.ChunkPos.X + "x" + c.ChunkPos.Y));
            for (int i = 0; i < Res.ChunkSize * Res.ChunkSize; i++)
            {
                float chance = rainArray[i] / 1000;
                if (r.NextDouble() < chance)
                {
                    Vector2 pos = new Vector2(i % Res.ChunkSize, i / Res.ChunkSize);
                    bool canplace = true;
                    for (int j = 0; j < c.Destructibles.Count; j++)
                    {
                        if (c.Destructibles[j].Pos == pos)
                        {
                            canplace = false;
                        }
                    }

                    if (canplace)
                    {
                        if (c.Tiles[(int)pos.X + (int)pos.Y * cSize].Tex != 4)
                        {
                            c.Destructibles.Add(new Destructible(c.ChunkPos, pos, 500));
                        }
                    }
                }
            }
        }

        private static void GenerateLakes(ref Chunk c)
        {
            Random r = new Random(RNG.RIntFromString(c.ChunkPos.X + "x" + c.ChunkPos.Y));

            if (r.NextDouble() < 0.1)
            {
                IntVector2 startingPos=new IntVector2(cSize/2-1);
                c.Tiles[startingPos.X + startingPos.Y * 32].Tex = 4;
                Vector2 p = startingPos;
                for (int i = 0; i < 512; i++)
                {

                    Vector2 a = new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f);
                    a.Normalize();
                    if (Vector2.Distance(p, startingPos) < 15)
                    {
                        p += a;
                        for (int x = 0; x <= 1; x++)
                        {
                            for (int y = 0; y <= 1; y++)
                            {
                                c.Tiles[(int)p.X + x + (int)(p.Y+y) * 32].Tex = 4;
                            }
                        }


                    }
                    else
                    {
                        p = startingPos;
                    }
                }
            }
        }

    }
}
