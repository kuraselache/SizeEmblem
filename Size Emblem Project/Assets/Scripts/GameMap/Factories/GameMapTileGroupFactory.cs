using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameMap.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.GameMap.Factories
{
    public class GameMapTileGroupFactory : IGameMapTileGroupFactory
    {
        

        public IGameMapTileGroup Resolve()
        {
            return new GameMapTileGroup();
        }

        public void Release(IGameMapTileGroup gameMapTileGroup)
        {
        }
    }
}
