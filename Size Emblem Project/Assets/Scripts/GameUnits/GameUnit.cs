using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.GameUnits
{
    public class GameUnit : MonoBehaviour, IGameUnit
    {
        #region Unity Dependencies & Components

        private SpriteRenderer _spriteRenderer;

        #endregion


        public string unitName;
        public string UnitName { get { return unitName; } }


        public ILocalizationString NameLocalized { get; }



        #region Position & Dimensions

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
                RefreshBounds();
            }
        }

        public int mapY;
        public int MapY
        {
            get { return mapY; }
            set
            {
                if (mapY == value) return;
                mapY = value;
                RefreshBounds();
                UpdateSortingOrder(-value);
            }
        }

        public int? tileWidthOverride;
        public int TileWidth
        {
            get
            {
                if (tileWidthOverride.HasValue) return tileWidthOverride.Value;
                return BaseUnitData != null ? BaseUnitData.TileWidth : 0;
            }
        }

        public int? tileHeightOverride;
        public int TileHeight
        {
            get
            {
                if (tileHeightOverride.HasValue) return tileHeightOverride.Value;
                return BaseUnitData != null ? BaseUnitData.TileHeight : 0;
            }
        }

        public Bounds Bounds { get; private set; }

        public void RefreshBounds()
        {
            Bounds = new Bounds(new Vector3(MapX + TileWidth / 2, MapY + TileHeight / 2), new Vector3(TileWidth, TileHeight));
        }

        #endregion


        #region Combat State

        public Faction faction;
        public Faction Faction { get { return faction; } }

        // Turn state properties
        public float MovementConsumed { get; set; }
        public bool MovementActionConsumed { get; set; }
        public bool MinorActionConsumed { get; set; }
        public bool MajorActionConsumed { get; set; }

        #endregion


        #region Movement

        public int BaseMovement { get { return BaseUnitData.MovementSpeed; } }

        public int MaxMovement { get { return GetAttribute(UnitAttribute.Movement); } }


        public int spentMovement;
        public int SpentMovement
        {
            get { return spentMovement; }
            set
            {
                if (value == spentMovement) return;
                spentMovement = value;
            }
        }

        public int RemainingMovement { get { return Math.Max(MaxMovement - SpentMovement, 0); } }

        public IReadOnlyList<MovementType> MovementTypes { get; private set; }


        public bool CanMove()
        {
            return RemainingMovement > 0;
        }

        public void RefreshMovementTypes()
        {
            var movementTypes = new List<MovementType>();
            movementTypes.Add(BaseUnitData.BaseMovementType);

            MovementTypes = movementTypes.AsReadOnly();
        }

        #endregion


        #region Destruction Statistics

        public ulong destructionTotal;
        public ulong DestructionTotal
        {
            get { return destructionTotal; }
            set
            {
                if (value == destructionTotal) return;
                destructionTotal = value;
            }
        }



        public ulong bodyCount;
        public ulong BodyCount
        {
            get { return bodyCount; }
            set
            {
                if (value == bodyCount) return;
                bodyCount = value;
            }
        }

        #endregion


        public GameUnitData baseUnitData;
        public GameUnitData BaseUnitData { get { return baseUnitData; } }


        #region Attribute Functions

        public int level = 1;
        public int Level { get { return level; } }


        public int currentHP;
        public int CurrentHP { get { return currentHP; } }

        public int currentSP;
        public int CurrentSP { get { return currentSP; } }


        public List<IAbility> Abilities { get; } = new List<IAbility>();

        // Dictionary to hold stat-up items used for this unit
        public Dictionary<UnitAttribute, int> AttributesGains { get; } = new Dictionary<UnitAttribute, int>();

        // Dictionary to hold stat increases from level ups. Maybe should be a separate level container?
        public Dictionary<UnitAttribute, int> AttributesLevels { get; } = new Dictionary<UnitAttribute, int>();



        public int GetAttribute(UnitAttribute attribute)
        {
            var stat = 0;

            // Get the base attribute from the unit base data
            stat += GetAttributeBase(attribute);

            // Get gain from level ups to stat
            stat += GetAttributeFromLevelUps(attribute);

            // Get bonus for attribute
            stat += GetAttributeFromGains(attribute);

            // Get equipment bonus for attrubte
            stat += GetAttributeFromEquipment(attribute);

            // Get support bounus for attribute
            stat += GetAttributeFromSupport(attribute);

            return stat;
        }

        public int GetAttributeBase(Constants.UnitAttribute attribute)
        {
            switch (attribute)
            {
                case UnitAttribute.MaxHP:              return BaseUnitData.HP; 
                case UnitAttribute.MaxSP:              return BaseUnitData.SP; 
                case UnitAttribute.Strength:           return BaseUnitData.Strength; 
                case UnitAttribute.Magic:              return BaseUnitData.Magic;
                case UnitAttribute.Defense:            return BaseUnitData.Defense;
                case UnitAttribute.Resistance:         return BaseUnitData.Resistance;
                case UnitAttribute.Skill:              return BaseUnitData.Skill;
                case UnitAttribute.Speed:              return BaseUnitData.Speed;
                case UnitAttribute.Luck:               return BaseUnitData.Luck;
                case UnitAttribute.Movement:           return BaseUnitData.MovementSpeed;
                case UnitAttribute.WalkingDamage:      return BaseUnitData.BonusWalkingDamage;
                case UnitAttribute.WalkingDamageRange: return BaseUnitData.BonusWalkingDamageRange;
                default: throw new NotImplementedException(string.Format("Attribute: {0} not implemented in Unit:GetAttributeBase function", attribute));
            }
        }

        public int GetAttributeFromGains(UnitAttribute attribute)
        {
            if (!AttributesGains.ContainsKey(attribute)) return 0;
            return AttributesGains[attribute];
        }

        public int GetAttributeFromLevelUps(UnitAttribute attribute)
        {
            if (!AttributesLevels.ContainsKey(attribute)) return 0;
            return AttributesLevels[attribute];
        }

        /// <summary>
        /// Get attribute bonuses from equipment
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public int GetAttributeFromEquipment(UnitAttribute attribute)
        {
            // Equipment isn't implemented yet, ignore this
            return 0;
        }

        public int GetAttributeFromSupport(UnitAttribute attribute)
        {
            // Also not implemented, ignore this for now
            return 0;
        }

        #endregion


        #region Turn & Action Functions

        /// <summary>
        /// Function to reset a unit to their pre-turn state, resetting actions and movement consumed
        /// </summary>
        public void ResetTurnState()
        {
            MovementConsumed = 0f;
            MovementActionConsumed = false;
            MinorActionConsumed = false;
            MajorActionConsumed = false;
        }

        #endregion


        #region Drawing & Graphics Related

        public SpriteRenderer GetSpriteRenderer()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            return _spriteRenderer;
        }

        public void UpdateSortingOrder(int newOrder)
        {
            var spriteRenderer = GetSpriteRenderer();
            if (spriteRenderer == null) return;

            spriteRenderer.sortingOrder = newOrder;
        }

        public void AlignSpriteRenderer()
        {
            var spriteRenderer = GetSpriteRenderer();
            if (spriteRenderer == null) return;

            // If the sprite tile is too small then turn it off and don't bother with alignment
            if(TileWidth <= 0 || TileHeight <= 0)
            {
                spriteRenderer.enabled = false;
                return;
            }

            // Move the sprite to a local position so it appears in the center of the area it occupies
            spriteRenderer.enabled = true;
            var offsetPosition = new Vector3(TileWidth / 2f, TileHeight / 2f);
            spriteRenderer.transform.localPosition = offsetPosition;
        }


        #endregion


        
        


        public void Start()
        {
            var spriteRenderer = GetSpriteRenderer();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = BaseUnitData.unitSprite;
                AlignSpriteRenderer();
            }

            // Initialize properties that are normally only updated on property changed
            RefreshBounds();
            RefreshMovementTypes();
        }

    }
}
