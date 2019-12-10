using SizeEmblem.Assets.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.GameData;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.GameUnits
{
    public class Ability : IAbility
    {
        public AbilityData Data { get; }
        private IGameUnit _owner;
        private int _equipmentSource;


        public Ability(IGameUnit owner, AbilityData sourceData, int equipmentSource) : this(owner, sourceData)
        {
            _equipmentSource = equipmentSource;
        }

        public Ability(IGameUnit owner, AbilityData sourceData)
        {
            if (owner == null) throw new ArgumentNullException("Could not instanciate new instance of ability", nameof(owner));
            if (sourceData == null) throw new ArgumentNullException("Could not instanciate new instance of Ability", nameof(sourceData));

            _owner = owner;
            Data = sourceData;
        }



        public string IDName { get { return Data.IDName; } }

        public string FriendlyName { get { return Data.FriendlyName; } }

        public ILocalizationString NameLocal { get; }




        public WeaponAdvantageCategory WeaponCategory { get { return Data.WeaponCategory; } }
        public AbilityCategory AbilityCategory { get { return Data.AbilityCategory; } }


        // Ability Effects
        public IAbilityEffect[] AbilityEffects { get { return Data.AbilityEffects; } }


        // Ability Accuracy
        public bool SkipAccuracyCheck { get { return Data.SkipAccuracyCheck; } }

        public int Accuracy { get { return Data.Accuracy; } }

        // Scope Properties
        public AbilityTargetRule TargetRule { get { return Data.TargetRule; } }

        // Range Properties
        public AbilityRangeDistanceRule RangeDistanceRule { get { return Data.RangeDistanceRule; } }
        public AbilityRangeSpecialRule RangeSpecialRule { get { return Data.RangeSpecialRule; } }
        public RangeValue<int> RangeMinMax { get { return new RangeValue<int>(Data.MinRange, Data.MaxRange); } }

        // Area of Effect Properties
        public IEnumerable<MapPoint> AreaPoints { get { return Data.AreaPoints; } }

        // Repeat Count
        public int RepeatCount { get { return Data.RepeatCount; } }
        public int RepeatThreshold { get { return Data.RepeatThreshold; } }

        public AbilityActionConsumption ActionConsumption { get { return Data.ActionConsumption; } }
        public string MinorActionConsumptionID { get { return Data.MinorActionConsumptionID; } }


        // Counter flags
        public bool CanBeCountered { get { return Data.CanBeCountered; } }
        public bool CanCounterAttack { get { return Data.CanCounterAttack; } }



        // Cost Properties
        public int HPCost { get { return Data.HPCost; } }
        public int SPCost { get { return Data.SPCost; } }



        public void ProcessTurnEnd()
        {
            if(InCooldownState())
            {
                CooldownRemaining = Math.Max(CooldownRemaining - 1, 0);
            }

            if(InWarmupState())
            {
                WarmupRemaining = Math.Max(WarmupRemaining - 1, 0);
            }
        }

        /// <summary>
        /// Function for handling when this ability is used.
        /// This will decrement uses and cooldown and any other on-use state chagnes for this ability
        /// </summary>
        public void ProcessUsed()
        {
            DecrementUses(1);
            ReduceCooldown(1);
        }


        /// <summary>
        /// Function for resetting the restrictions on using this specific ability.
        /// This pertains to Limited Uses per Battle, Cooldowns, and Warmups
        /// </summary>
        public void ResetBattleUsages()
        {
            ResetBattleUses();
            ResetCooldown();
            ResetWarmup();
        }


        /// <summary>
        /// Check to see if the owner unit of this ability can use this ability.
        /// This assumes the owner can use abilities in general. This check is _only_ for this specific skill, it's own requirements and conditions
        /// </summary>
        /// <returns>True if this ability can be used. False otherwise.</returns>
        public bool CanUseAbility()
        {
            // Check all of our conditions. If any fail then this ability can't be used
            if (Data.HPCost > 0 && _owner.HP <= Data.HPCost) return false;
            if (Data.SPCost > 0 && _owner.SP < Data.SPCost) return false;
            // Something to check for weapon durability?
            if (HasUsesPerBattle() && UsesRemaining == 0) return false;
            if (InCooldownState()) return false;
            if (InWarmupState()) return false;

            // Fallthrough: All our conditions passed so this ability can be used
            return true;
        }


        #region Limited Number of Uses Per Battle

        public int UsesRemaining { get; private set; }

        public bool HasUsesPerBattle()
        {
            return Data.HasPerBattleUses;
            //return UsesRemaining >= 0;
        }


        public void ResetBattleUses()
        {
            if (Data.HasPerBattleUses) UsesRemaining = Data.PerBattleUses;
            else UsesRemaining = -1;
        }

        public void DecrementUses(int uses)
        {
            if(HasUsesPerBattle())
                UsesRemaining = Math.Max(UsesRemaining - uses, 0);
        }

        #endregion




        #region Cooldown

        public int CooldownRemaining { get; private set; }

        public bool InCooldownState()
        {
            return CooldownRemaining > 0;
        }

        public void ResetCooldown()
        {
            CooldownRemaining = 0;
        }

        public void ApplyCooldown()
        {
            if(Data.HasCooldown)
                SetCooldownTurns(Data.CooldownTurns);
        }

        public void SetCooldownTurns(int cooldownTurns)
        {
            cooldownTurns = Math.Max(cooldownTurns, 0);
            CooldownRemaining = cooldownTurns;
        }

        public void ReduceCooldown(int turns)
        {
            if (InCooldownState())
                CooldownRemaining = Math.Max(CooldownRemaining - turns, 0);
        }

        #endregion



        #region Warmup

        public int WarmupRemaining { get; private set; }

        public bool InWarmupState()
        {
            if (!Data.HasWarmup) return false;
            return WarmupRemaining > 0;
        }

        public void ResetWarmup()
        {
            var warmupTurns = Data.WarmupTurns;
            warmupTurns = Math.Max(warmupTurns, 0);
            WarmupRemaining = warmupTurns;
        }

        #endregion

    }
}
