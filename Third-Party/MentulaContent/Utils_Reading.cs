using Mentula.Content.MM;
using Mentula.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mentula.Content
{
    internal static partial class Utils
    {
        public static void WriteKey(this ContentWriter cw, byte[] key)
        {
            cw.Write(key.Length);
            cw.Write(key);
        }

        public static byte[] ReadKey(this ContentReader cr)
        {
            int length = cr.ReadInt32();
            return cr.ReadBytes(length);
        }

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
        public static void CheckProcessorType(string type, MMSource source)
        {
            string request;
            if (!source.Container.TryGetValue("name", out request))
            {
                if (!source.Container.TryGetValue("default", out request))
                {
                    throw new ArgumentException("No processor type found.")
                        .AsContainerException(source.Container.Name);
                }
            }

            if (type.ToUpper() != request.ToUpper())
            {
                throw new ArgumentException($"Wrong processor type selected this= '{type}', needed='{request}'")
                    .AsContainerException(source.Container.Name);
            }
        }
    }
}