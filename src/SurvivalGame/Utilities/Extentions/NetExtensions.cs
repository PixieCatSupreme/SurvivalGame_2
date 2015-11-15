using Lidgren.Network;

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

        public static void Write(this NetBuffer msg, IntVector2 point)
        {
            msg.Write(point.X);
            msg.Write(point.Y);
        }
    }
}