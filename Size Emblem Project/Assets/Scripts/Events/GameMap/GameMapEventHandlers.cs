using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Events.GameMap
{
    public delegate void GameMapLoadedHandler(IGameMap map, EventArgs e);


    public delegate void SelectedUnitChangedHandler(IGameMap map, UnitSelectedEventArgs e);

    public class UnitSelectedEventArgs : EventArgs
    {
        public readonly IGameUnit Unit;

        public UnitSelectedEventArgs()
        {

        }

        public UnitSelectedEventArgs(IGameUnit unit)
        {
            Unit = unit;
        }
    }
}
