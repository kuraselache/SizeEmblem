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
        public AbilityCategory Category;

        public string ActionConsumptionSpecial;

        // Ability Cost
        public int HPCost;
        public int SPCost;
        public int DurabilityCost;  // Not sure how I'd link this to a weapon yet

        // Ability Restrictions
        public int PerBattleUses;
        public int CooldownTurns;
        public int WarmupTurns;


    }
}