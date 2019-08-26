using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum WeaponType
    {
        None, // Exists outside the triangle, does not get advantage or disadvantage
        Physical, // Beasts weapon, loses to magic
        Weapon,  // Beasts magic, loses to physical
        Magic, // Beats physical, loses to weapon
    }
}
