﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameMap.Factories
{
    public interface IGameMapTileGroupFactory
    {
        IGameMapTileGroup Resolve(int mapLayers);

        void Release(IGameMapTileGroup gameMapTileGroup);
    }
}
