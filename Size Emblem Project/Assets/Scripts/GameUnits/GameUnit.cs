using SizeEmblem.Assets.Scripts.Calculators;
using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.GameUnits.AbilityEffects;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.GameData;
using SizeEmblem.Scripts.GameUnits;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameUnits
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
                _mapPoint.X = value;
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
                _mapPoint.Y = value;
                UpdateZValue(-value);
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

        private MapPoint _mapPoint = new MapPoint();
        public MapPoint MapPoint { get { return _mapPoint; } }

        public void RefreshMapPoint()
        {
            _mapPoint.X = MapX;
            _mapPoint.Y = MapY;
            _mapPoint.Width = TileWidth;
            _mapPoint.Height = TileHeight;
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
            if (HP <= 0) return false; // temp flag for KO'd units, should be replaced in the future with something smarter
            if (ActionOver) return false;
            return true;
        }

        public bool CanMoveAction()
        {
            if (CanAct() && HasRemainingMovement() && !MovementActionConsumed) return true;
            return false;
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
            // Units start fully healed
            HealHPSPFull();

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


        /// <summary>
        /// Check if this unit has any movement remaining.
        /// </summary>
        /// <returns></returns>
        public bool HasRemainingMovement()
        {
            return RemainingMovement > 0;
        }

        public void AddRouteCost(IGameMapMovementRoute route)
        {
            // Units can only move once for now
            SpentMovement = 0;
            MovementActionConsumed = true;
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
            if (!Statistics.ContainsKey(statistic))
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

        public ulong DecrementStatistic(UnitStatistic statistic, ulong value)
        {
            // Check if we have a value for this statistic already. If we don't we can't decrement it so just assign it to zero and return it
            if (!Statistics.ContainsKey(statistic))
            {
                _statistics.Add(statistic, 0);
                return 0;
            }

            // Underflow check
            if(value > _statistics[statistic])
            {
                _statistics[statistic] = 0;
                return 0;
            }

            // Update value of the statistic but subtracting it from the given value
            var newValue = _statistics[statistic] - value;
            _statistics[statistic] = newValue;
            return newValue;
        }

        #endregion



        public SizeCategory SizeCategory { get { return BaseUnitData.SizeCategory; } }
        public WeaponAdvantageCategory WeaponAdvantageCategory { get { return BaseUnitData.WeaponType; } }

        public GameUnitData baseUnitData;
        public GameUnitData BaseUnitData { get { return baseUnitData; } }

        public void SetBaseUnit(GameUnitData baseUnit)
        {
            baseUnitData = baseUnit;
            RefreshUnit();
        }

        public void RefreshUnit()
        {
            RefreshMovementTypes();
            RefreshSprite();
            RefreshMapPoint();
        }


        #region Attribute Functions

        public int level = 1;
        public int Level { get { return level; } }


        [SerializeField]
        private int _hp;
        public int HP { get { return _hp; } }

        [SerializeField]
        private int _sp;
        public int SP { get { return _sp; } }


        public List<IAbility> Abilities { get; } = new List<IAbility>();

        public bool CanUseAbility(IAbility ability)
        {
            // Units that can't act can't use any abilities
            if (!CanAct()) return false;
            // Make sure this unit owns this ability instance
            if (!Abilities.Contains(ability)) return false;

            // Ask the ability itself if it can be used. It'll check HP/SP cost, cooldown/warmup state, etc.
            return ability.CanUseAbility();
        }


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

        public int GetAttributeBase(UnitAttribute attribute)
        {
            switch (attribute)
            {
                case UnitAttribute.None:               return 0;
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

        public void HealHPSPFull()
        {
            _hp = GetAttribute(UnitAttribute.MaxHP);
            _sp = GetAttribute(UnitAttribute.MaxSP);
        }

        #endregion


        #region Details


        public HeightContainer Height { get { return MeasurementCalculator.GetHeightContainer(BaseUnitData.Height); } }
        public bool ShowHeight { get { return BaseUnitData.ShowHeight; } }

        public WeightContainer Weight { get { return MeasurementCalculator.GetScaledWeightContainer(BaseUnitData.Weight, BaseUnitData.Height); } }
        public bool ShowWeight { get { return BaseUnitData.ShowWeight; } }


        #endregion



        #region Drawing & Graphics Related

        public SpriteRenderer GetSpriteRenderer()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            return _spriteRenderer;
        }

        public void UpdateZValue(int newOrder)
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
            if (TileWidth <= 0 || TileHeight <= 0)
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

            if (_lastActionOver != ActionOver)
            {
                _spriteRenderer.color = ActionOver ? Color.grey : Color.white;
                _lastActionOver = ActionOver;
            }

        }

        #endregion


        public void Initialize()
        {
            // Initialize properties that are normally only updated on property changed
            RefreshMovementTypes();
            RefreshMapPoint();


            if(this.UnitFaction == Faction.PlayerFaction)
            {
                var stompDamageEffectParameters = new DamageEffectParameters();
                stompDamageEffectParameters.SizeBiggerDamageMultiplier = 1.5f;
                stompDamageEffectParameters.SizeSmallerDamageMultiplier = 0.8f;
                stompDamageEffectParameters.DamagePairs = new[] { new DamageEffectPairParameter { OffensiveAttribute = UnitAttribute.Strength, OffensiveAttributeMultiplier = 1, DefensiveAttribute = UnitAttribute.Defense, DefensiveAttributeMultiplier = 1 } };
                stompDamageEffectParameters.TileDamage = 1;

                var abilityDataStomp = new AbilityData()
                {
                    IDName = "STOMP",
                    FriendlyName = "Stomp",
                    FriendlyDescription = "Deal extra damage to smaller units",
                    AbilityCategory = AbilityCategory.Attack,
                    WeaponCategory = WeaponAdvantageCategory.Physical,
                    RangeDistanceRule = AbilityRangeDistanceRule.SizeRangeSmall,
                    RangeSpecialRule = AbilityRangeSpecialRule.None,
                    MinRange = 1,
                    MaxRange = 1,
                    MinorActionConsumptionID = String.Empty,
                    HPCost = 0,
                    SPCost = 0,
                    DurabilityCost = 0,
                    Accuracy = 90,
                    TargetRule = AbilityTargetRule.EnemiesOnly,
                    AreaPoints = new[] { new MapPoint(0, 0, 1, 1) },
                    AbilityEffects = new[] { new DamageEffect(stompDamageEffectParameters) },
                };
                var abilityStomp = new Ability(this, abilityDataStomp);
                Abilities.Add(abilityStomp);


                var kickDamageEffectParameters = new DamageEffectParameters();
                kickDamageEffectParameters.SizeBiggerDamageMultiplier = 0.8f;
                kickDamageEffectParameters.SizeSmallerDamageMultiplier = 1.5f;
                kickDamageEffectParameters.DamagePairs = new[] { new DamageEffectPairParameter { OffensiveAttribute = UnitAttribute.Strength, OffensiveAttributeMultiplier = 1, DefensiveAttribute = UnitAttribute.Defense, DefensiveAttributeMultiplier = 1 } };
                kickDamageEffectParameters.TileDamage = 1;

                var abilityDataKick = new AbilityData()
                {
                    IDName = "KICK",
                    FriendlyName = "Kick",
                    FriendlyDescription = "Deal extra damage to larger units",
                    AbilityCategory = AbilityCategory.Attack,
                    WeaponCategory = WeaponAdvantageCategory.Physical,
                    RangeDistanceRule = AbilityRangeDistanceRule.SizeRangeSmall,
                    RangeSpecialRule = AbilityRangeSpecialRule.None,
                    MinRange = 1,
                    MaxRange = 1,
                    MinorActionConsumptionID = String.Empty,
                    HPCost = 0,
                    SPCost = 0,
                    DurabilityCost = 0,
                    Accuracy = 90,
                    TargetRule = AbilityTargetRule.EnemiesOnly,
                    AreaPoints = new[] { new MapPoint(0, 0, 1, 1) },
                    AbilityEffects = new[] { new DamageEffect(kickDamageEffectParameters) },
                };
                var abilityKick = new Ability(this, abilityDataKick);
                Abilities.Add(abilityKick);


                var abilityDataRapidJab = new AbilityData()
                {
                    IDName = "SCHOOL_RapidJab",
                    FriendlyName = "Rapid Jab",
                    FriendlyDescription = "Attack multiple times",
                    AbilityCategory = AbilityCategory.Special,
                    WeaponCategory = WeaponAdvantageCategory.Physical,
                    RangeDistanceRule = AbilityRangeDistanceRule.SizeRangeSmall,
                    RangeSpecialRule = AbilityRangeSpecialRule.None,
                    MinRange = 1,
                    MaxRange = 1,
                    MinorActionConsumptionID = String.Empty,
                    HPCost = 0,
                    SPCost = 3,
                    DurabilityCost = 0,
                    Accuracy = 80,
                    TargetRule = AbilityTargetRule.EnemiesOnly,
                    AreaPoints = new[] { new MapPoint(0, 0, 1, 1) },
                };
                var abilityRapidJab = new Ability(this, abilityDataRapidJab);
                Abilities.Add(abilityRapidJab);

                var abilityDataRoundhouse = new AbilityData()
                {
                    IDName = "SCHOOL_Roundhouse",
                    FriendlyName = "Roundhouse",
                    FriendlyDescription = "Hit all enemies around user",
                    AbilityCategory = AbilityCategory.Special,
                    WeaponCategory = WeaponAdvantageCategory.Physical,
                    RangeDistanceRule = AbilityRangeDistanceRule.SizeRangeSmall,
                    RangeSpecialRule = AbilityRangeSpecialRule.Directional,
                    MinRange = 1,
                    MaxRange = 1,
                    MinorActionConsumptionID = String.Empty,
                    HPCost = 0,
                    SPCost = 2,
                    DurabilityCost = 0,
                    Accuracy = 95,
                    TargetRule = AbilityTargetRule.EnemiesOnly,
                    AreaPoints = new[] { new MapPoint(0, 0, 1, 1) },
                };
                var abilityRoundhouse = new Ability(this, abilityDataRoundhouse);
                Abilities.Add(abilityRoundhouse);
            }
            else
            {
                var abilityDataTankShot = new AbilityData()
                {
                    IDName = "TANK_SHOT",
                    FriendlyName = "Cannon Fire",
                    FriendlyDescription = "Not canon fire",
                    AbilityCategory = AbilityCategory.Attack,
                    WeaponCategory = WeaponAdvantageCategory.Weapon,
                    RangeDistanceRule = AbilityRangeDistanceRule.Basic,
                    RangeSpecialRule = AbilityRangeSpecialRule.None,
                    MinRange = 1,
                    MaxRange = 3,
                    MinorActionConsumptionID = String.Empty,
                    HPCost = 0,
                    SPCost = 0,
                    DurabilityCost = 0,
                    Accuracy = 90,
                    TargetRule = AbilityTargetRule.EnemiesOnly,
                    AreaPoints = new[] { new MapPoint(0, 0, 1, 1) },
                };
                var abilityTankShot = new Ability(this, abilityDataTankShot);
                Abilities.Add(abilityTankShot);
            }

            
        }



        public void Start()
        {
            var spriteRenderer = GetSpriteRenderer();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = BaseUnitData.unitSprite;
                AlignSpriteRenderer();
            }

            Initialize();
        }


        public void Update()
        {
            UpdateSprite();
        }
    }
}
