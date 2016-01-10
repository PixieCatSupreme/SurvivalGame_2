using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;

namespace Mentula.Utilities.Udp
{
    public class UdpBuffer
    {
        public const int MAX_SIZE = 1024;

        public byte[] Buffer { get; set; }
        public int Length { get; set; }
        public int Position { get; set; }

        private static readonly IndexOutOfRangeException r = new IndexOutOfRangeException("Cannot read outside of bufffer.");
        private static readonly OverflowException w = new OverflowException("Cannot write outside of buffer.");

        public UdpBuffer()
        {
            Buffer = new byte[MAX_SIZE];
        }

        public bool ReadBool()
        {
            if (Position + 1 < Length) return Convert.ToBoolean(Buffer[++Position]);
            throw r;
        }

        public byte ReadByte()
        {
            if (Position + 1 < Length) return Buffer[++Position];
            throw r;
        }

        public double ReadDouble()
        {
            const int BYTES = sizeof(double); // 8

            if (Position + BYTES < Length) return BitConverter.ToDouble(Read(BYTES), 0);
            throw r;
        }

        public float ReadFloat()
        {
            const int BYTES = sizeof(float); // 4

            if (Position + BYTES < Length) return BitConverter.ToSingle(Read(BYTES), 0);
            throw r;
        }

        public float ReadHalfPrecision()
        {
            return new HalfSingle { PackedValue = ReadUInt16() }.ToSingle();
        }

        public short ReadInt16()
        {
            const int BYTES = sizeof(short); // 2

            if (Position + BYTES < Length) return BitConverter.ToInt16(Read(BYTES), 0);
            throw r;
        }

        public int ReadInt32()
        {
            const int BYTES = sizeof(int); // 4

            if (Position + BYTES < Length) return BitConverter.ToInt32(Read(BYTES), 0);
            throw r;
        }

        public long ReadInt64()
        {
            const int BYTES = sizeof(long); // 8

            if (Position + BYTES < Length) return BitConverter.ToInt64(Read(BYTES), 0);
            throw r;
        }

        public sbyte ReadSByte()
        {
            if (Position + 1 < Length) return (sbyte)Buffer[++Position];
            throw r;
        }

        public ushort ReadUInt16()
        {
            const int BYTES = sizeof(ushort); // 2

            if (Position + BYTES < Length) return BitConverter.ToUInt16(Read(BYTES), 0);
            throw r;
        }

        public uint ReadUInt32()
        {
            const int BYTES = sizeof(uint); // 4

            if (Position + BYTES < Length) return BitConverter.ToUInt32(Read(BYTES), 0);
            throw r;
        }

        public ulong ReadUInt64()
        {
            const int BYTES = sizeof(ulong); // 8

            if (Position + BYTES < Length) return BitConverter.ToUInt64(Read(BYTES), 0);
            throw r;
        }

        public Vector2 ReadVector2()
        {
            Vector2 val;
            val.X = ReadFloat();
            val.Y = ReadFloat();
            return val;
        }

        public BytePoint ReadBytePoint()
        {
            BytePoint val;
            val.X = ReadByte();
            val.Y = ReadByte();
            return val;
        }

        public IntVector2 ReadIntPoint()
        {
            IntVector2 val;
            val.X = ReadInt32();
            val.Y = ReadInt32();
            return val;
        }

        public void Write(bool value)
        {
            if (Length + 1 < MAX_SIZE) Buffer[++Length] = Convert.ToByte(value);
            else throw w;
        }

        public void Write(byte value)
        {
            if (Length + 1 < MAX_SIZE) Buffer[++Length] = value;
            else throw w;
        }

        public void Write(double value)
        {
            if (Length + sizeof(double) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(float value)
        {
            if (Length + sizeof(float) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(short value)
        {
            if (Length + sizeof(short) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(int value)
        {
            if (Length + sizeof(int) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }
        
        public void Write(long value)
        {
            if (Length + sizeof(long) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(sbyte value)
        {
            if (Length + sizeof(sbyte) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(ushort value)
        {
            if (Length + sizeof(ushort) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(uint value)
        {
            if (Length + sizeof(uint) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(ulong value)
        {
            if (Length + sizeof(ulong) < MAX_SIZE) Write(BitConverter.GetBytes(value));
            else throw w;
        }

        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public unsafe void Write(Vector2 *value)
        {
            Write(value->X);
            Write(value->Y);
        }

        public void Write(BytePoint value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public unsafe void Write(BytePoint *value)
        {
            Write(value->X);
            Write(value->Y);
        }

        public void Write(IntVector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public unsafe void Write(IntVector2 *value)
        {
            Write(value->X);
            Write(value->Y);
        }

        public void WriteHalfPrecision(float value)
        {
            Write(new HalfSingle(value).PackedValue);
        }

        private byte[] Read(int length)
        {
            byte[] value = new byte[length];
            Array.Copy(Buffer, Position, value, 0, length);
            Position += length;
            return value;
        }

        private void Write(byte[] bytes)
        {
            Array.Copy(bytes, 0, Buffer, Length, bytes.Length);
            Length += bytes.Length;
        }
    }
}