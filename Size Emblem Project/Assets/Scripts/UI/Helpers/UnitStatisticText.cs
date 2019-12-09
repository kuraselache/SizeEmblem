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
    public class UnitStatisticText : MonoBehaviour, IUnitTextHelper
    {
        private TextMeshProUGUI _text;

        public UnitStatistic unitStatistic;

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
            if(SelectedUnit == null)
            {
                _text.text = String.Empty;
                return;
            }

            // Get the statistic value for the unit
            var statistic = SelectedUnit.GetStatistic(unitStatistic);

            // TODO: some prefix or suffix stuff
            var prefix = String.Empty;
            var suffix = String.Empty;

            if (unitStatistic == UnitStatistic.PropertyDamage) prefix = "$";
            if (unitStatistic == UnitStatistic.BodyCount) suffix = " people";

            // Then format it all into our output string
            _text.text = String.Join(String.Empty, prefix, statistic.ToString("N0"), suffix);
        }


        public void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
    }
}
