using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Containers
{
    public struct TileStateChangeContainer
    {
        public readonly IGameMapTile Tile;

        public readonly bool? DestroyedFlag;
        public readonly int? OriginalHealth;

        public readonly IGameMapTileData OriginalTileData;


        public TileStateChangeContainer(IGameMapTile tile, bool? destroyedFlag = null, int? originalHealth = null, IGameMapTileData originalTileData = null)
        {
            Tile = tile;
            DestroyedFlag = destroyedFlag;
            OriginalHealth = originalHealth;
            OriginalTileData = originalTileData;
        }

        public void Undo()
        {
            if (Tile == null) return;

            Tile.UndoChange(this);
        }
    }
}
