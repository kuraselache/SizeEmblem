using SizeEmblem.Assets.Scripts.Calculators;
using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameUnits.AbilityEffects
{
    public class DamageEffect : IAbilityEffect
    {
        private readonly DamageEffectParameters _damageParameters;

        public DamageEffect(DamageEffectParameters damageParameters)
        {
            _damageParameters = damageParameters;
        }


        public AbilityResultContainer PreviewResults(AbilityExecuteParameters parameters)
        {
            var results = new AbilityResultContainer();
            results.BaseDamage = CalculateDamage(parameters);
            CreateDamageModifiers(parameters, results);

            return results;
        }

        public AbilityResultContainer CreateResults(AbilityExecuteParameters parameters)
        {
            var previewResults = PreviewResults(parameters);

            // TODO: Actually add RNG/accuracy
            previewResults.Successful = true;

            return previewResults;
        }


        public void ExecuteEffect(AbilityExecuteParameters parameters, AbilityResultContainer results)
        {
            if (!results.Successful) return;

            var targetUnit = parameters.Target as IGameUnit;
            targetUnit.TakeDamage(results.CalculateDamage());
        }



        public int CalculateDamage(AbilityExecuteParameters parameters)
        {
            var user = parameters.UnitExecuting;
            var target = parameters.Target;
            
            // Set our current damage to the damage's base effect
            var damage = _damageParameters.BaseDamage;

            if(_damageParameters.DamagePairs != null && target is IGameUnit)
            {
                var targetUnit = parameters.Target as IGameUnit;

                foreach (var damagePair in _damageParameters.DamagePairs)
                {
                    var attack  = (int)(user.GetAttribute(damagePair.OffensiveAttribute) * damagePair.OffensiveAttributeMultiplier);
                    var defense = (int)(targetUnit.GetAttribute(damagePair.DefensiveAttribute) * damagePair.DefensiveAttributeMultiplier);
                    var pairDamage = Math.Max(attack - defense, 0);
                    damage += pairDamage;
                }
            }

            return damage;
        }

        public void CreateDamageModifiers(AbilityExecuteParameters parameters, AbilityResultContainer abilityResultContainer)
        {
            var user = parameters.UnitExecuting;
            if (!(parameters.Target is IGameUnit)) return;
            var target = parameters.Target as IGameUnit;

            var sizeDifference = SizeCalculator.GetSizeDifference(user.SizeCategory, target.SizeCategory);

            // Static size difference modifiers
            if(sizeDifference > 0)
            {
                var multiplier = Mathf.Pow(1.2f, sizeDifference);
                abilityResultContainer.DamageModifiers.Add(new DamageModifierContainer { DamageMultiplier = multiplier, Name = "Bigger Size Bonus" });
            }
            else if(sizeDifference < 0)
            {
                var multiplier = Mathf.Pow(0.8f, sizeDifference);
                abilityResultContainer.DamageModifiers.Add(new DamageModifierContainer { DamageMultiplier = multiplier, Name = "Smaller Size Bonus" });
            }

            // Ability-based Size Difference
            if (_damageParameters.SizeBiggerDamageMultiplier > 1 && sizeDifference > 0)
            {
                var multiplier = Mathf.Pow(_damageParameters.SizeBiggerDamageMultiplier, sizeDifference);
                abilityResultContainer.DamageModifiers.Add(new DamageModifierContainer { DamageMultiplier = multiplier, Name = "Ability Bigger Size Bonus" });
            }
            else if(_damageParameters.SizeBiggerDamageMultiplier < 1 && sizeDifference > 0)
            {
                var multiplier = Mathf.Pow(_damageParameters.SizeBiggerDamageMultiplier, sizeDifference);
                abilityResultContainer.DamageModifiers.Add(new DamageModifierContainer { DamageMultiplier = multiplier, Name = "Ability Bigger Size Penalty" });
            }


            if(_damageParameters.SizeSmallerDamageMultiplier > 1 && sizeDifference < 0)
            {
                var multiplier = Mathf.Pow(_damageParameters.SizeSmallerDamageMultiplier, -sizeDifference);
                abilityResultContainer.DamageModifiers.Add(new DamageModifierContainer { DamageMultiplier = multiplier, Name = "Ability Smaller Size Bonus" });
            }
            else if (_damageParameters.SizeSmallerDamageMultiplier < 1 && sizeDifference < 0)
            {
                var multiplier = Mathf.Pow(_damageParameters.SizeSmallerDamageMultiplier, -sizeDifference);
                abilityResultContainer.DamageModifiers.Add(new DamageModifierContainer { DamageMultiplier = multiplier, Name = "Ability Smaller Size Penalty" });
            }
        }
    }
}
