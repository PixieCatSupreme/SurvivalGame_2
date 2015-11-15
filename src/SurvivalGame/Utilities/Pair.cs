﻿namespace Mentula.Utilities
{
    public struct Pair<Tkey, TValue>
    {
        public Tkey Key;
        public TValue Value;

        public Pair(Tkey key)
        {
            Key = key;
            Value = default(TValue);
        }

        public Pair(Tkey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString() 
        {
            return Key.ToString() + ": " + Value.ToString();
        }
    }

    public struct Tag
    {
        public short Key;
        public short Value;

        public Tag(short key)
        {
            Key = key;
            Value = 100;
        }

        public Tag(short key, short value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return Key + ": " + Value.ToString();
        }

        public static Tag operator +(Tag l, Tag r)
        {
            return new Tag(l.Key, (short)(l.Value + r.Value));
        }
    }
}