using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.GameMap
{
    [CreateAssetMenu(fileName = "New Tile Data", menuName = "Game Data/Tile Data/Movement", order = 6)]
    public class GameMapTileMovementData : ScriptableObject
    {

        public MovementData[] movementData;
    }
}
