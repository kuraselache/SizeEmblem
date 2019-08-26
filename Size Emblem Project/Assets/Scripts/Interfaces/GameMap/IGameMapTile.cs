using SizeEmblem.Scripts.GameMap;
using SizeEmblem.Scripts.Interfaces.Units;
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

        ulong GetInhibitionScore(IGameUnit unit);

        void InflictDamage(int damage, IGameUnit attackingUnit);
    }
}
