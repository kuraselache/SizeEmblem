using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class UnitDetailsState : IInputState
    {

        private readonly IGameBattle _gameBattle;
        private readonly IGameUnit _unit;
        private readonly IUnitDetailsWindow _unitDetailsWindow;

        public UnitDetailsState(IGameBattle gameBattle, IGameUnit unit, IUnitDetailsWindow unitDetailsWindow)
        {
            _gameBattle = gameBattle;
            _unit = unit;
            _unitDetailsWindow = unitDetailsWindow;
        }


        public bool IsActive { get; private set; }


        public void Activate()
        {
            IsActive = true;
            _unitDetailsWindow.SelectedUnit = _unit;
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
            _unitDetailsWindow.IsVisible = false;
        }
    }
}
