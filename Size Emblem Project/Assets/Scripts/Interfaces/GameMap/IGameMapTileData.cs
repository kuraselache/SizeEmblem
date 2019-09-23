using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMapTileData
    {
        string TileName { get; } // Internal use

        int TileWidth { get; }
        int TileHeight { get; }

        Dictionary<MovementType, MovementData> MovementData { get; }

        int TileMaxHealth { get; }
        bool Destructable { get; set; }

        ulong DestructionValue { get; set; }
        RangeValueULong DestructionDeathTollRange { get; set; }


        float GetMovementCost(MovementType movementType);
    }
}
