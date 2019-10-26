using SizeEmblem.Assets.Scripts.Interfaces.UI;
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

        public Canvas WindowCanvas;

        #endregion

        public AbilitySelectionButton abilitySelectionButtonPrefab;


        private List<IAbilitySelectionButton> _abilitySelectionButtons = new List<IAbilitySelectionButton>();


        public IGameUnit SelectedUnit { get; private set; }


        public void UpdateSelectedUnit(IGameUnit newUnit)
        {
            for(var i = 0; i < _abilitySelectionButtons.Count; i++)
            {
                if(i >= newUnit.Abilities.Count)
                {
                    _abilitySelectionButtons[i].ClearAbilityData();
                    continue;
                }
                _abilitySelectionButtons[i].UpdateAbilityData(newUnit, newUnit.Abilities[i]);
            }

        }

        public void ClearSelectedUnit()
        {
            _abilitySelectionButtons.ForEach(x => x.ClearAbilityData());
        }

        private void AbilityButtonSelected(IAbilitySelectionButton button, Events.UI.AbilitySelectedEventArgs e)
        {
            Debug.Log(string.Format("Unit: {0} wants to use ability: {1}", e.User.UnitName, e.Ability.FriendlyName));
        }


        // Start is called before the first frame update
        void Start()
        {
            var childrenButtons = this.GetComponentsInChildren<IAbilitySelectionButton>();
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