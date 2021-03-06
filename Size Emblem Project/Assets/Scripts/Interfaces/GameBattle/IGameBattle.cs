﻿using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.GameBattle
{
    public interface IGameBattle
    {
        IReadOnlyList<IGameUnit> ImmediateActionQueue { get; }

        Faction CurrentPhase { get; }


        void StartBattle();

        void StartTurn();
        void EndTurn();

        void StartPhase(Faction faction);
        void EndPhase();

        void StartImmediateAction();
        void EndImmediateAction();


        bool IsUnitsPhase(IGameUnit unit);

        bool IsPlayerEnabledPhase();


        // Input Management
        void AddInputState(IInputState nextInputState);
        bool CurrentInputState(out IInputState currentState);

        void ResetInputStack();
        void ClearInputStack();
        void ClearTopInputState();

        // Execute Abilities
        void ExecuteAbility(IGameUnit user, IAbility ability, MapPoint targetPoint);
    }
}
