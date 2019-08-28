using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Interfaces.GameUnits
{
    public interface IGameUnit : IGameMapObject
    {
        string UnitName { get; }

        ILocalizationString NameLocalized { get; }

        Faction UnitFaction { get; set; }

        int BaseMovement { get; }
        int MaxMovement { get; }
        int SpentMovement { get; }
        int RemainingMovement { get; }

        IReadOnlyList<MovementType> MovementTypes { get; }


        ulong DestructionTotal { get; set; }
        ulong BodyCount { get; set; }

        int GetAttribute(UnitAttribute attribue);

        List<IAbility> Abilities { get; }


        bool CanMove();
    }
}
