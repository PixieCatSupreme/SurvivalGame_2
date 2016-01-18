using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Mentula.Content;
using System.Diagnostics;
using System.Threading;

namespace Mentula.MMEditor
{
    public partial class MainWindow : Form
    {
        private const int TAB = 50;

        private Type[] Items;
        private int yOffset;
        private int xOffset;
        private string name;
        private string def;

        public MainWindow()
        {
            InitializeComponent();
            SetItems();
            AddComboBox(0);
        }

        public void OnSelectionChanged(object sender, EventArgs e)
        {
            yOffset = 0;
            xOffset = 0;
            int index = ((ComboBox)sender).SelectedIndex;

            Controls.Clear();
            AddComboBox(index);
            AddMembers(index);
            AddBtnGenerate();
        }

        public void OnGenerate(object sender, EventArgs e)
        {
            ErrorProvider.Clear();
            int count = (Controls.Count - 2) >> 1;

            KeyValuePair<Control, string>[] values = new KeyValuePair<Control, string>[count];

            for (int i = 0, j = 0; i < values.Length && j < Controls.Count; j++)
            {
                TextBox value = Controls[j] as TextBox;
                if (value == null) continue;

                Label key = Controls[j - 1] as Label;
                values[i++] = new KeyValuePair<Control, string>(key, value.Text);
            }

            bool generate = true;
            for (int i = 0; i < values.Length; i++)
            {
                KeyValuePair<Control, string> cur = values[i];
                string keyName = cur.Key.Text.Replace("*", string.Empty);

                if (cur.Key.Text[cur.Key.Text.Length - 1] == '*' &&
                    string.IsNullOrWhiteSpace(cur.Value))
                {
                    ErrorProvider.SetError(cur.Key, $"{keyName} is required!");
                    generate = false;
                    continue;
                }
            }

            if (!generate) return;

            KeyValuePair<string, string>[] pairs = new KeyValuePair<string, string>[values.Length];
            for (int i = 0; i < pairs.Length; i++)
            {
                pairs[i] = new KeyValuePair<string, string>(values[i].Key.Text.Replace("*", string.Empty), values[i].Value);
            }
            string text = MMGenerator.Generate(pairs, name, def);
        }

        private void AddMembers(int index)
        {
            Type type = Items[index];

            MemberInfo[] members = GetMembers(type);
            RemoveIgnore(ref members);
            SetNameDefault(ref members);
            AddAttributes(CheckMembers(ref members));
        }

        private void SetNameDefault(ref MemberInfo[] members)
        {
            name = string.Empty;
            def = string.Empty;

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo cur = members[i];

                if (cur.GetCustomAttribute(typeof(MMIsName)) != null)
                {
                    if (string.IsNullOrEmpty(name)) name = cur.Name;
                    else throw new ArgumentException("Class has multiple names!");
                }
                else if (cur.GetCustomAttribute(typeof(MMIsDefault)) != null)
                {
                    if (string.IsNullOrEmpty(def)) def = cur.Name;
                    else throw new ArgumentException("Class has multiple defaults!");
                }
            }
        }

