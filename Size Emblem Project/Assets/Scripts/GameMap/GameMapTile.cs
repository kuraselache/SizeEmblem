using Assets.Scripts.Interfaces.Managers;
using Assets.Scripts.Managers;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.Units;
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
            set { mapX = value; }
        }

        public int mapY;
        public int MapY
        {
            get { return mapY; }
            set
            {
                if (value == mapY) return;
                mapY = value;
                UpdateSortingOrder(-value);
            }
        }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public Bounds Bounds { get; private set; }

        public void RefreshBounds()
        {
            Bounds = new Bounds(new Vector3(MapX + TileWidth / 2, MapY + TileHeight / 2), new Vector3(TileWidth, TileHeight));
        }

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



        public void InflictDamage(int damage, IGameUnit attackingUnit)
        {
            // Can't inflict damage to non-destructable tiles so check that first
            if (!MapTileData.Destructable) return;

            // For now only handle dealing positive damage
            if (damage <= 0) return;

            // Change tile health to inflict damage
            TileHealth = Mathf.Clamp(TileHealth - damage, 0, MapTileData.TileMaxHealth);

            // Check if this tile has been destroyed aka HP = 0
            if(tileHealth <= 0 && !IsDestroyed)
            {
                // If we have an attacking unit then count our destruction value and body count towards them
                if(attackingUnit != null)
                {
                    attackingUnit.DestructionTotal += DestructionValue;
                    attackingUnit.BodyCount += DestructionBodyCount;
                }

                // If we have a tile to become when we're destroyed then we'll change to that
                if(MapTileData.OnDestroyedTile != null)
                {
                    // Update the backing map tile data for this tile, this should trigger a refresh of this tile
                    MapTileData = MapTileData.OnDestroyedTile;
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

        #endregion


        public Sprite OverrideSprite;


        // Parent data scriptable object for this tile
        public GameMapTileData mapTileData;
        public GameMapTileData MapTileData
        {
            get { return mapTileData; }
            set
            {
                if (value == mapTileData) return;
                mapTileData = value;
                RefreshMapTileData();
            }
        }


        public void UpdateSortingOrder(int newOrder)
        {
            if(_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer != null) _spriteRenderer.sortingOrder = newOrder;
        }

        public void Start()
        {
            RefreshMapTileData();
            // Initialize properties that are normally only updated on property changed
            RefreshBounds();
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
