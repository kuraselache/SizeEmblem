using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Events.GameMap
{
    // Map Loaded Event
    public delegate void GameMapLoadedHandler(IGameMap map, EventArgs e);


    // Selected Unit Event
    public delegate void SelectedUnitHandler(IGameMap map, UnitSelectedEventArgs e);

    public class UnitSelectedEventArgs : EventArgs
    {
        public static new UnitSelectedEventArgs Empty = new UnitSelectedEventArgs();

        public readonly IGameUnit Unit;

        protected UnitSelectedEventArgs()
        {

        }

        public UnitSelectedEventArgs(IGameUnit unit)
        {
            Unit = unit;
        }
    }

    // Selected Route Event
    public delegate void SelectedRouteHandler(IGameMap map, RouteSelectedEventArgs e);

    public class RouteSelectedEventArgs : EventArgs
    {
        public static new RouteSelectedEventArgs Empty = new RouteSelectedEventArgs();

        public readonly IGameMapMovementRoute Route;

        protected RouteSelectedEventArgs()
        {

        }

        public RouteSelectedEventArgs(IGameMapMovementRoute route)
        {
            Route = route;
        }
    }

    // Unit Move Completed Event
    public class UnitMoveCompletedEventArgs : EventArgs
    {
        public readonly MoveUnitChangeContainer MoveUnitChangeContainer;

        public UnitMoveCompletedEventArgs(MoveUnitChangeContainer moveUnitChangeContainer)
        {
            MoveUnitChangeContainer = moveUnitChangeContainer;
        }
    }
}
