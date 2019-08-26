using SizeEmblem.Scripts.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SizeEmblem.Scripts.UI
{
    public class UISelectedUnitSummary : MonoBehaviour
    {
        #region UI Components

        public Text UnitNameText;


        #endregion


        private GameUnit _selectedUnit;
        public GameUnit SelectedUnit
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
            _uiCanvas.enabled = isEnabled;
        }


        #region Components

        private Canvas _uiCanvas;

        void Start()
        {
            _uiCanvas = GetComponent<Canvas>();
        }

        #endregion

    }

}