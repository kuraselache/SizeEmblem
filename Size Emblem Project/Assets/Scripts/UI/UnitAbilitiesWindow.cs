using SizeEmblem.Assets.Scripts.Events.UI;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Assets.Scripts.UI.Base;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Extensions;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class UnitAbilitiesWindow : WindowBase, IUnitAbilitiesWindow, IDisposable
    {


        public AbilitySelectionButton abilitySelectionButtonPrefab;




        private List<IAbilitySelectionButton> _abilitySelectionButtons = new List<IAbilitySelectionButton>();


        private IGameUnit _unit;
        private AbilityCategory _category = AbilityCategory.None;


        public void UpdateSelectedUnit(IGameUnit newUnit, AbilityCategory category)
        {
            if (_unit == newUnit && _category == category) return;
            _unit = newUnit;
            _category = category;
        }


        public override void RefreshUI()
        {

            var abilities = _unit.Abilities.Where(x => x.AbilityCategory == _category).ToList();

            for (var i = 0; i < _abilitySelectionButtons.Count; i++)
            {
                if (i >= abilities.Count)
                {
                    _abilitySelectionButtons[i].ClearAbilityData();
                    continue;
                }
                _abilitySelectionButtons[i].UpdateAbilityData(_unit, abilities[i]);
            }

            _abilitySelectionButtons.ForEach(x => x.RefreshUI());

            base.RefreshUI();
        }


        public void ClearSelectedUnit()
        {
            _abilitySelectionButtons.ForEach(x => x.ClearAbilityData());
        }

        private void AbilityButtonSelected(IAbilitySelectionButton button, AbilitySelectedEventArgs e)
        {
            SelectedAbility?.Invoke(this, e);
        }

        public event EventHandler<AbilitySelectedEventArgs> SelectedAbility;



        // Start is called before the first frame update
        public new void Start()
        {
            var childrenButtons = GetComponentsInChildren<IAbilitySelectionButton>();
            foreach(var button in childrenButtons)
            {
                button.Selected += AbilityButtonSelected;
            }
            _abilitySelectionButtons.AddRange(childrenButtons);

            base.Start();
        }



        #region IDisposable Implementation

        public void Dispose()
        {
            if(_abilitySelectionButtons != null && _abilitySelectionButtons.Any())
                _abilitySelectionButtons.ForEach(x => x.Selected -= AbilityButtonSelected);
        }

        #endregion

    }
}