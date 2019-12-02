using SizeEmblem.Scripts.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SizeEmblem.Scripts.Interfaces.GameMap
{
    public interface IGameMapObject
    {
        Vector3 WorldPosition { get; set; }
        int MapX { get; set; }
        int MapY { get; set; }

        int TileWidth { get; }
        int TileHeight { get; }

        MapPoint MapPoint { get; }
    }
}
