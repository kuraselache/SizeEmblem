using SizeEmblem.Assets.Scripts.Events.UI;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
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
    public class SelectedUnitAbilitiesWindow : MonoBehaviour, ISelectedUnitAbilitiesWindow, IDisposable
    {
        #region UI Components

        public Canvas UICanvas;

        #endregion

        public AbilitySelectionButton abilitySelectionButtonPrefab;


        private List<IAbilitySelectionButton> _abilitySelectionButtons = new List<IAbilitySelectionButton>();


        private IGameUnit _unit;
        private AbilityCategory _category = AbilityCategory.None;


        public void UpdateSelectedUnit(IGameUnit newUnit, AbilityCategory category)
        {
            if (_unit == newUnit && _category == category) return;
            _unit = newUnit;
            _category = category;

            var abilities = newUnit.Abilities.Where(x => x.AbilityCategory == category).ToList();

            for(var i = 0; i < _abilitySelectionButtons.Count; i++)
            {
                if(i >= abilities.Count)
                {
                    _abilitySelectionButtons[i].ClearAbilityData();
                    continue;
                }
                _abilitySelectionButtons[i].UpdateAbilityData(newUnit, abilities[i]);
            }

        }

        public void ClearSelectedUnit()
        {
            _abilitySelectionButtons.ForEach(x => x.ClearAbilityData());
        }

        private void AbilityButtonSelected(IAbilitySelectionButton button, AbilitySelectedEventArgs e)
        {
            Debug.Log(string.Format("Unit: {0} wants to use ability: {1}", e.User.UnitName, e.Ability.FriendlyName));

            SelectedAbility?.Invoke(this, e);
        }

        public event EventHandler<AbilitySelectedEventArgs> SelectedAbility;



        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                ChangeCanvasEnabled(_isVisible);
            }
        }

        public void ChangeCanvasEnabled(bool isEnabled)
        {
            UICanvas.enabled = isEnabled;
        }


        // Start is called before the first frame update
        void Start()
        {
            var childrenButtons = GetComponentsInChildren<IAbilitySelectionButton>();
            foreach(var button in childrenButtons)
            {
                button.Selected += AbilityButtonSelected;
            }
            _abilitySelectionButtons.AddRange(childrenButtons);
        }

        // Update is called once per frame
        void Update()
        {

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