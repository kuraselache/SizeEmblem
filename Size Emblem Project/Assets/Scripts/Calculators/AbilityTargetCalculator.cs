using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Calculators
{
    public static class AbilityTargetCalculator
    {
        #region Filter Functions

        public static IReadOnlyCollection<IGameMapObject> FilterTargets(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            switch (usedAbility.TargetRule)
            {
                case AbilityTargetRule.Always: return FilterRuleAlways(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.AnyUnit: return FilterRuleAnyUnit(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.EnemiesOnly: return FilterRuleEnemiesOnly(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.AlliesOnly: return FilterRuleAlliesOnly(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.CasterOnly: return FilterRuleCasterOnly(castingUnit, usedAbility, targetedObjects);
            }

            // We shouldn't get here
            throw new NotImplementedException($"Could not filter targets for ability target rule: {usedAbility.TargetRule} on ability: {usedAbility.NameLocal}");
        }


        public static IReadOnlyCollection<IGameMapObject> FilterRuleAlways(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            return targetedObjects.ToList().AsReadOnly();
        }

        public static IReadOnlyCollection<IGameMapObject> FilterRuleAnyUnit(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            return targetedObjects.OfType<IGameUnit>().ToList().AsReadOnly();
        }

        public static IReadOnlyCollection<IGameMapObject> FilterRuleEnemiesOnly(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            var casterFaction = castingUnit.UnitFaction;
            return targetedObjects.OfType<IGameUnit>().Where(x => x.UnitFaction != casterFaction).ToList().AsReadOnly();
        }

        public static IReadOnlyCollection<IGameMapObject> FilterRuleAlliesOnly(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            var casterFaction = castingUnit.UnitFaction;
            return targetedObjects.OfType<IGameUnit>().Where(x => x.UnitFaction == casterFaction).ToList().AsReadOnly();
        }

        public static IReadOnlyCollection<IGameMapObject> FilterRuleCasterOnly(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            var list = new List<IGameMapObject>();
            list.Add(castingUnit);
            return list.AsReadOnly();
        }


        #endregion


        #region IsValid Functions

        public static bool CheckForValidTargets(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            switch(usedAbility.TargetRule)
            {
                case AbilityTargetRule.Always: return CheckRuleAlways(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.AnyUnit: return CheckRuleAnyUnit(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.EnemiesOnly: return CheckRuleEnemiesOnly(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.AlliesOnly: return CheckRuleAlliesOnly(castingUnit, usedAbility, targetedObjects);
                case AbilityTargetRule.CasterOnly: return CheckRuleCasterOnly(castingUnit, usedAbility, targetedObjects);
            }

            // We shouldn't get here
            throw new NotImplementedException($"Could not check for valid target for ability target rule: {usedAbility.TargetRule} on ability: {usedAbility.NameLocal}");
        }

        public static bool CheckRuleAlways(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            return true;
        }

        public static bool CheckRuleAnyUnit(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            return targetedObjects.OfType<IGameUnit>().Any();
        }

        public static bool CheckRuleEnemiesOnly(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            var casterFaction = castingUnit.UnitFaction;
            return targetedObjects.OfType<IGameUnit>().Any(x => x.UnitFaction != casterFaction);
        }

        public static bool CheckRuleAlliesOnly(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            var casterFaction = castingUnit.UnitFaction;
            return targetedObjects.OfType<IGameUnit>().Any(x => x.UnitFaction == casterFaction);
        }

        public static bool CheckRuleCasterOnly(IGameUnit castingUnit, IAbility usedAbility, IEnumerable<IGameMapObject> targetedObjects)
        {
            return targetedObjects.SingleOrDefault() == castingUnit;
        }

        #endregion

    }
}
