using Mentula.Content.MM;

namespace Mentula.Content
{
    internal static partial class Utils
    {
        public static string GetStringValue(this Container cnt, string name)
        {
            string rawValue;
            if (cnt.TryGetValue(name, out rawValue)) return rawValue;
            else throw new ParameterNullException(name);
        }

        public static short GetInt16Value(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToInt16(name, rawValue);
        }

        public static int GetInt32Value(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToInt32(name, rawValue);
        }

        public static long GetInt64Value(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToInt64(name, rawValue);
        }

        public static ushort GetUInt16Value(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToUInt16(name, rawValue);
        }

        public static uint GetUInt32Value(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToUInt32(name, rawValue);
        }

        public static ulong GetUInt64Value(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToUInt64(name, rawValue);
        }

        public static float GetFloatValue(this Container cnt, string name)
        {
            string rawValue = cnt.GetStringValue(name);
            return ConvertToFloat(name, rawValue);
        }
    }
}