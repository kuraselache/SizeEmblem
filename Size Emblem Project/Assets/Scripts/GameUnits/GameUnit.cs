using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
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

        public bool IsActive { get; set; } = true;

        // Turn state properties
        public bool MovementActionConsumed { get; set; }
        public bool MinorActionConsumed { get; set; }
        public bool MajorActionConsumed { get; set; }
        
        public bool ActionOver { get; set; }


        public bool CanAct()
        {
            if (!IsActive) return false;
            if (HP <= 0) return false;
            if (ActionOver) return false;
            return true;
        }

        public bool CanMoveAction()
        {
            if (!CanAct() || SpentMovement > 0) return false;
            return true;
        }

        public void ResetActionsConsumed()
        {
            SpentMovement = 0;
            MovementActionConsumed = false;
            MinorActionConsumed = false;
            MajorActionConsumed = false;
            ActionOver = false;
        }


        public void EndAction()
        {
            ActionOver = true;
        }


        public void ProcessBattleStart()
        {
            // Make sure units are reset at the start of battle
            ResetActionsConsumed();
            // Reset usage of all abilities
            Abilities.ForEach(x => x.ResetBattleUsages());
        }

        public void ProcessBattleEnd()
        {

        }


        public void ProcessTurnStart()
        {

        }

        public void ProcessTurnEnd()
        {
            Abilities.ForEach(x => x.ProcessTurnEnd());
        }

        public void ProcessPhaseStart()
        {
        }

        public void ProcessPhaseEnd()
        {
            // We'll reset here so units once the phase is over. This will reset visual state to normal
            ResetActionsConsumed();
        }

        #endregion


        #region Faction

        public Faction unitFaction;
        public Faction UnitFaction
        {
            get { return unitFaction; }
            set
            {
                if (value == unitFaction) return;
                unitFaction = value;
            }
        }

        #endregion


        #region Movement


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

        private readonly List<MovementType> _movementTypes = new List<MovementType>();
        public IReadOnlyList<MovementType> MovementTypes { get { return _movementTypes.AsReadOnly(); } }


        public bool CanMove()
        {
            return RemainingMovement > 0;
        }

        public void AddRouteCost(IGameMapMovementRoute route)
        {
            SpentMovement = Int16.MaxValue;
            ActionOver = true;
        }

        public void RefreshMovementTypes()
        {
            _movementTypes.Clear();
            _movementTypes.Add(BaseUnitData.BaseMovementType);
        }

        #endregion


        #region Statistics

        private Dictionary<UnitStatistic, ulong> _statistics = new Dictionary<UnitStatistic, ulong>();
        public IReadOnlyDictionary<UnitStatistic, ulong> Statistics { get { return _statistics as IReadOnlyDictionary<UnitStatistic, ulong>; } }

        public ulong SetStatistic(UnitStatistic statistic, ulong value)
        {
            if(!Statistics.ContainsKey(statistic))
                _statistics.Add(statistic, value);
            else
                _statistics[statistic] = value;

            return value;
        }

        public ulong IncrementStatistic(UnitStatistic statistic, ulong value)
        {
            if (!Statistics.ContainsKey(statistic))
            {
                _statistics.Add(statistic, value);
                return value;
            }

            var newValue = _statistics[statistic] + value;
            _statistics[statistic] = newValue;
            return newValue;
        }

        #endregion



        public SizeCategory SizeCategory { get { return BaseUnitData.SizeCategory; } }
        public WeaponAdvantageCategory WeaponAdvantageCategory { get { return BaseUnitData.WeaponType; } }

        public GameUnitData baseUnitData;
        public GameUnitData BaseUnitData { get { return baseUnitData; } }


        #region Attribute Functions

        public int level = 1;
        public int Level { get { return level; } }


        public int hp;
        public int HP { get { return hp; } }

        public int sp;
        public int SP { get { return sp; } }


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


        private bool _lastActionOver;

        public void RefreshSprite()
        {
            _lastActionOver = ActionOver;

            UpdateSprite();
        }

        public void UpdateSprite()
        {
            if (_spriteRenderer == null) return;

            if(_lastActionOver != ActionOver)
            {
                _spriteRenderer.color = ActionOver ? Color.grey : Color.white;
                _lastActionOver = ActionOver;
            }
            
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


        public void Update()
        {
            UpdateSprite();
        }
    }
}
