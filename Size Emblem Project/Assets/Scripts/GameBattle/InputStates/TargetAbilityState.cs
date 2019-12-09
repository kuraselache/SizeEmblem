using SizeEmblem.Assets.Scripts.Calculators;
using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.GameMap;
using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
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
    public class TargetAbilityState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameMap _gameMap;
        private readonly IGameUnit _unitCasting;
        private readonly IAbility _abilityTargeting;

        private readonly IInputStateFactory _inputStateFactory;

        public TargetAbilityState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unitCasting, IAbility abilityTargeting, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;
            _unitCasting = unitCasting;
            _abilityTargeting = abilityTargeting;

            _inputStateFactory = inputStateFactory;
        }


        private GameMapAbilityRange _abilityRange;

        public bool IsActive { get; private set; }


        public void Activate()
        {
            IsActive = true;

            _abilityRange = new GameMapAbilityRange(_unitCasting, _abilityTargeting);

            _gameMap.SetAbilityRange(_abilityRange);
            _gameMap.IsCursorEnabled = true;
        }


        public void UpdateState()
        {
            // Back button hit: Undo this state
            if (Input.GetMouseButtonDown(1))
            {
                _gameBattle.ClearTopInputState();
                return;
            }

            if(Input.GetMouseButtonDown(0))
            {
                // Get the current cursor position
                var cursorPoint = _gameMap.GetCursorPosition();
                // Then check if it lines up with the current abilty ranges
                if(_abilityRange.Points.Any(x => x.SameOrigin(cursorPoint)))
                {
                    // We have a cursor and our target offsets, convert the offsets to actual map points
                    var targetPoints = _abilityTargeting.AreaPoints.Select(x => x.ApplyOffset(cursorPoint.X, cursorPoint.Y));
                    // Then see if there's any valid targets
                    var foundTargets = _gameMap.FindAllMapObjectsInBounds(out var targets, targetPoints);
                    var validTarget = AbilityTargetCalculator.CheckForValidTargets(_unitCasting, _abilityTargeting, targets);
                    if(validTarget)
                    {
                        _gameBattle.AddInputState(_inputStateFactory.ResolveExecuteAbilityState(_unitCasting, _abilityTargeting, cursorPoint.X, cursorPoint.Y));
                        //Debug.Log("Valid target found");
                    }
                    else
                    {
                        Debug.Log("No valid targets found");
                    }
                }
                else
                {
                    Debug.Log("Clicked out of bounds!");
                }
            }
        }


        public void Deactivate()
        {
            IsActive = false;

            _gameMap.ClearAbilityRange();
            _gameMap.IsCursorEnabled = false;
        }

    }
}
