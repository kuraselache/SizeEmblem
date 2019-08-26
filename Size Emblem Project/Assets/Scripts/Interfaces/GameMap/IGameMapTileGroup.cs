using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMapTileGroup
    {
        IList<IGameMapTile> Tiles { get; }

        float GetMovementCostForType(MovementType movementType);
        uint GetInhibitionScoreForUnit(IGameUnit unit);
    }
}
