using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Scripts.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMapTile : IGameMapObject
    {
        GameMapTileData MapTileData { get; }

        int TileHealth { get; }
        bool IsDestroyed { get; }

        ulong GetInhibitionScore(IGameUnit unit);

        //void TakeDamage(int damage, IGameUnit attackingUnit);
        void TakeDamage(int damage, IGameUnit attackingUnit, MoveUnitChangeContainer moveChangeContainer);

        void UndoChange(TileStateChangeContainer changeContainer);
    }
}
