using System;
using System.Globalization;

namespace Mentula.Content
{
    internal static partial class Utils
    {
        private static readonly CultureInfo usInfo = CultureInfo.CreateSpecificCulture("en-US");

        public static ContainerException AsContainerException(this Exception e, string name)
        {
            return new ContainerException(name, e);
        }

        public static bool TryParse(string s, out float result)
        {
            return float.TryParse(s, NumberStyles.Number, usInfo, out result);
        }

        public static short ConvertToInt16(string name, string value)
        {
            short result;
            if (short.TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(short));
        }

        public static int ConvertToInt32(string name, string value)
        {
            int result;
            if (int.TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(int));
        }

        public static long ConvertToInt64(string name, string value)
        {
            long result;
            if (long.TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(long));
        }

        public static ushort ConvertToUInt16(string name, string value)
        {
            ushort result;
            if (ushort.TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(ushort));
        }

        public static uint ConvertToUInt32(string name, string value)
        {
            uint result;
            if (uint.TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(uint));
        }

        public static ulong ConvertToUInt64(string name, string value)
        {
            ulong result;
            if (ulong.TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(ulong));
        }

        public static float ConvertToFloat(string name, string value)
        {
            float result;
            if (TryParse(value, out result)) return result;
            else throw new ParameterException(name, value, typeof(float));
        }
    }
}