using SizeEmblem.Scripts.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Helpers.Comparers
{
    public class MovementCostComparerLowInhibition : IComparer<MovementCost>
    {

        public static MovementCostComparerLowInhibition Instance { get; } = new MovementCostComparerLowInhibition();

        protected MovementCostComparerLowInhibition()
        {
            // please use the singleton reference
        }


        public int Compare(MovementCost x, MovementCost y)
        {
            // Execute the base compareison first. If it says the two MovementCosts are different then obey it. If it says they're the same then this comparer will do it's work
            var baseCompare = MovementCostComparer.Instance.Compare(x, y);
            if (baseCompare != 0) return baseCompare;

            return x.Inhibition.CompareTo(y.Inhibition);
        }
    }
}
