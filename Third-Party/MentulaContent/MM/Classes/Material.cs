using System.Diagnostics;
using System.Linq;

namespace Mentula.Content
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    [MMEditable]
    public abstract class Material
    {
        [MMIsDefault]
        public readonly ulong Id;
        [MMIsName]
        public readonly string Name;

        [AlternativeName("UTS")]
        public readonly float Ultimate_Tensile_Strength;
        [AlternativeName("TSAY")]
        public readonly float Tensile_Strain_At_Yield;
        public readonly float Density;

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

        public override string ToString()
        {
            return $"Id={Id} Name={Name} UTS={Ultimate_Tensile_Strength} TSAY={Tensile_Strain_At_Yield} Dens={Density}";
        }
    }
}