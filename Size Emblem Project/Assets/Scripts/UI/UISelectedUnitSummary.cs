using SizeEmblem.Scripts.GameUnits;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SizeEmblem.Scripts.UI
{
    public class UISelectedUnitSummary : MonoBehaviour
    {
        #region UI Components

        public Canvas WindowCanvas;

        public TextMeshProUGUI UnitNameText;

        #endregion


        private IGameUnit _selectedUnit;
        public IGameUnit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (value == _selectedUnit) return;
                _selectedUnit = value;
                RefreshUI();
            }
        }


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

        public void RefreshUI()
        {
            if(SelectedUnit == null)
            {
                IsVisible = false;
                return;
            }

            IsVisible = true;
            UnitNameText.text = SelectedUnit.UnitName;
        }

        public void ChangeCanvasEnabled(bool isEnabled)
        {
            WindowCanvas.enabled = isEnabled;
        }


        #region Components

        void Start()
        {
        }

        #endregion

    }

}