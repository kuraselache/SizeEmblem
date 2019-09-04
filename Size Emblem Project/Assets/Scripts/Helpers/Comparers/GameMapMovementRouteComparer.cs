using SizeEmblem.Scripts.Interfaces.GameMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Helpers.Comparers
{
    public class GameMapMovementRouteComparer : IComparer<IGameMapMovementRoute>
    {
        public static GameMapMovementRouteComparer Instance { get; } = new GameMapMovementRouteComparer();

        protected GameMapMovementRouteComparer()
        {

        }




        public int Compare(IGameMapMovementRoute x, IGameMapMovementRoute y)
        {
            var compareResult = MovementCostComparerLowInhibition.Instance.Compare(x.RouteCost, y.RouteCost);
            if (compareResult == 0)
                compareResult = x.Route.Count.CompareTo(y.Route.Count);

            return compareResult;
        }
    }
}
