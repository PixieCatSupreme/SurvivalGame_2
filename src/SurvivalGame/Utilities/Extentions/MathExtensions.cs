using System;
using Microsoft.Xna.Framework;
using math = System.Math;
using Mentula.Utilities;
using Mentula.Engine.Core.ExtendedMath;
using Resc = Mentula.Utilities.Resources.Res;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Utilities.MathExtensions
{
    public static class MathEX
    {
        public static float Lerp(float Min, float Max, float amount)
        {
            return Min + (Max - Min) * amount;
        }

        public static float InvLerp(float Min, float Max, float value)
        {
            return (value - Min) / (Max - Min);
        }

        public static float GetMaxDiff(Vector2 a, Vector2 b)
        {
            return math.Max(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }

        public static float GetMaxDiff(IntVector2 a, IntVector2 b)
        {
            return math.Max(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }

        public static float GetMinDiff(Vector2 a, Vector2 b)
        {
            return math.Min(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }

        public static float GetMinDiff(IntVector2 a, IntVector2 b)
        {
            return math.Min(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }

        public static float DifferenceBetweenDegrees(float a, float b)
        {
            float x = math.Max(a, b);
            float y = math.Min(a, b);
            float c = math.Abs(x - y);

            if (c < 180) return c;

            return 360 - c;
        }

        public static float DifferenceBetweenRadians(float a, float b)
        {
            float x = math.Max(a, b);
            float y = math.Min(a, b);
            float c = math.Abs(x - y);
            if (c < MathF.PI) return c;

            return (float)(MathF.Tau) - c;
        }

        public static float VectorToDegrees(Vector2 v)
        {
            v.Normalize();
            return (float)math.Atan2(v.Y, v.X) * (float)(180f / math.PI);
        }

        public static float VectorToRadians(Vector2 v)
        {
            v.Normalize();
            return (float)math.Atan2(v.Y, v.X);
        }

        public static Vector2 RadiansToVector(float rad)
        {
            return new Vector2((float)math.Cos(rad), (float)math.Sin(rad));
        }

        public static Vector2 DegreesToVector(float deg)
        {
            return new Vector2((float)math.Cos(deg / 180 * math.PI), (float)math.Sin(deg / 180 * math.PI));
        }

        public static Vector2 Abs(Vector2 vec)
        {
            if (vec.X < 0) vec.X *= -1;
            if (vec.Y < 0) vec.Y *= -1;
            return vec;
        }

        public static int FloorAtBi(int N, int Base)
        {
            return (int)(math.Floor((float)N / Base) * Base);
        }

        public static int CeilAtBi(int N, int Base)
        {
            return (int)(math.Ceiling((float)N / Base) * Base);
        }

        public static byte ApplyPercentage(long value, ulong percentage)
        {
            float mltp = percentage / 100f;
            long result = (long)(value / mltp);

            if (result < 0) return 0;
            return (byte)result;
        }

        public static int ToPercentage(int value, int max)
        {
            float onePerc = max / 100f;
            return (int)(value / onePerc);
        }

        public static Vector2 FormatPos(this Vector2 tilePos)
        {
            while (tilePos.X < 0 | tilePos.Y < 0 | tilePos.X > Resc.ChunkSize | tilePos.Y > Resc.ChunkSize)
            {
                if (tilePos.X < 0) tilePos.X += Resc.ChunkSize;
                else if (tilePos.X > Resc.ChunkSize) tilePos.X -= Resc.ChunkSize;

                if (tilePos.Y < 0) tilePos.Y += Resc.ChunkSize;
                else if (tilePos.Y > Resc.ChunkSize) tilePos.Y -= Resc.ChunkSize;
            }

            return tilePos;
        }

        public static T[] Copy<T>(this IList<T> collection)
        {
            int count;

            if (collection.GetType() == typeof(T[])) count = ((T[])collection).Length;
            else count = collection.Count;

            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = collection[i];
            }

            return result;
        }

        public static void Shuffle<T>(this IList<T> collection)
        {
            int count;

            if (collection.GetType() == typeof(T[])) count = ((T[])collection).Length;
            else count = collection.Count;

            int[] indexes = new int[count];
            Random rng = new Random();

            for (int i = 0; i < count; i++)
            {
                int rnd;
                indexes[i] = -1;

                do
                {
                    rnd = rng.Next(0, count);
                } while (indexes.Contains(rnd));

                indexes[i] = rnd;
            }

            T[] temp = new T[count];
            for (int i = 0; i < count; i++)
            {
                temp[indexes[i]] = collection[i];
            }

            for (int i = 0; i < count; i++)
            {
                collection[i] = temp[i];
            }
        }

        public static Color ApplyAlpha(this Color c, byte a)
        {
            return new Color(c.R, c.G, c.B, a);
        }
    }
}