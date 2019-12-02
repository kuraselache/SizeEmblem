using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Interfaces.Managers;
using SizeEmblem.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.GameMap
{
    public class GameMapTile : MonoBehaviour, IGameMapTile
    {
        #region Unity Dependencies & Components

        private SpriteRenderer _spriteRenderer;

        #endregion

        #region Awake & Dependencies

        private IRNGManager _rngManager;

        public void Awake()
        {
            _rngManager = new RNGManager();
        }


        #endregion


        #region Tile Position & Dimensions

        public Vector3 WorldPosition
        {
            get { return transform.position; }
            set
            {
                if (value == transform.position) return;
                transform.position = value;
            }
        }


        public int mapX;
        public int MapX
        {
            get { return mapX; }
            set 
            {
                if (value == mapX) return;
                mapX = value;
                _mapPoint.X = value;
            }
        }

        public int mapY;
        public int MapY
        {
            get { return mapY; }
            set
            {
                if (value == mapY) return;
                mapY = value;
                _mapPoint.Y = value;
                UpdateZLevel(-value);
            }
        }

        private int _tileWidth;
        public int TileWidth 
        {
            get { return _tileWidth; }
            set
            {
                if (value == _tileWidth) return;
                _tileWidth = value;
                _mapPoint.Width = value;
            }
        }

        private int _tileHeight;
        public int TileHeight 
        {
            get { return _tileHeight; }
            set
            {
                if (value == _tileHeight) return;
                _tileHeight = value;
                _mapPoint.Height = value;
            }
        }


        private MapPoint _mapPoint = new MapPoint();
        public MapPoint MapPoint { get { return _mapPoint; } }


        #endregion


        #region Tile Health & Destruction

        public int tileHealth;
        public int TileHealth
        {
            get { return tileHealth; }
            set
            {
                if (value == tileHealth) return;
                tileHealth = value;
            }
        }

        public int TileDefense => 0;

        public bool isDestroyed;
        public bool IsDestroyed
        {
            get { return isDestroyed; }
            set
            {
                if (value == isDestroyed) return;
                isDestroyed = value;
            }
        }

        public ulong destructionValue;
        public ulong DestructionValue { get { return destructionValue; } }

        public ulong destructionBodyCount;
        public ulong DestructionBodyCount { get { return destructionBodyCount; } }



        public void TakeDamage(int damage, IGameUnit attackingUnit, MoveUnitChangeContainer moveUnitChangeContainer)
        {
            // Can't inflict damage to non-destructable tiles so check that first
            if (!MapTileData.Destructable || TileHealth <= 0) return;

            // For now only handle dealing positive damage
            if (damage <= 0) return;

            // Change tile health to inflict damage
            var startingHealth = TileHealth;
            TileHealth = Mathf.Clamp(TileHealth - damage, 0, MapTileData.TileMaxHealth);

            // Check if this tile has been destroyed aka HP = 0
            if (tileHealth <= 0 && !IsDestroyed)
            {
                // Create a container for our state change
                var tileChange = new TileStateChangeContainer(this, IsDestroyed, startingHealth, MapTileData);
                moveUnitChangeContainer.TileChanges.Add(tileChange);

                // If we have an attacking unit then count our destruction value and body count towards them
                if (attackingUnit != null)
                {
                    attackingUnit.IncrementStatistic(UnitStatistic.PropertyDamage, DestructionValue);
                    moveUnitChangeContainer.ApplyStatisticOffset(UnitStatistic.PropertyDamage, DestructionValue);

                    attackingUnit.IncrementStatistic(UnitStatistic.BodyCount, DestructionBodyCount);
                    moveUnitChangeContainer.ApplyStatisticOffset(UnitStatistic.BodyCount, DestructionBodyCount);
                }

                // If we have a tile to become when we're destroyed then we'll change to that
                if(MapTileData.OnDestroyedTile != null)
                {
                    // Update the backing map tile data for this tile, this should trigger a refresh of this tile
                    ChangeMapTileData(MapTileData.OnDestroyedTile);
                }
                else
                {
                    // Otherwise just flag this tile as destroyed
                    IsDestroyed = true;
                }
            }
        }


        public void RefreshDestructionValues()
        {
            destructionValue = MapTileData.DestructionValue;
            destructionBodyCount = _rngManager.GetRange(MapTileData.DestructionDeathTollRange.minValue, MapTileData.DestructionDeathTollRange.maxValue);
        }


        public ulong GetInhibitionScore(IGameUnit unit)
        {
            if (unit.GetAttribute(UnitAttribute.WalkingDamage) > TileHealth) return DestructionValue;
            return 0;
        }


        public void UndoChange(TileStateChangeContainer changeContainer)
        {
            if(changeContainer.DestroyedFlag.HasValue)
            {
                IsDestroyed = changeContainer.DestroyedFlag.Value;
            }

            if(changeContainer.OriginalHealth.HasValue)
            {
                TileHealth = changeContainer.OriginalHealth.Value;
            }

            if(changeContainer.OriginalTileData != null && changeContainer.OriginalTileData is GameMapTileData) // oh no why am I doing this
            {
                ChangeMapTileData(changeContainer.OriginalTileData as GameMapTileData);
            }
        }

        #endregion


        public Sprite OverrideSprite;


        // Parent data scriptable object for this tile
        public GameMapTileData mapTileData;
        public GameMapTileData MapTileData
        {
            get { return mapTileData; }
            private set
            {
                if (value == mapTileData) return;
                mapTileData = value;
            }
        }

        public void ChangeMapTileData(GameMapTileData newMapTileData)
        {
            if (MapTileData == newMapTileData) return;

            MapTileData = newMapTileData;
            RefreshMapTileData();
        }


        public void UpdateZLevel(int newOrder)
        {
            if(_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer != null) _spriteRenderer.sortingOrder = newOrder;
        }

        public void Start()
        {
            RefreshMapTileData();
            // Initialize properties that are normally only updated on property changed
        }


        public void RefreshMapTileData()
        {
            if (MapTileData == null) return;

            // When we change tiles refresh our HP to the tile's max health
            TileHealth = MapTileData.TileMaxHealth;

            RefreshMapTileSprite();
            RefreshDestructionValues();
        }


        public void RefreshMapTileSprite()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            if(OverrideSprite != null)
            {
                spriteRenderer.sprite = OverrideSprite;
                return;
            }

            spriteRenderer.sprite = MapTileData?.TileSprite;
        }


        public void RefreshTileDimensions()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

        }

    }
}
