﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Containers
{
    public class AbilityResultContainer
    {
        public bool Successful;

        public int BaseDamage;
        public readonly List<DamageModifierContainer> DamageModifiers = new List<DamageModifierContainer>();

        public int RepeatCount; // Number of times the action repeats, 2 for double attack for example

        public int HitRate;
        public bool IsHit;
        public bool IsCrit;

        // Flag for which direction, if any, for advantage of this damage. >0 means advantage for the attacker, <0 means advantage for the target, 0 means no advantage
        public int Advantage;

        public int CalculateDamage()
        {
            //if (!IsHit) return 0;

            var baseDamage = BaseDamage;

            var multipliersProduct = DamageModifiers.Aggregate(1.0f, (product, modifier) => product * modifier.DamageMultiplier);
            var modifierSum = DamageModifiers.Sum(x => x.DamageModifier);

            return (int)((baseDamage + modifierSum) * multipliersProduct);
        }
    }
}
