using System;
using System.Collections.Generic;
using Mentula.Utilities;
using Microsoft.Xna.Framework;
using static Mentula.Utilities.Resources.Res;

namespace Mentula.Server
{
    public static class ChunkGenerator
    {
        private static float[] rainArray;




        public static void GenerateTerrain(ref Chunk chunk)
        {
            chunk.Tiles = new Tile[ChunkSize * ChunkSize];
            rainArray = new float[ChunkSize * ChunkSize];
            for (int i = 0; i < ChunkSize * ChunkSize; i++)
            {
                int x = i % ChunkSize + chunk.ChunkPos.X * ChunkSize;
                int y = i / ChunkSize + chunk.ChunkPos.Y * ChunkSize;
                rainArray[i] =
                PerlinNoise.Generate(10, ChunkSize / 4, x, y) +
                PerlinNoise.Generate(30, ChunkSize * 2, x, y) +
                PerlinNoise.Generate(60, ChunkSize * 16, x, y);
                chunk.Tiles[i] = new Tile(ChT(rainArray[i]), new IntVector2(i % ChunkSize, i / ChunkSize));
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
            Random r = new Random(RNG.RIntFromString(c.ChunkPos.X + Seed + c.ChunkPos.Y));
            for (int i = 0; i < r.NextDouble() * 10;)
            {
                bool canplace = true;
                Vector2 p = new Vector2((int)(r.NextDouble() * ChunkSize), (int)(r.NextDouble() * ChunkSize));
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
                    if (c.Tiles[(int)p.X + (int)p.Y * ChunkSize].Tex != 4)
                    {
                        //n.Add(new NPC("Wolf", new Stats(7), 35, p, c.ChunkPos) { TextureId = 9996 });
                        //n[i].creature.Rotation = (float)(r.NextDouble() * Math.PI * 2);
                        i++;
                    }

                }

            }

        }

        public static void GenerateTrees(ref Chunk c, List<Structure> s)
        {
            Random r = new Random(RNG.RIntFromString(c.ChunkPos.X + "x" + c.ChunkPos.Y));
            for (int i = 0; i < ChunkSize * ChunkSize; i++)
            {
                float chance = rainArray[i] / 1000;
                if (r.NextDouble() < chance)
                {
                    Vector2 pos = new Vector2(i % ChunkSize, i / ChunkSize);
                    bool canplace = true;
                    for (int j = 0; j < s.Count; j++)
                    {
                        Rectangle rect = new Rectangle(s[j].Space.X - c.ChunkPos.X * ChunkSize, s[j].Space.Y - c.ChunkPos.Y * ChunkSize, s[j].Space.Width, s[j].Space.Height);
                        if (!Rectangle.Intersect(rect, new Rectangle((int)pos.X, (int)pos.Y, 1, 1)).IsEmpty)
                        {
                            canplace = false;
                        }
                    }
                    if (canplace)
                    {
                        for (int j = 0; j < c.Destructibles.Count; j++)
                        {
                            if (c.Destructibles[j].Pos == pos)
                            {
                                canplace = false;
                            }
                        }
                    }
                    if (canplace)
                    {
                        if (c.Tiles[(int)pos.X + (int)pos.Y * ChunkSize].Tex != 4)
                        {
                            c.Destructibles.Add(new Destructible(c.ChunkPos, pos, 500));
                        }
                    }
                }
            }
        }

    }
}
