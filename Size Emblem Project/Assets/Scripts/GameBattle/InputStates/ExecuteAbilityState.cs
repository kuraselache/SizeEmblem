using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class ExecuteAbilityState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameMap _gameMap;
        private readonly IGameUnit _unitCasting;
        private readonly IAbility _abilityTargeting;

        private readonly int _targetX;
        private readonly int _targetY;

        private readonly IInputStateFactory _inputStateFactory;

        public ExecuteAbilityState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unitCasting, IAbility abilityTargeting, int targetX, int targetY, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;
            _unitCasting = unitCasting;
            _abilityTargeting = abilityTargeting;
            _targetX = targetX;
            _targetY = targetY;

            _inputStateFactory = inputStateFactory;
        }


        public bool IsActive { get; private set; }

        public void Activate()
        {
            IsActive = true;

            // Determine who has been targeted by the given X,Y target of the ability
            _gameMap.FindMapObjectInBounds(out var foundObject, 0, 0);
        }


        public void UpdateState()
        {
            // Back button hit: Undo this state
            if (Input.GetMouseButtonDown(1))
            {
                _gameBattle.ClearTopInputState();
                return;
            }
        }


        public void Deactivate()
        {
            IsActive = false;

        }
    }
}
