using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMapMovementRoute : IComparable<IGameMapMovementRoute>
    {
        IReadOnlyList<Direction> Route { get; }
        MovementCost RouteCost { get; }
        int StartX { get; }
        int StartY { get; }
        int EndX { get; }
        int EndY { get; }
        int EndSortKey { get; }


        bool CheckDirectionForOverlap(Direction newDirection);
        IGameMapMovementRoute CreateExtendRoute(Direction newDirection, MovementCost newDirectionCost);
    }
}
