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


        SizeCategory SizeCategory { get; }
        WeaponAdvantageCategory WeaponAdvantageCategory { get; }




        IReadOnlyDictionary<UnitStatistic, ulong> Statistics { get; }
        ulong SetStatistic(UnitStatistic statistic, ulong value);
        ulong IncrementStatistic(UnitStatistic statistic, ulong value);


        int HP { get; }
        int SP { get; }
        int GetAttribute(UnitAttribute attribue);

        // Abilities
        List<IAbility> Abilities { get; }
        bool CanUseAbility(IAbility ability);


        


        // Combat State
        bool IsActive { get; set; }
        bool MovementActionConsumed { get; set; }
        bool MinorActionConsumed { get; set; }
        bool MajorActionConsumed { get; set; }
        bool ActionOver { get; set; }

        bool CanAct();
        bool CanMoveAction();
        void ResetActionsConsumed();

        void EndAction();


        void ProcessBattleStart();
        void ProcessBattleEnd();

        void ProcessTurnStart();
        void ProcessTurnEnd();


        void ProcessPhaseStart();
        void ProcessPhaseEnd();

        // Movement State
        int MaxMovement { get; }
        int SpentMovement { get; }
        int RemainingMovement { get; }
        IReadOnlyList<MovementType> MovementTypes { get; }

        bool HasRemainingMovement();
        void AddRouteCost(IGameMapMovementRoute route);
    }
}
