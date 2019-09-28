using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum WeaponAdvantageCategory
    {
        None, // Exists outside the triangle, does not get advantage or disadvantage
        Physical, // Beats weapon, loses to magic
        Weapon,  // Beats magic, loses to physical
        Magic, // Beats physical, loses to weapon
        Perfect, // Always gets advantage
        Worst, // Always gets disadvantage

        Inherit = 100, // Special flag intended for abilities
    }
}
