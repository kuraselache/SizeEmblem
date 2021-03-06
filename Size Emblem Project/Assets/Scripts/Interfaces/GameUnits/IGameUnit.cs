﻿using SizeEmblem.Assets.Scripts.Containers;
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




        ulong GetStatistic(UnitStatistic statistic);
        ulong SetStatistic(UnitStatistic statistic, ulong value);
        ulong IncrementStatistic(UnitStatistic statistic, ulong value);
        ulong DecrementStatistic(UnitStatistic statistic, ulong value);


        int Level { get; }
        int HP { get; }
        int SetHP(int newHP);

        int SP { get; }
        int SetSP(int newSP);



        // Combat Methods for HP/SP
        void TakeDamage(int damage);
        void Die();
        bool IsDead();

        void ConsumeAbilityCost(IAbility ability);

        // Attributes
        int GetAttribute(UnitAttribute attribue);

        HeightContainer Height { get; }
        bool ShowHeight { get; }

        WeightContainer Weight { get; }
        bool ShowWeight { get; }

        // Abilities
        List<IAbility> Abilities { get; }
        bool CanUseAbility(IAbility ability);



        // Combat State
        bool IsActive { get; set; }
        bool CanAct();

        bool MovementActionConsumed { get; set; }
        bool MinorActionConsumed { get; set; }
        bool MajorActionConsumed { get; set; }
        bool ActionOver { get; set; }

        bool CanTakeTurn();
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
