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
        private static IntVector2 lastPos;
        private static perlinpoint[] points;

        public static Chunk Generate(ref Chunk chunk)
        {
            GenerateTerrain(ref chunk);

            return chunk;
        }

        private static void GenerateTerrain(ref Chunk chunk)
        {
            chunk.Tiles = new Tile[cSize * cSize];

            if (chunk.ChunkPos != lastPos)
            {
                generatePerlinPoints(chunk.ChunkPos);
                lastPos = chunk.ChunkPos;
            }

            Vector2 pos = chunk.ChunkPos.ToVector2();
            double chunkR = 0;
            for (int i = 0; i < cSize * cSize; i++)
            {
                double rain = 0;
                double w = 0;
                for (int j = 0; j < points.Length; j++)
                {
                    float d = Vector2.Distance(points[j].pos, pos);
                    double weight = 1 / Math.Pow(d, d / 1000) * points[j].w;
                    rain += points[j].val * weight;
                    w += weight;
                }
                rain /= w;
                chunkR += rain;
                chunk.Tiles[i] = new Tile(ChT(rain), new IntVector2(i % cSize, i / cSize));
            }

            chunk.Rain = (float)chunkR / (cSize * cSize);
        }

        private static int ChT(double rain)
        {
            int n = -1;
            if (rain < 0.25)
            {
                n = 0;
            }
            else if (rain < 0.5)
            {
                n = 1;
            }
            else if (rain < 0.75)
            {
                n = 2;
            }
            else if (rain < 1)
            {
                n = 3;
            }
            return n;
        }

        private static void generatePerlinPoints(IntVector2 pos)
        {
            int x = pos.X * cSize;
            int y = pos.Y * cSize;
            int a0 = cSize * 16;
            int a1 = cSize * 4;
            int a2 = cSize;

            points = new perlinpoint[12];

            points[0] = new perlinpoint(new Vector2(MathEX.FloorAtBi(x, a0), MathEX.FloorAtBi(y, a0)), RNG.RFloatFromString(MathEX.FloorAtBi(x, a0), Res.Seed, MathEX.FloorAtBi(y, a0)), 4);
            points[1] = new perlinpoint(new Vector2(MathEX.FloorAtBi(x, a0), MathEX.CeilAtBi(y, a0)), RNG.RFloatFromString(MathEX.FloorAtBi(x, a0), Res.Seed, MathEX.CeilAtBi(y, a0)), 4);
            points[2] = new perlinpoint(new Vector2(MathEX.CeilAtBi(x, a0), MathEX.FloorAtBi(y, a0)), RNG.RFloatFromString(MathEX.CeilAtBi(x, a0), Res.Seed, MathEX.FloorAtBi(y, a0)), 4);
            points[3] = new perlinpoint(new Vector2(MathEX.CeilAtBi(x, a0), MathEX.CeilAtBi(y, a0)), RNG.RFloatFromString(MathEX.CeilAtBi(x, a0), Res.Seed, MathEX.CeilAtBi(y, a0)), 4);

            points[4] = new perlinpoint(new Vector2(MathEX.FloorAtBi(x, a1), MathEX.FloorAtBi(y, a1)), RNG.RFloatFromString(MathEX.FloorAtBi(x, a1), Res.Seed, MathEX.FloorAtBi(y, a1)), 2);
            points[5] = new perlinpoint(new Vector2(MathEX.FloorAtBi(x, a1), MathEX.CeilAtBi(y, a1)), RNG.RFloatFromString(MathEX.FloorAtBi(x, a1), Res.Seed, MathEX.CeilAtBi(y, a1)), 2);
            points[6] = new perlinpoint(new Vector2(MathEX.CeilAtBi(x, a1), MathEX.FloorAtBi(y, a1)), RNG.RFloatFromString(MathEX.CeilAtBi(x, a1), Res.Seed, MathEX.FloorAtBi(y, a1)), 2);
            points[7] = new perlinpoint(new Vector2(MathEX.CeilAtBi(x, a1), MathEX.CeilAtBi(y, a1)), RNG.RFloatFromString(MathEX.CeilAtBi(x, a1), Res.Seed, MathEX.CeilAtBi(y, a1)), 2);

            points[8] = new perlinpoint(new Vector2(MathEX.FloorAtBi(x, a2), MathEX.FloorAtBi(y, a2)), RNG.RFloatFromString(MathEX.FloorAtBi(x, a2), Res.Seed, MathEX.FloorAtBi(y, a2)), 1);
            points[9] = new perlinpoint(new Vector2(MathEX.FloorAtBi(x, a2), MathEX.CeilAtBi(y, a2)), RNG.RFloatFromString(MathEX.FloorAtBi(x, a2), Res.Seed, MathEX.CeilAtBi(y, a2)), 1);
            points[10] = new perlinpoint(new Vector2(MathEX.CeilAtBi(x, a2), MathEX.FloorAtBi(y, a2)), RNG.RFloatFromString(MathEX.CeilAtBi(x, a2), Res.Seed, MathEX.FloorAtBi(y, a2)), 1);
            points[11] = new perlinpoint(new Vector2(MathEX.CeilAtBi(x, a2), MathEX.CeilAtBi(y, a2)), RNG.RFloatFromString(MathEX.CeilAtBi(x, a2), Res.Seed, MathEX.CeilAtBi(y, a2)), 1);

        }

        private class perlinpoint
        {
            public Vector2 pos;
            public float val;
            public float w;

            public perlinpoint()
            {
            }

            public perlinpoint(Vector2 p, float v, float weight)
            {
                pos = p;
                val = v;
                w = weight;
            }
        }
    }
}
