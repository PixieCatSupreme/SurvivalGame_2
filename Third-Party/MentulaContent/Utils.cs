using Mentula.Content.MM;
using Mentula.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mentula.Content
{
    internal static class Utils
    {
        private static readonly CultureInfo usInfo = CultureInfo.CreateSpecificCulture("en-US");

        public static void WriteString(this ContentWriter cw, string value)
        {
            byte[] enc = Encoding.UTF8.GetBytes(value);
            cw.Write(enc.Length);
            cw.Write(enc);
        }

        public static string ReadCString(this ContentReader cr)
        {
            int length = cr.ReadInt32();
            byte[] enc = cr.ReadBytes(length);
            return Encoding.UTF8.GetString(enc);
        }

        public static void WriteIntVector2(this ContentWriter cw, IntVector2 value)
        {
            cw.Write(value.X);
            cw.Write(value.Y);
        }

        public static IntVector2 ReadIntVector2(this ContentReader cr)
        {
            IntVector2 result;
            result.X = cr.ReadInt32();
            result.Y = cr.ReadInt32();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckProcessorType(string type, string request)
        {
            if (type.ToUpper() != request.ToUpper()) throw new ArgumentException("Wrong processor type selected this= '" + type + "', needed='" + request + "'");
        }

        public static bool TryParse(string s, out float result)
        {
            return float.TryParse(s, NumberStyles.Number, usInfo, out result);
        }
    }
}