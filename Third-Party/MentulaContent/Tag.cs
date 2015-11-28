using System;
using System.Collections.Generic;

namespace Mentula.Content
{
    [MMEditable]
    public struct Tag
    {
        [MMIsName]
        public short Key;
        [MMIsDefault]
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

    public static class TagExtensions
    {
        public static bool FirstIsFalse(this IEnumerable<Tag> source, short key, Func<short, bool> selector)
        {
            foreach (Tag cur in source)
            {
                if (cur.Key == key) return selector(cur.Value);
            }

            return false;
        }

        public static bool FirstIsFalse(this IEnumerable<Tag> source, Func<short, bool> selector, params short[] keys)
        {
            bool[] result = new bool[keys.Length];

            foreach (Tag cur in source)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == cur.Key) result[i] = selector(cur.Value);
                }
            }

            for (int i = 0; i < result.Length; i++)
            {
                if (!result[i]) return false;
            }

            return true;
        }
    }
}