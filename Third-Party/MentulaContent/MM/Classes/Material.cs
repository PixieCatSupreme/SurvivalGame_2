using System.Diagnostics;
using System.Linq;

namespace Mentula.Content
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    [MMEditable]
    public abstract class Material
    {
        [MMIsDefault]
        public ulong Id { get; private set; }
        [MMIsName]
        public string Name { get; private set; }

        [AlternativeName("UTS")]
        public float Ultimate_Tensile_Strength { get; private set; }
        [AlternativeName("TSAY")]
        public float Tensile_Strain_At_Yield { get; private set; }
        public float Density { get; private set; }

        internal Material()
        {
            Id = 0;
            Name = "Unobtanium";
            Ultimate_Tensile_Strength = float.PositiveInfinity;
            Tensile_Strain_At_Yield = float.PositiveInfinity;
            Density = float.PositiveInfinity;
        }

        internal Material(ulong id, string name, float UTS, float TSAY, float density)
        {
            Id = id;
            Name = name;
            Ultimate_Tensile_Strength = UTS;
            Tensile_Strain_At_Yield = TSAY;
            Density = density;
        }

        internal Material(Material m)
        {
            Id = m.Id;
            Name = m.Name;
            Ultimate_Tensile_Strength = m.Ultimate_Tensile_Strength;
            Tensile_Strain_At_Yield = m.Tensile_Strain_At_Yield;
            Density = m.Density;
        }

        public void InitRefrence(Material[] dataset)
        {
            Material dataEntry = dataset.FirstOrDefault(m => m.Id == Id);

            if (dataEntry == null) return;

            Name = dataEntry.Name;
            Ultimate_Tensile_Strength = dataEntry.Ultimate_Tensile_Strength;
            Tensile_Strain_At_Yield = dataEntry.Tensile_Strain_At_Yield;
            Density = dataEntry.Density;
        }

        public override string ToString()
        {
            return $"Id={Id} Name={Name} UTS={Ultimate_Tensile_Strength} TSAY={Tensile_Strain_At_Yield} Dens={Density}";
        }
    }
}