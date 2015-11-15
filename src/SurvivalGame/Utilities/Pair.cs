namespace Mentula.Utilities
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
}