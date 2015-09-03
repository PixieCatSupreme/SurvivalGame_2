using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Utilities;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;

namespace Mentula.Server
{
    public static class ChunkGenerator
    {
        private static int cSize = Res.ChunkSize;
        private static float[] rainArray;


        public static Chunk Generate(ref Chunk chunk)
        {
            GenerateTerrain(ref chunk);

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
                chunk.Tiles[i]= new Tile(ChT(rainArray[i]),new IntVector2(i%cSize,i/cSize));
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
    }
}
