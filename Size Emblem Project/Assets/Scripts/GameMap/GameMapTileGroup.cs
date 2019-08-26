using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Extensions;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.GameMap
{
    public class GameMapTileGroup : IGameMapTileGroup
    {
        private List<IGameMapTile> _tiles;
        public IReadOnlyList<IGameMapTile> Tiles { get { return _tiles.AsReadOnly(); } }


        public GameMapTileGroup(int layers)
        {
            // Populate the tile group for this 
            _tiles = new List<IGameMapTile>(layers);
            for (var i = 0; i < layers; i++) _tiles.Add(null);
        }

        public void SetLayerTile(int layer, IGameMapTile tile)
        {
            if (layer < 0 || layer >= _tiles.Count) return;
            _tiles[layer] = tile;
        }



        public float GetMovementCostForType(MovementType movementType)
        {
            // If this group is empty then this group is impassable
            if (!_tiles.Any()) return float.NaN;

            // Check our highest map layer to our lowest, finding the first not-NaN movement cost
            foreach(var tile in _tiles.FastReverse().Where(x => x != null))
            {
                var movementCost = tile.MapTileData.GetMovementCost(movementType);
                if (!float.IsNaN(movementCost)) return movementCost;
            }

            // Fallthrough: All tiles returned NaN so this movement group is impassable
            return float.NaN;
        }

        public uint GetInhibitionScoreForUnit(IGameUnit unit)
        {
            if (!Tiles.Any()) return 0;


            return 0;
        }
    }
}
