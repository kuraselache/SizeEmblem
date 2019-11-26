using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class UnitSelectAbilityState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameUnit _unit;
        private AbilityCategory _abilityCategory;

        private readonly IUnitAbilitiesWindow _selectedUnitAbilitiesWindow;

        private readonly IInputStateFactory _inputStateFactory;

        public UnitSelectAbilityState(IGameBattle gameBattle, IGameUnit unit, AbilityCategory abilityCategory, IUnitAbilitiesWindow selectedUnitAbilitiesWindow, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _unit = unit;
            _abilityCategory = abilityCategory;
            _selectedUnitAbilitiesWindow = selectedUnitAbilitiesWindow;

            _inputStateFactory = inputStateFactory;

        }


        public bool IsActive { get; private set; }

        
        public void Activate()
        {
            IsActive = true;

            _selectedUnitAbilitiesWindow.UpdateSelectedUnit(_unit, _abilityCategory);
            _selectedUnitAbilitiesWindow.IsVisible = true;
        }

        
        public void UpdateState()
        {
            if (!IsActive) return;

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

            _selectedUnitAbilitiesWindow.ClearSelectedUnit();
            _selectedUnitAbilitiesWindow.IsVisible = false;
        }
    }
}
