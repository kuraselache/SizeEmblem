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


        AbilityData Data { get; }


        WeaponCategory WeaponCategory { get; }
        AbilityCategory AbilityCategory { get; }


        AbilityRangeDistanceRule RangeDistanceRule { get; }

        RangeValue<int> RangeMinMax { get; }

        
        

        


        void ResetBattleUsages();

        void ProcessTurnEnd();

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