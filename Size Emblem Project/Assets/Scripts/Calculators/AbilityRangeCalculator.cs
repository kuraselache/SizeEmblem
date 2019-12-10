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

            // Get our drawing areas and points
            var drawXStart = casterLeft;
            var drawXEnd = casterRight;
            var drawYStart = casterTop + abilityRange.maxValue;
            var drawYEnd = casterBottom - abilityRange.maxValue;

            var skipXStart = casterLeft;
            var skipXEnd = casterRight;
            var skipDepth = abilityRange.minValue - abilityRange.maxValue;



            for(var y = drawYStart; y >= drawYEnd; y--)
            {
                for(var x = drawXStart; x <= drawXEnd; x++)
                {
                    // Skip this point if we're in our skip draw range. This is how minimum-range is done
                    if (skipDepth > 0 && x >= skipXStart && x <= skipXEnd) continue;
                    // If this abilty has the direction flag then skip if we aren't aligned with the caster
                    if (ability.RangeSpecialRule == AbilityRangeSpecialRule.Directional && ((y > casterTop || y < casterBottom) && (x < casterLeft || x > casterRight))) continue;
                    yield return new MapPoint(x, y, 1, 1);
                }

                // Adjust our drawXStart&End based on if we're on top approach the caster (expanding our area) or below the caster (shrinking our area)
                if(y > casterTop)
                {
                    drawXStart--;
                    drawXEnd++;
                    
                    if(skipDepth > 0)
                    {
                        skipXStart--;
                        skipXEnd++;
                    }
                    skipDepth++;
                }
                if(y <= casterBottom)
                {
                    drawXStart++;
                    drawXEnd--;
                    
                    if(skipDepth > 0)
                    {
                        skipXStart++;
                        skipXEnd--;
                    }
                    skipDepth--;
                }
            }

            
        }


        public static bool CanAbilityTargetUnit(IGameUnit user, IGameUnit target, IAbility ability)
        {
            // Get the map points the ability can target
            var targetPoints = GetMapPointsAbilityTargets(ability, user).ToList();

            // See if there's any collision between those map points and the target
            var rangeCheck = targetPoints.Any(x => x.CollidesWith(target.MapPoint));
            return rangeCheck;
        }
    }
}
