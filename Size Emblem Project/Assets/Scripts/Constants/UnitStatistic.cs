using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum UnitStatistic
    {
        None,
        // Combat related statistics
        Combats = 1,

        // Movement statistics
        TilesMoved = 100,

        // Destruction statistics
        PropertyDamage = 200,
        BodyCount = 201,
    }
}
