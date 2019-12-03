using SizeEmblem.Scripts.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SizeEmblem.Scripts.GameData
{
    [Serializable]
    public class AbilityData
    {
        public string IDName;

        // Ability Name
        public string FriendlyName;
        public int LocalizationNameID;

        // Description
        public string FriendlyDescription;
        public int LocalizationDescriptionID;

        // Ability Category and Action Type
        public AbilityCategory AbilityCategory;
        public WeaponAdvantageCategory WeaponCategory;

        // Scope of Ability
        public AbilityTargetRule TargetRule;

        // Range
        public AbilityRangeDistanceRule RangeDistanceRule;
        public AbilityRangeSpecialRule RangeSpecialRule;

        public int MinRange;
        public int MaxRange;

        // AoE
        public int[] AreaPoints;


        // ID for what minor action this ability consumes, NULL for none
        public string MinorActionConsumptionID;

        // Ability Cost
        public int HPCost;
        public int SPCost;
        public int DurabilityCost;  // Not sure how I'd link this to a weapon yet

        // Ability Restrictions
        public bool HasPerBattleUses;
        public int PerBattleUses;

        public bool HasCooldown;
        public int CooldownTurns;

        public bool HasWarmup;
        public int WarmupTurns;

        // Base Accuracy
        public int Accuracy;
        public bool SkipAccuracyCheck;

    }
}