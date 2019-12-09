using SizeEmblem.Assets.Scripts.Interfaces.UI.Helpers;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI.Helpers
{
    public class UnitAttributeText : MonoBehaviour, IUnitTextHelper
    {
        private TextMeshProUGUI _text;

        public UnitAttribute unitAttribute;

        private IGameUnit _selectedUnit;
        public IGameUnit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (value == _selectedUnit) return;
                _selectedUnit = value;
            }
        }


        public void RefreshText()
        {
            // If a unit isn't selected then make the text field empty
            if (SelectedUnit == null)
            {
                _text.text = String.Empty;
                return;
            }

            // Get the attribute value for the unit
            var attribute = SelectedUnit.GetAttribute(unitAttribute);

            _text.text = attribute.ToString();
        }


        public void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            
        }
    }
}
