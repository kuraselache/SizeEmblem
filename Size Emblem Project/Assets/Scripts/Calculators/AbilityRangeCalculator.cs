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

        /// <summary>
        /// Get the Min/Max range of an ability for a given unit.
        /// This will check the abilty's parameters and rule flags and use the unit using the abilty's own state to calculate the min/max range of the ability.
        /// </summary>
        /// <param name="ability">The ability to find the min/max range for</param>
        /// <param name="user">The unit using the ability</param>
        /// <returns>A min/max range value that has the range of the ability</returns>
        public static RangeValue<int> GetAbilityRangeDistance(IAbility ability, IGameUnit user)
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
                max = SizeCalculator.GetMaxRangeBonusForSize(user.SizeCategory);

                max += Math.Max(min - 1, 0);
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


        public static IEnumerable<MapPoint> GetMapPointsAbilityTargets(IAbility ability, IGameUnit user)
        {
            var rangeMinMax = GetAbilityRangeDistance(ability, user);

            return GetMapPointsAbilityTargets(ability, rangeMinMax, user.MapX, user.MapY, user.TileWidth, user.TileHeight);

        }

        public static IEnumerable<MapPoint> GetMapPointsAbilityTargets(IAbility ability, RangeValue<int> abilityRange, int casterX, int casterY, int casterWidth, int casterHeight)
        {
            // Quick get the edges of our unit
            var casterLeft   = casterX;
            var casterRight  = casterX + casterWidth - 1;
            var casterBottom = casterY;
            var casterTop    = casterY + casterHeight - 1;
            

            var startX = Math.Max(casterLeft - abilityRange.maxValue, 0);
            var endX = casterRight + abilityRange.maxValue;

            var startY = Math.Max(casterBottom - abilityRange.maxValue, 0);
            var endY = casterTop + abilityRange.maxValue;

            
            for(var x = startX; x <= endX; x++)
            {
                // Is the current X-position inside the caster?
                var isXInCaster = (x >= casterLeft && x <= casterRight);
                var casterXClosest = x < casterLeft ? casterLeft : casterRight;

                for(var y = startY; y <= endY; y++)
                {
                    // Is the current Y-position inside the caster?
                    var isYInCaster = (y >= casterBottom && y <= casterTop);

                    float fDistance;
                    // Check if we're inside of our caster. That's a special edge case of distance zero!
                    if(isXInCaster && isYInCaster)
                    {
                        fDistance = 0;
                    }
                    // Otherwise just use a normal distance algorithm to find the distance from the current X,Y to the closest tile of the caster
                    else
                    {
                        var casterYClosest = y < casterBottom ? casterBottom : casterTop;

                        var xDistance = x - casterXClosest;
                        var yDistance = y - casterYClosest;
                        fDistance = (float)Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
                    }

                    // Final distance always rounds up
                    var distance = (int)Math.Ceiling(fDistance);

                    // Check our distance. If it's within our ability range then 
                    if(distance >= abilityRange.minValue && distance <= abilityRange.maxValue)
                    {
                        yield return new MapPoint(x, y, 1, 1);
                    }
                }
            }
        }

    }
}
