using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Extensions;
using SizeEmblem.Scripts.Interfaces.GameMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.GameMap
{
    public class GameMapMovementRoute : IGameMapMovementRoute
    {
        private List<Direction> _route = new List<Direction>();
        public IReadOnlyList<Direction> Route { get { return _route.AsReadOnly(); } }

        public MovementCost RouteCost { get; }

        public int StartX { get; }
        public int StartY { get; }

        public int EndX { get; }
        public int EndY { get; }

        // We use this just as a quick hack for sorting routes based on their end X,Y
        public int EndSortKey { get { return EndX * 10000 + EndY; } }

        public GameMapMovementRoute(int startX, int startY)
        {
            StartX = startX;
            StartY = startY;
            EndX = startX;
            EndY = startY;
            // The start of a route has no direction
            _route.Add(Direction.None);
            RouteCost = new MovementCost();
        }

        //public GameMapMovementRoute(int startX, int startY, IEnumerable<Direction> route, MovementCost routeCost)
        //{
        //    StartX = startX;
        //    StartY = startY;
        //    // Copy the passed route to this instance
        //    _route.AddRange(route);
        //    RouteCost = routeCost;
        //}

        public GameMapMovementRoute(IGameMapMovementRoute toCopy, Direction addedDirection, MovementCost addedCost)
        {
            StartX = toCopy.StartX;
            StartY = toCopy.StartY;
            _route.AddRange(toCopy.Route);
            _route.Add(addedDirection);
            RouteCost = toCopy.RouteCost + addedCost;

            EndX = toCopy.EndX;
            EndY = toCopy.EndY;
            switch(addedDirection)
            {
                case Direction.East: EndX++; break;
                case Direction.West: EndX--; break;
                case Direction.North: EndY++; break;
                case Direction.South: EndY--; break;
            }
        }



        /// <summary>
        /// Check a direction from this route. If the direction overlaps with any part of the route then return false. If the direction gives no overlap then return true
        /// </summary>
        /// <param name="newDirection">The direction to check on this route for overlap</param>
        /// <returns>True if the direction causes no overlap with the current route. False if there is overlap.</returns>
        public bool CheckDirectionForOverlap(Direction newDirection)
        {
            // If we have no route directions then the new direction is always valid
            if(!Route.Any()) return true;
            // No direction in a non-empty route will overlap with itself so it will always be false
            if (newDirection == Direction.None) return false;

            // We'll work in coordinates where 0,0 is the end of this route
            // Calculate our direction X,Y in relative coordinates to the end of this route
            var newDirectionX = newDirection == Direction.East  ? 1 : newDirection == Direction.West  ? -1 : 0;
            var newDirectionY = newDirection == Direction.North ? 1 : newDirection == Direction.South ? -1 : 0;

            // Assign our current position at the end of this route
            var currentX = 0;
            var currentY = 0;

            // And work backwards through each step checking for overlap with our newDirection X,Y
            foreach(var direction in _route.FastReverse())
            {
                // If the current direction is none then there's no position change to consider
                if (direction == Direction.None) continue;

                // Update our current X,Y on the _opposite_ of the direction
                switch(direction)
                {
                    case Direction.East: currentX -= 1; break;
                    case Direction.West: currentX += 1; break;
                    case Direction.North: currentY -= 1; break;
                    case Direction.South: currentY += 1; break;
                }
                // If we have a collision with our current X,Y with our new dirction X,Y then we have overlap and this check fails
                if (currentX == newDirectionX && currentY == newDirectionY) return false;
            }

            // Fallthrough: We checked out entire route for overlap and found none so this direction is good!
            return true;

        }


        public IGameMapMovementRoute CreateExtendRoute(Direction newDirection, MovementCost newDirectionCost)
        {
            return new GameMapMovementRoute(this, newDirection, newDirectionCost);
        }


        #region IComparable Implementation

        public int CompareTo(IGameMapMovementRoute other)
        {
            // Routes are sorted by movement cost, the impedence
            //var costCompare = this.RouteCost.

            throw new NotImplementedException();
        }

        #endregion
    }
}
