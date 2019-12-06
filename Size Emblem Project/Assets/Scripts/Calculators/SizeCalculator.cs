using SizeEmblem.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Calculators
{

    /// <summary>
    /// Calculator class for most things related to the SizeCategory of a unit
    /// </summary>
    public static class SizeCalculator
    {

        public static int GetSizeDifference(SizeCategory a, SizeCategory b)
        {
            return (int)a - (int)b;
        }

        public static int GetMaxRangeBonusForSize(SizeCategory size)
        {
            switch (size)
            {
                case SizeCategory.ExtraSmall:
                case SizeCategory.Small:         return 1;
                case SizeCategory.Medium:        return 2;
                case SizeCategory.Large:         return 3;
                case SizeCategory.Gigantic:      return 4;
                case SizeCategory.GiganticSuper: return 5;
                default: return 1;
            }
        }
    }
}
