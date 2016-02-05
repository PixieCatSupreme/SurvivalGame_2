using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content.MM
{
    public class R : Dictionary<int, string>
    {
        internal R(KeyValuePair<string, KeyValuePair<int, string>[]>[] raw)
        {
            for (int i = 0; i < raw.Length; i++)
            {
                KeyValuePair<string, KeyValuePair<int, string>[]> iA = raw.ElementAt(i);

                for (int j = 0; j < iA.Value.Length; j++)
                {
                    KeyValuePair<int, string> item = iA.Value[j];

                    if (ContainsKey(item.Key))
                    {
                        throw new ContainerException(iA.Key,  new BuildException($"{item.Key} has already been added!"));
                    }

                    Add(item.Key, $"{iA.Key}/{item.Value}");
                }
            }
        }

        public int GetTagId(string name)
        {
            foreach (KeyValuePair<int, string> cur in this)
            {
                if (cur.Value == name) return cur.Key;
            }

            return -1;
        }
    }
}