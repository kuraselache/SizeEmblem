using SizeEmblem.Scripts.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Helpers.Comparers
{
    public class MovementCostComparer : IComparer<MovementCost>
    {
        public static MovementCostComparer Instance { get; } = new MovementCostComparer();

        protected MovementCostComparer()
        {
            // please use the singleton reference
        }


        private const float CompareToEpsilon = 0.001f;

        public int Compare(MovementCost x, MovementCost y)
        {
            // Passible checks: Whatever tile is passible comes first
            if (x.IsPassable && !y.IsPassable) return -1;
            if (!x.IsPassable && y.IsPassable) return 1;

            // Cost check: Whatever tile costs least comes first. Since this is a floating point comparison we need to compensate for that using a custom Epsilon value. If the difference is cost is below the Epsilon then
            // assume they're the same cost value
            var diff = x.Cost - y.Cost;
            return Math.Abs(diff) > CompareToEpsilon ? Math.Sign(diff) : 0;
        }
    }
}
