using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Containers
{
    public class MovementCost
    {
        public readonly bool IsPassable;
        public readonly float Cost;
        public readonly ulong Inhibition;

        public MovementCost()
        {
            IsPassable = true;
            Cost = 0;
            Inhibition = 0;
        }

        public MovementCost(float cost, ulong inhibition)
        {
            IsPassable = true;
            Cost = cost;
            Inhibition = inhibition;
        }

        public MovementCost(bool isPassable, float cost, ulong inhibition)
        {
            IsPassable = isPassable;
            Cost = cost;
            Inhibition = inhibition;
        }


        public static MovementCost operator +(MovementCost a, MovementCost b)
        {
            return new MovementCost(a.IsPassable | b.IsPassable, a.Cost + b.Cost, a.Inhibition + b.Inhibition);
        }

        public readonly static MovementCost Impassable = new MovementCost(false, 0, 0);

    }
}