        private void AddAttributes(KeyValuePair<MemberInfo, MemberOptions>[] attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                AddAttribute(ref attributes[i]);
            }
        }

        private void AddAttribute(ref KeyValuePair<MemberInfo, MemberOptions> attribute)
        {
            if ((attribute.Value & MemberOptions.IsCustomType) != 0)
            {
                Type underLying;
                if (GetUnderLyingType(attribute.Key, out underLying))
                {
                    AddInputBox(attribute, false);
                    MemberInfo[] members = GetMembers(underLying);
                    RemoveIgnore(ref members);

                    xOffset += TAB;
                    AddAttributes(CheckMembers(ref members));
                    xOffset -= TAB;
                }
            }
            else AddInputBox(attribute);
        }

        private void AddInputBox(KeyValuePair<MemberInfo, MemberOptions> attribute, bool hasInput = true)
        {
            AlternativeName newName = attribute.Key.GetCustomAttribute(typeof(AlternativeName)) as AlternativeName;

            string name = newName == null ? attribute.Key.Name : newName.NewName;
            bool optional = (attribute.Value & MemberOptions.IsOptional) != 0;

            Label label = new Label() { Text = name, Location = new Point(xOffset, yOffset += 25) };
            if (!optional && hasInput) label.Text += '*';
            else if (!hasInput) label.Font = new Font(label.Font, FontStyle.Bold);
            Controls.Add(label);

            if (hasInput)
            {
                TextBox input = new TextBox() { Location = new Point(label.Location.X + label.Width, yOffset) };
                label.AutoSize = true;
                Controls.Add(input);
            }
        }

        private void AddComboBox(int index)
        {
            ComboBox combo = new ComboBox();
            combo.Items.AddRange(Items.Select(t => t.Name).ToArray());
            combo.SelectedIndex = index;
            combo.SelectedIndexChanged += OnSelectionChanged;
            Controls.Add(combo);
        }

        private void AddBtnGenerate()
        {
            Button btn = new Button
            {
                Location = new Point(xOffset += TAB, yOffset += TAB),
                Text = "Generate"
            };
            btn.Click += OnGenerate;

            Controls.Add(btn);
        }

        private void SetItems()
        {
            Items = Assembly.GetAssembly(typeof(MMEditable))
                .GetTypes()
                .Where(t => t.GetCustomAttribute(typeof(MMEditable)) != null)
                .ToArray();
        }

        private MemberInfo[] GetMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            return type.GetFields(flags).Cast<MemberInfo>()
                    .Concat(type.GetProperties(flags).Cast<MemberInfo>()).ToArray();
        }

        private void RemoveIgnore(ref MemberInfo[] members)
        {
            bool[] keep = new bool[members.Length];

            for (int i = 0; i < members.Length; i++)
            {
                keep[i] = members[i].GetCustomAttribute(typeof(MMIgnore)) == null;
            }

            int count = keep.Count(m => m);
            MemberInfo[] result = new MemberInfo[count];

            for (int i = 0, j = 0; i < members.Length; i++)
            {
                if (keep[i]) result[j++] = members[i];
            }

            members = result;
        }

        private bool GetUnderLyingType(MemberInfo member, out Type underLying)
        {
            Type memberType = null;

            FieldInfo info = member as FieldInfo;
            if (info != null) memberType = info.FieldType;

            if (memberType == null)
            {
                PropertyInfo propInfo = member as PropertyInfo;
                if (propInfo != null) memberType = propInfo.PropertyType;
            }

            if (memberType != null && memberType.GetCustomAttribute(typeof(MMEditable)) != null)
            {
                underLying = memberType;
                return true;
            }

            underLying = null;
            return false;
        }

        private KeyValuePair<MemberInfo, MemberOptions>[] CheckMembers(ref MemberInfo[] members)
        {
            KeyValuePair<MemberInfo, MemberOptions>[] result = new KeyValuePair<MemberInfo, MemberOptions>[members.Length];

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo current = members[i];
                MemberOptions options = MemberOptions.Default;

                if (current.GetCustomAttribute(typeof(MMOptional)) != null) options |= MemberOptions.IsOptional;

                Type type = null;

                FieldInfo info = current as FieldInfo;
                if (info != null) type = info.FieldType;

                if (type == null)
                {
                    PropertyInfo propInfo = current as PropertyInfo;
                    if (propInfo != null) type = propInfo.PropertyType;
                }

                if (type.IsArray) options |= MemberOptions.IsArray;
                if (type.GetCustomAttribute(typeof(MMEditable)) != null) options |= MemberOptions.IsCustomType;

                result[i] = new KeyValuePair<MemberInfo, MemberOptions>(current, options);
            }

            return result;
        }
    }
}