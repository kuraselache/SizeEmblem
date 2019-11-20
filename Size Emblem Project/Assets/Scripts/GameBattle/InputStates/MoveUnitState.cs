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
    public class MoveUnitState : IInputState
    {
        private IGameBattle _gameBattle;
        private IGameMap _gameMap;

        private IGameUnit _unit;
        private IUnitSummaryWindow _unitSummaryWindow;

        public MoveUnitState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unit, IUnitSummaryWindow unitSummaryWindow)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;

            _unit = unit;
            _unitSummaryWindow = unitSummaryWindow;
        }

        public bool IsActive { get; set; }

        public void BeginState()
        {
            _gameMap.ShowUnitMovementRange(_unit);

            _gameMap.IsCursorEnabled = true;
            _unitSummaryWindow.IsEnabled = true;
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


        public void AdvanceState(IInputState nextState)
        {
        }


        public void DisposeState(IInputState nextTopState)
        {
            _gameMap.ClearMovementOverlay();

            _gameMap.IsCursorEnabled = false;
            _unitSummaryWindow.IsEnabled = false;
        }
    }
}
