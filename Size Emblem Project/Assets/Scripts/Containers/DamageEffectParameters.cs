using SizeEmblem.Scripts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Containers
{
    public class DamageEffectParameters
    {
        // Base damage of this ability. This ignores all attribiutes of both caster and target
        public int BaseDamage = 0;

        // Damage pairing
        public DamageEffectPairParameter[] DamagePairs;

        // Bonus multipliers:
        public float SizeBiggerDamageMultiplier = 1.0f; // Multiply damage by this value if the target is bigger than the target (1 means no effect)
        public float SizeSmallerDamageMultiplier = 1.0f; // Multiply damage by this value if the target is smaller than the target (1 means no effect)

        // Tile damage
        public int TileDamage = 0;
    }

    public class DamageEffectPairParameter
    {
        public UnitAttribute OffensiveAttribute;
        public float OffensiveAttributeMultiplier;

        public UnitAttribute DefensiveAttribute;
        public float DefensiveAttributeMultiplier;
    }
}
