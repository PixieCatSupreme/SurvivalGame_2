using Mentula.Utilities;
using System;
using System.Collections.Generic;
using Mentula.Utilities.MathExtensions;
using Mentula.Utilities.Resources;

namespace Mentula.Server
{
    public static class PerlinNoise
    {
        private static Dictionary<string, float> NoiseDict = new Dictionary<string, float>();

        public static float Generate(float weight, float frequency, float x, float y)
        {
            float xLow = (float)Math.Floor(x / frequency) * frequency;
            float xHigh = (float)Math.Ceiling(x / frequency) * frequency;
            float yLow = (float)Math.Floor(y / frequency) * frequency;
            float yHigh = (float)Math.Ceiling(y / frequency) * frequency;
            float x0y0 = GetNoise(xLow, yLow);
            float x0y1 = GetNoise(xLow, yHigh);
            float x1y0 = GetNoise(xHigh, yLow);
            float x1y1 = GetNoise(xHigh, yHigh);
            float x0Noise;
            float x1Noise;
            if (yLow == yHigh)
            {
                x0Noise = x0y0;
                x1Noise = x1y0;
            }
            else
            {
                x0Noise = MathEX.Lerp(x0y0, x0y1, MathEX.InvLerp(yLow, yHigh, y));
                x1Noise = MathEX.Lerp(x1y0, x1y1, MathEX.InvLerp(yLow, yHigh, y));
            }
            if (x0Noise == x1Noise)
            {
                return x0Noise * weight;
            }
            else
            {
                return MathEX.Lerp(x0Noise, x1Noise, MathEX.InvLerp(xLow, xHigh, x)) * weight;
            }
        }

        private static float GetNoise(float x, float y)
        {
            float n;
            string s = x.ToString() + Res.Seed + y.ToString();

            if (NoiseDict.Count > 4096)
            {
                NoiseDict = new Dictionary<string, float>();
            }

            if (NoiseDict.ContainsKey(s))
            {
                NoiseDict.TryGetValue(s, out n);
                return n;
            }
            else
            {
                n = RNG.RFloatFromString(s);
                NoiseDict.Add(s, n);
                return n;
            }
        }
    }
}