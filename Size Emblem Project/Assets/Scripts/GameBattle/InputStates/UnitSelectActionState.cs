using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Constants;
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
    public class UnitSelectActionState : IInputState
    {
        private IGameBattle _gameBattle;
        private IGameMap _gameMap;

        private IGameUnit _unit;
        private IUnitActionWindow _unitActionWindow;

        private IInputStateFactory _inputStateFactory;

        public UnitSelectActionState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unit, IUnitActionWindow unitActionWindow, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;
            _unit = unit;
            _unitActionWindow = unitActionWindow;

            _inputStateFactory = inputStateFactory;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            _unitActionWindow.IsVisible = true;

            _unitActionWindow.AttackSelected  += UnitActionWindow_AttackSelected;
            _unitActionWindow.SpecialSelected += UnitActionWindow_SpecialSelected;
            _unitActionWindow.DefendSelected  += UnitActionWindow_DefendSelected;
            _unitActionWindow.ItemSelected    += UnitActionWindow_ItemSelected;
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
            _unitActionWindow.IsVisible = false;

            _unitActionWindow.AttackSelected  -= UnitActionWindow_AttackSelected;
            _unitActionWindow.SpecialSelected -= UnitActionWindow_SpecialSelected;
            _unitActionWindow.DefendSelected  -= UnitActionWindow_DefendSelected;
            _unitActionWindow.ItemSelected    -= UnitActionWindow_ItemSelected;

        }


        private void UnitActionWindow_AttackSelected()
        {
            var nextInputState = _inputStateFactory.ResolveUnitSelectAbilityState(_unit, AbilityCategory.Attack);
            _gameBattle.AddInputState(nextInputState);
        }

        private void UnitActionWindow_SpecialSelected()
        {
            var nextInputState = _inputStateFactory.ResolveUnitSelectAbilityState(_unit, AbilityCategory.Special);
            _gameBattle.AddInputState(nextInputState);
        }


        private void UnitActionWindow_DefendSelected()
        {
            _unit.EndAction();
            _gameBattle.ResetInputStack();
        }

        private void UnitActionWindow_ItemSelected()
        {
            throw new NotImplementedException();
        }

        

        

        
    }
}
