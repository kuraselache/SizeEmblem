using SizeEmblem.Scripts.GameUnits;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SizeEmblem.Scripts.UI
{
    public class SelectedUnitSummaryWindow : MonoBehaviour
    {
        #region UI Components

        public Canvas WindowCanvas;

        public Image Window;

        public TextMeshProUGUI UnitNameText;
        public TextMeshProUGUI UnitHPText;
        public TextMeshProUGUI UnitSPText;

        public float margin = 30;

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

            var targetWidth = 100f;

            if (UnitNameText != null)
            {
                UnitNameText.text = SelectedUnit.UnitName;
                targetWidth = Mathf.Max(targetWidth, UnitNameText.preferredWidth);
            }
            if (UnitHPText != null)
                UnitHPText.text = string.Format("HP: {0} / {1}", SelectedUnit.HP, SelectedUnit.GetAttribute(Constants.UnitAttribute.MaxHP));
            if(UnitSPText != null)
                UnitSPText.text = string.Format("SP: {0} / {1}", SelectedUnit.SP, SelectedUnit.GetAttribute(Constants.UnitAttribute.MaxSP));

            targetWidth += margin * 2;
            Window.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        }

        public void ChangeCanvasEnabled(bool isEnabled)
        {
            WindowCanvas.enabled = isEnabled;
        }


        #region Components

        void Start()
        {
            _isVisible = WindowCanvas.enabled;
            RefreshUI();
        }

        #endregion

    }

}