using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KVP = System.Collections.Generic.KeyValuePair<string, string>;

namespace Mentula.MMEditor
{
    public static class MMGenerator
    {
        public static string Generate(KVP[] values, string nameKey, string defKey, bool format = true)
        {
            StringBuilder sb = new StringBuilder();
            string addition = string.Empty;

            KVP nameAttr = values.FirstOrDefault(c => c.Key == nameKey);
            sb.Append('[').Append(nameAttr.Value).Append(format ? ": " : ":");

            KVP defAttr = values.FirstOrDefault(c => c.Key == defKey);
            if (IsStringType(defAttr)) sb.Append('"').Append(defAttr.Value).Append('"');
            else sb.Append(defAttr.Value);
            sb.Append(']');

            if (values.Length > 2)
            {
                sb.Append(format ? "\n{\n\t" : "{");
                for (int i = 0; i < values.Length - 2; i++)
                {
                    KVP cur = values[i];
                    if (cur.Key == defAttr.Key || cur.Key == nameAttr.Key) continue;

                    sb.AppendValue(cur, format).Append(',');
                    if (format && i + 1 < values.Length) sb.Append("\n\t");
                }
                sb.Append(format ? "\n}" : "}");
            }

            return sb.ToString();
        }

        private static StringBuilder AppendValue(this StringBuilder sb, KVP value, bool format)
        {
            sb.Append('[').Append(value.Key).Append(format ? ": " : ":");

            if (IsStringType(value)) sb.Append(value.Value);
            else sb.Append('"').Append(value.Value).Append('"');

            sb.Append(']');
            return sb;
        }

        private static bool IsStringType(KVP value)
        {
            decimal numeric;
            return decimal.TryParse(value.Value, out numeric);
        }
    }
}
