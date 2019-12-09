using SizeEmblem.Assets.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Containers
{
    public struct HeightContainer
    {
        public readonly double Height;
        public readonly HeightUnit Metric;

        public HeightContainer(double height, HeightUnit metric)
        {
            Height = height;
            Metric = metric;
        }

        public new string ToString()
        {
            return $"{Height:N0} {Metric}";
        }
    }

    public struct WeightContainer
    {
        public readonly double Weight;
        public readonly WeightUnit Metric;

        public WeightContainer(double weight, WeightUnit metric)
        {
            Weight = weight;
            Metric = metric;
        }

        public new string ToString()
        {
            return $"{Weight:N0} {Metric}";
        }
    }
}
