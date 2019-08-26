using Assets.Scripts.Containers;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.GameMap
{
    [CreateAssetMenu(fileName = "New Tile Data", menuName = "Game Data/Tile Data/Tile", order = 5)]
    public class GameMapTileData : ScriptableObject, IGameMapTileData
    {
        #region Properties & Fields

        // Internal use for identifying what a tile is
        public string tileName = "New Tile Data";
        public string TileName { get { return tileName; } }



        [Range(1,10)]
        public int tileWidth = 1;
        public int TileWidth { get { return tileWidth; } }

        [Range(1, 10)]
        public int tileHeight = 1;
        public int TileHeight { get { return tileHeight; } }


        public GameMapTileMovementData movementTileData;

        
        public Dictionary<MovementType, MovementData> MovementData { get; } = new Dictionary<MovementType, MovementData>();



        [Range(0,100)]
        public int tileMaxHealth;
        public int TileMaxHealth
        {
            get { return tileMaxHealth; }
            set
            {
                if (value == tileMaxHealth) return;
                tileMaxHealth = value;
            }
        }

        public GameMapTileData onDestroyedTile;
        public GameMapTileData OnDestroyedTile
        {
            get { return onDestroyedTile; }
            set
            {
                if (value == onDestroyedTile) return;
                onDestroyedTile = value;
            }
        }


        public bool destructable;
        public bool Destructable
        {
            get { return destructable; }
            set
            {
                if (value == destructable) return;
                destructable = value;
            }
        }




        public ulong destructionValue;
        public ulong DestructionValue
        {
            get { return destructionValue; }
            set
            {
                if (value == destructionValue) return;
                destructionValue = value;
            }
        }

        public RangeValueULong destructionDeathTollRange;
        public RangeValueULong DestructionDeathTollRange
        {
            get { return destructionDeathTollRange; }
            set
            {
                if (value == destructionDeathTollRange) return;
                destructionDeathTollRange = value;
            }
        }




        // Sprite for this tile
        public Sprite TileSprite;



        #endregion

        /// <summary>
        /// aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
        /// 
        /// why do I do these things
        /// </summary>
        public void PopulateMovementData()
        {
            if (MovementData.Any() || movementTileData == null) return;

            foreach(var movementData in movementTileData.movementData)
            {
                if (MovementData.ContainsKey(movementData.movementType)) continue;
                MovementData.Add(movementData.movementType, movementData);
            }
        }


        /// <summary>
        /// Get the movement cost for this tile for a given movement type.
        /// If the movement type is not included in this tile then it will check the "General" movement type and return that.
        /// If no movement data is found it will return NaN to indicate this tile is impassible.
        /// </summary>
        /// <param name="movementType">The type of movement to check.</param>
        /// <returns>The movement cost of the tile for the given movement type. Retuns NaN if no movement data is found.</returns>
        public float GetMovementCost(MovementType movementType)
        {
            // Make sure our dumb dictionary of stupidity is populated before we check movement data for this tile data
            PopulateMovementData();

            // If no movement data exists for the given movement type assume it is impassable aka a negative value
            if (!MovementData.ContainsKey(movementType))
            {
                if (movementType == MovementType.General) return float.NaN;
                return (GetMovementCost(MovementType.General));
            }

            return MovementData[movementType].MovementCost;
        }
    }
}
