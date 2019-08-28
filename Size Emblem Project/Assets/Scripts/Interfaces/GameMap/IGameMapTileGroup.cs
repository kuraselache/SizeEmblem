using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMapTileGroup
    {
        IReadOnlyList<IGameMapTile> Tiles { get; }

        void SetLayerTile(int layer, IGameMapTile tile);

        float GetMovementCostForType(MovementType movementType);
        ulong GetInhibitionScoreForUnit(IGameUnit unit);
    }
}
