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
}