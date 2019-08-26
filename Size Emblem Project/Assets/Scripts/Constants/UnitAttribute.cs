using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SizeEmblem.Scripts.Constants
{
    public enum UnitAttribute
    {
        None, // Special use-case flag for damage algorithms, don't use for setting stats of units
        MaxHP,
        MaxSP,
        Strength,
        Magic,
        Defense,
        Resistance,
        Skill,
        Speed,
        Luck,
        Movement,
        WalkingDamage,
        WalkingDamageRange,
    }
}
