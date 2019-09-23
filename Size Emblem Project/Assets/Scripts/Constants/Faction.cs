using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum Faction
    {
        UndefinedFaction = 0,
        PlayerFaction = 1,
        AlliedFaction = 2,
        CivilianFaction = 3, // Special faction, not aligned with any other faction, not considered as hostile for victory conditions, but is treated as hostile for AoE effects
        EnemyAlphaFaction = 10,
        EnemyBetaFaction = 11,
    }
}
