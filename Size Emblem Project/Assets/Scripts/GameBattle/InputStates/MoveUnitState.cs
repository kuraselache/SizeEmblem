using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Events.GameMap;
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
    public class MoveUnitState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameMap _gameMap;

        private readonly IGameUnit _unit;
        private readonly IUnitSummaryWindow _unitSummaryWindow;

        private readonly IInputStateFactory _inputStateFactory;

        public MoveUnitState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unit, IUnitSummaryWindow unitSummaryWindow, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;

            _unit = unit;
            _unitSummaryWindow = unitSummaryWindow;

            _inputStateFactory = inputStateFactory;
        }

        public bool IsActive { get; set; }

        public void Activate()
        {
            _gameMap.ShowUnitMovementRange(_unit);

            _gameMap.IsCursorEnabled = true;
            _unitSummaryWindow.IsEnabled = true;

            _gameMap.PlayerSelectedRoute += GameMap_PlayerSelectedRoute;
        }


        public void UpdateState()
        {
            // Back button hit: Undo this state
            if (Input.GetMouseButtonDown(1))
            {
                _gameBattle.ClearTopInputState();
                return;
            }

            // Check if the player is selecting the unit they're moving. If so then the user doesn't want to move the unit so we'll advance to the UnitSelectAction state
            if (Input.GetMouseButtonDown(0))
            {
                var result = _gameMap.GetUnitAtCursor(out var unit);
                if (!result) return;

                if(unit == _unit)
                {
                    _gameBattle.AddInputState(_inputStateFactory.ResolveUnitSelectActionState(_unit));
                }
            }
        }



        public void Deactivate()
        {
            _gameMap.ClearMovementOverlay();

            _gameMap.IsCursorEnabled = false;
            _unitSummaryWindow.IsEnabled = false;

            _gameMap.PlayerSelectedRoute -= GameMap_PlayerSelectedRoute;
        }


        private void GameMap_PlayerSelectedRoute(IGameMap map, RouteSelectedEventArgs e)
        {
            var route = e.Route;
            _gameBattle.AddInputState(_inputStateFactory.ResolveMovingUnitState(_unit, route));
        }
    }
}
