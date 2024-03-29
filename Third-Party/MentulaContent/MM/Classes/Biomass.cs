﻿using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    [MMEditable]
    public class Biomass : Material
    {
        public readonly float BurnTemperature;
        public readonly float NutritiousValue;

        internal Biomass()
        {
            BurnTemperature = float.PositiveInfinity;
            NutritiousValue = float.PositiveInfinity;
        }

        internal Biomass(float burn, float nutr, ulong id, string name, Vector3 values)
            : base(id, name, values.X, values.Y, values.Z)
        {
            BurnTemperature = burn;
            NutritiousValue = nutr;
        }

        public override string ToString()
        {
            return $"BurnT={BurnTemperature} NutrV={NutritiousValue} Material={base.ToString()}";
        }
    }
}
