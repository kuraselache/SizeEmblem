using SizeEmblem.Assets.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SizeEmblem.Scripts.Interfaces.GameUnits
{
    public interface IAbility
    {
        string IDName { get; }
        string FriendlyName { get; }
        ILocalizationString NameLocal { get; }




        WeaponAdvantageCategory WeaponCategory { get; }
        AbilityCategory AbilityCategory { get; }


        // Ability Effects
        IAbilityEffect[] AbilityEffects { get; }


        // Ability Accuracy
        bool SkipAccuracyCheck { get; } // Flag if accuracy checks should be skipped, such as for healing abilities

        int Accuracy { get; }



        // Ability Scope
        AbilityTargetRule TargetRule { get; }

        // Ability Range
        AbilityRangeDistanceRule RangeDistanceRule { get; }
        AbilityRangeSpecialRule RangeSpecialRule { get; }

        RangeValue<int> RangeMinMax { get; }

        // Ability AoE/Area
        IEnumerable<MapPoint> AreaPoints { get; }


        // Repeat Count
        int RepeatCount { get; }
        int RepeatThreshold { get; }


        // Action Consumption Flags
        AbilityActionConsumption ActionConsumption { get; }
        string MinorActionConsumptionID { get; }

        // Ability Cost
        int HPCost { get; }
        int SPCost { get; }



        void ResetBattleUsages();

        void ProcessTurnEnd();

        void ProcessUsed();


        bool CanUseAbility();

        // Special restriction rules
        int UsesRemaining { get; }
        int CooldownRemaining { get; }
        int WarmupRemaining { get; }

        bool InCooldownState();
        void ResetCooldown();
        void ApplyCooldown();
        void ReduceCooldown(int turns);
    }

}