using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Calculators
{
    public static class AbilityRangeCalculator
    {

        public static RangeValue<int> GetAbilityRangeDistance(IAbility ability, IGameUnit unit)
        {
            int min;
            int max;

            // Simple result:
            if(ability.RangeDistanceRule == AbilityRangeDistanceRule.Basic)
            {
                min = ability.RangeMinMax.minValue;
                max = ability.RangeMinMax.maxValue;
            }
            else if(ability.RangeDistanceRule == AbilityRangeDistanceRule.SizeRangeSmall)
            {
                min = ability.RangeMinMax.minValue;
                max = 1;
                switch(unit.SizeCategory)
                {
                    case SizeCategory.ExtraSmall:
                    case SizeCategory.Small: max = 1; break;
                    case SizeCategory.Medium: max = 2; break;
                    case SizeCategory.Large: max = 3; break;
                    case SizeCategory.Gigantic: max = 4; break;
                    case SizeCategory.GiganticSuper: max = 5; break;
                }

                max += min - 1;
            }
            else
            {
                // We hit an invalid state here
                throw new NotImplementedException(String.Format("RangeDistanceRule: {0} on ability: {1}, not implemented", ability.RangeDistanceRule, ability.IDName));
            }

            // Sanity check: Max can't be less than min
            if(max < min)
            {
                throw new Exception(String.Format("Got invalid min/max range values for ability: {0}. Min: {1}, Max: {2}. Rule: {3}", ability.IDName, min, max, ability.RangeDistanceRule));
            }

            return new RangeValue<int>(min, max);
        }
    }
}
