using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Calculators
{
    public class DamageCalculator
    {
        public DamageResultContainer CalculateDamage(IAbility ability, IGameUnit attacker, IGameUnit target)
        {
            var damageContainer = new DamageResultContainer();

            // Calculate our base damage:
            damageContainer.BaseDamage = CalculateBaseDamage(ability, attacker, target);

            // See if this hit doubles
            damageContainer.RepeatCount = WillDouble(ability, attacker, target) ? 2 : 1;

            // Apply weapon advantage
            var triangleModifier = GetWeaponAdvantageModifier(ability, attacker, target);
            if (triangleModifier != null) damageContainer.DamageModifiers.Add(triangleModifier);

            return damageContainer;
        }

        public DamageResultContainer CalculateEstimatedDamage()
        {
            return null;
        }



        public int CalculateBaseDamage(IAbility ability, IGameUnit attacker, IGameUnit target)
        {
            var strengthDamage = (int)Math.Max(ability.StrengthMultiplier * attacker.GetAttribute(UnitAttribute.Strength) - target.GetAttribute(UnitAttribute.Defense), 0);
            // TODO: Physical damage mitigation

            var magicDamage    = (int)Math.Max(ability.MagicMultiplier * attacker.GetAttribute(UnitAttribute.Magic) - target.GetAttribute(UnitAttribute.Resistance), 0);
            // TODO: Magical damage mitigation

            return strengthDamage + magicDamage;
        }


        public bool WillDouble(IAbility ability, IGameUnit attacker, IGameUnit target)
        {
            if (!ability.CanDouble) return false;

            var attackerSpeed = attacker.GetAttribute(UnitAttribute.Speed);
            var defenderSpeed = target.GetAttribute(UnitAttribute.Speed);

            return (attackerSpeed - defenderSpeed) > 5;
        }


        #region Weapon Advantage Triangle

        public DamageModifierContainer GetWeaponAdvantageModifier(IAbility ability, IGameUnit attacker, IGameUnit target)
        {
            // Get the direction of the advantage of this ability used by the attacker vs the target
            var direction = GetWeaponAdvantageDirection(GetCategoryForAbility(ability, attacker), target.WeaponAdvantageCategory);
            // If there's no advantage thent here's no modifier to apply for weapon triangle purposes
            if (direction == 0) return null;

            // Otherwise create a container based on weapon advantage
            var modifier = new DamageModifierContainer() { Name = "WEAPON_ADVANTAGE" };
            if(direction > 0)
            {
                // Weapon advantage for attacker
                modifier.DamageMultiplier = 1.1f;
                modifier.HitRateModifier = 10;
            }
            else
            {
                // Weapon advantage for target
                modifier.DamageMultiplier = 0.8f;
                modifier.HitRateModifier = -10;

            }
            return modifier;
        }


        public WeaponAdvantageCategory GetCategoryForAbility(IAbility ability, IGameUnit caster)
        {
            if (ability.WeaponCategory == WeaponAdvantageCategory.Inherit) return caster.WeaponAdvantageCategory;
            return ability.WeaponCategory;
        }


        /// <summary>
        /// Calculate the weapon advantage triangle of two categories.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns>The direction of advantage. >0 means attackers advtange, <0 means defender's advantage, 0 means no advantage. Value 1 means normal triangle advantage, 2 means perfect/worst weapon category advantage.</returns>
        public int GetWeaponAdvantageDirection(WeaponAdvantageCategory attacker, WeaponAdvantageCategory defender)
        {
            // First check: If the categories are equal then there's no advantage
            if (attacker == defender) return 0;

            // Check for NONE types, which exist outside the triangle and cannot have advantage in the scope of this logic
            if (attacker == WeaponAdvantageCategory.None || defender == WeaponAdvantageCategory.None) return 0;

            // Check for perfect/worst conditions before we check the triangle itself
            if (attacker == WeaponAdvantageCategory.Perfect || defender == WeaponAdvantageCategory.Worst)   return  2;
            if (attacker == WeaponAdvantageCategory.Worst   || defender == WeaponAdvantageCategory.Perfect) return -2;

            // Triangle checks:  Physical beats Weapon beats Magic beats Physical
            if (TriangleCheck(attacker, defender) > 0) return 1;
            if (TriangleCheck(defender, attacker) > 0) return -1;

            // Fallthrough: There's no advantage
            return 0;
        }

        /// <summary>
        /// Weapon Advantage Triangle check only: This will only check if A has advantage over B within the confines of the advantage triangle
        /// </summary>
        /// <param name="a">The category to check if it has advantage over B</param>
        /// <param name="b">The category to check if against A</param>
        /// <returns>1 if <paramref name="a"/> has advantage over <paramref name="b"/>. 0 Otherwise.</returns>
        public int TriangleCheck(WeaponAdvantageCategory a, WeaponAdvantageCategory b)
        {
            if (a == WeaponAdvantageCategory.Physical && b == WeaponAdvantageCategory.Weapon) return 1;
            if (a == WeaponAdvantageCategory.Weapon && b == WeaponAdvantageCategory.Magic) return 1;
            if (a == WeaponAdvantageCategory.Magic && b == WeaponAdvantageCategory.Physical) return 1;

            return 0;
        }

        #endregion

    }
}
