using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.GameMap
{
    public class GameMapTileGroup : IGameMapTileGroup
    {
        public IList<IGameMapTile> Tiles { get; } = new List<IGameMapTile>();




        public float GetMovementCostForType(MovementType movementType)
        {
            // If this group is empty then this group is impassable
            if (!Tiles.Any()) return float.NaN;

            // Collect our movement data for this tile group and data
            var movementCosts = Tiles.Select(x => x.MapTileData.GetMovementCost(movementType)).ToArray();
            // If all tiles are impassible then this group is impasible for the given movement type
            if (movementCosts.All(x => float.IsNaN(x))) return float.NaN;
            // Otherwise return the LOWEST movement cost for this tile group
            return movementCosts.Where(x => !float.IsNaN(x)).Min();
        }

        public uint GetInhibitionScoreForUnit(IGameUnit unit)
        {
            if (!Tiles.Any()) return 0;


            return 0;
        }
    }
}
