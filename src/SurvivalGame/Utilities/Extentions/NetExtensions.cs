using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Utilities.Net
{
    public static class NetExtensions
    {
        public static IntVector2 ReadPoint(this NetBuffer msg)
        {
            return new IntVector2(msg.ReadInt32(), msg.ReadInt32());
        }

        public static unsafe void Write(this NetBuffer msg, IntVector2* point)
        {
            msg.Write(point->X);
            msg.Write(point->Y);
        }

        public static Tile[] ReadTileArr(this NetBuffer msg)
        {
            ushort length = msg.ReadUInt16();
            Tile[] result = new Tile[length];

            for (int i = 0; i < length; i++)
            {
                int texture = msg.ReadInt32();
                IntVector2 pos = msg.ReadPoint();
                result[i] = new Tile(texture, pos);
            }

            return result;
        }

        public static void Write(this NetBuffer msg, Tile[] value)
        {
            msg.Write((ushort)value.Length);

            for(int i = 0; i < value.Length; i++)
            {
                Tile cur = value[i];

                msg.Write(cur.Tex);
                msg.Write(cur.Pos.X);
                msg.Write(cur.Pos.Y);
            }
        }
    }
}