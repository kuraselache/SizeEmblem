﻿using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
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
    public class PreviewMoveUnitState : IInputState
    {

        private readonly IGameBattle _gameBattle;
        private readonly IGameMap _gameMap;

        private readonly IGameUnit _unit;
        private readonly IUnitSummaryWindow _unitSummaryWindow;

        public PreviewMoveUnitState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unit, IUnitSummaryWindow unitSummaryWindow)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;

            _unit = unit;
            _unitSummaryWindow = unitSummaryWindow;
        }

        public bool IsActive { get; private set; }

        

        public void Activate()
        {
            IsActive = true;

            _gameMap.ShowUnitMovementRange(_unit);
            _gameMap.IsCursorEnabled = true;
            _unitSummaryWindow.IsEnabled = true;
            _unitSummaryWindow.IsVisible = true;
            _unitSummaryWindow.SelectedUnit = _unit;
        }

        public void UpdateState()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _gameBattle.ClearTopInputState();
                return;
            }
        }



        public void Deactivate()
        {
            _gameMap.ClearMovementOverlay();
            _gameMap.IsCursorEnabled = false;
            _unitSummaryWindow.IsEnabled = false;
            _unitSummaryWindow.IsVisible = false;
            _unitSummaryWindow.SelectedUnit = null;
        }
    }
}
