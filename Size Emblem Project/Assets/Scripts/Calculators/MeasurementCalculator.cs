using SizeEmblem.Assets.Scripts.Constants;
using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Calculators
{
    public static class MeasurementCalculator
    {

        public static HeightContainer GetHeightContainer(double baseHeight)
        {
            // Find the best unit for this height


            var heightContainer = new HeightContainer(baseHeight, HeightUnit.Feet);
            return heightContainer;
        }



        public static WeightContainer GetWeightContainer(double baseWeight)
        {
            var weightContainer = new WeightContainer(baseWeight, WeightUnit.Pounds);
            return weightContainer;
        }

        public static WeightContainer GetScaledWeightContainer(double baseWeight, double height)
        {
            // We need to scale our weight based on the provided height. We assume all base weights are 
            var scale = height / 5.5;
            var scaledWeight = baseWeight * Math.Pow(scale, 3);
            return GetWeightContainer(scaledWeight);
        }
    }
}
