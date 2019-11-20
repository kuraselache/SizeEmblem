using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Assets.Scripts.UI.Base;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.GameUnits;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class UnitSummaryWindow : WindowBase, IUnitSummaryWindow
    {
        #region UI Components

        public Image Window;

        public TextMeshProUGUI UnitNameText;
        public TextMeshProUGUI UnitHPText;
        public TextMeshProUGUI UnitSPText;

        public float margin = 30;

        #endregion

        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
            }
        }


        private IGameUnit _selectedUnit;
        public IGameUnit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (value == _selectedUnit) return;
                _selectedUnit = value;
                _isDirty = true;
                IsVisible = value != null;
            }
        }

        public override bool IsVisible { get { return _isVisible && _selectedUnit != null && IsEnabled; } }


        public override void RefreshUI()
        {
            // Sanity check: Don't refresh this window if it isn't enabled or 
            if (IsEnabled && SelectedUnit != null)
            {
                // Minimum width of the hover summary window is 100f units
                var targetWidth = 100f;

                if (UnitNameText != null)
                {
                    UnitNameText.text = SelectedUnit.UnitName;
                    targetWidth = Mathf.Max(targetWidth, UnitNameText.preferredWidth);
                }
                if (UnitHPText != null)
                    UnitHPText.text = string.Format("HP: {0} / {1}", SelectedUnit.HP, SelectedUnit.GetAttribute(UnitAttribute.MaxHP));
                if (UnitSPText != null)
                    UnitSPText.text = string.Format("SP: {0} / {1}", SelectedUnit.SP, SelectedUnit.GetAttribute(UnitAttribute.MaxSP));

                // Add our margins to the width of the window
                targetWidth += margin * 2;
                Window.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
            }

            base.RefreshUI();
        }
    }

}