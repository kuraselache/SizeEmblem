using SizeEmblem.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameUnits
{
    public interface IUnitData
    {
        // Base unit name
        string UnitName { get; }


        // Unit base dimensions
        int TileWidth { get; }
        int TileHeight { get; }


        // Unit categorization
        SizeCategory SizeCategory { get; }
        UnitCategory UnitCategory { get; }

        // Movement attributes
        MovementType BaseMovementType { get; }
        int MovementSpeed { get; }


        // Base Attributes
        int HP { get; }
        int SP { get; }

        int Physical { get; }
        int Magic { get; }
        int Skill { get; }
        int Speed { get; }
        int Defense { get; }
        int Resistance { get; }
        int Luck { get; }

        // Details
        double Height { get; }
        bool ShowHeight { get; }

        double Weight { get; }
        bool ShowWeight { get; }
        //bool ScaleWeight { get; }
    }
}
