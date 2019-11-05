using SizeEmblem.Assets.Scripts.Calculators;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.GameMap
{
    public class GameMapAbilityRange
    {
        public IReadOnlyCollection<MapPoint> Points;


        public GameMapAbilityRange(IGameUnit unit, IAbility ability)
        {
            List<MapPoint> points;

            switch(ability.RangeSpecialRule)
            {
                case AbilityRangeSpecialRule.None: points = GenerateRadiusPoints(unit, ability); break;
                case AbilityRangeSpecialRule.Directional: throw new NotImplementedException(); break;
                default: throw new Exception();
            }

            Points = points.AsReadOnly();
        }


        public List<MapPoint> GenerateRadiusPoints(IGameUnit unit, IAbility ability)
        {
            var points = AbilityRangeCalculator.GetMapPointsAbilityTargets(ability, unit).ToList();
            return points;
        }

        public void GenerateDirectionalPoints(IGameMapObject mapObject, IAbility ability, IList<MapPoint>)
        {

        }
    }
}
