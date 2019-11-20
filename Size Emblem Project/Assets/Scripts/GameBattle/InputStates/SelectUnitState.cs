using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Events.GameMap;
using SizeEmblem.Scripts.Interfaces.GameMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class SelectUnitState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameMap _gameMap;
        private readonly IUnitSummaryWindow _unitSummaryWindow;

        private readonly IInputStateFactory _inputStateFactory;

        public SelectUnitState(IGameBattle gameBattle, IGameMap gameMap, IUnitSummaryWindow unitSummaryWindow, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;
            _unitSummaryWindow = unitSummaryWindow;
            _inputStateFactory = inputStateFactory;
        }

        public bool IsActive { get; set; }

        public void BeginState()
        {
            _gameMap.IsCursorEnabled = true;
            _unitSummaryWindow.IsEnabled = true;

            _gameMap.HoverUnitChanged += GameMap_HoverUnitChanged;
        }

        public void UpdateState()
        {
            // Back button hit: Go to the EndTurn state
            if (Input.GetMouseButtonDown(1))
            {
                _gameBattle.AddInputState(_inputStateFactory.ResolveEndTurnWindowState());
                return;
            }

            if(Input.GetMouseButtonDown(0))
            {
                var result = _gameMap.GetUnitAtCursor(out var unit);
                if (!result) return;

                if (_gameBattle.CanUnitAct(unit))
                {
                    var nextInputState = _inputStateFactory.ResolveMoveUnitState(unit);
                    _gameBattle.AddInputState(nextInputState);
                }
                else
                {
                    var nextInputState = _inputStateFactory.ResolvePreviewMoveUnitState(unit);
                    _gameBattle.AddInputState(nextInputState);
                }
            }

        }

        public void AdvanceState(IInputState nextState)
        {
            _gameMap.IsCursorEnabled = false;
            _unitSummaryWindow.IsEnabled = false;
        }

        public void DisposeState(IInputState nextTopState)
        {
            _gameMap.IsCursorEnabled = false;
            _unitSummaryWindow.IsEnabled = false;

            _gameMap.HoverUnitChanged -= GameMap_HoverUnitChanged;
        }


        private void GameMap_HoverUnitChanged(IGameMap map, UnitSelectedEventArgs e)
        {
            if (!IsActive) return;

            _unitSummaryWindow.SelectedUnit = e.Unit;
        }

    }
}
