using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Assets.Scripts.UI.Base;
using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class UnitDetailsWindow : WindowBase, IUnitDetailsWindow
    {

        #region Unity Dependencies

        public TextMeshProUGUI UnitNameText;
        public TextMeshProUGUI UnitLevelText;

        public TextMeshProUGUI UnitCurrentHPText;
        public TextMeshProUGUI UnitMaxHPText;

        public TextMeshProUGUI UnitCurrentSPText;
        public TextMeshProUGUI UnitMaxSPText;

        public TextMeshProUGUI UnitStrengthText;
        public TextMeshProUGUI UnitMagicText;
        public TextMeshProUGUI UnitDefenseText;
        public TextMeshProUGUI UnitResistanceText;
        public TextMeshProUGUI UnitSpeedText;
        public TextMeshProUGUI UnitMoveText;

        public TextMeshProUGUI UnitSizeText;
        public TextMeshProUGUI UnitHeightText;
        public TextMeshProUGUI UnitWeightText;

        #endregion

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

        public override void RefreshUI()
        {
            base.RefreshUI();

            if (SelectedUnit == null) return;


            if (UnitNameText != null)
                UnitNameText.text = SelectedUnit.UnitName;
            if (UnitLevelText != null)
                UnitLevelText.text = SelectedUnit.Level.ToString();
            
            if (UnitCurrentHPText != null)
                UnitCurrentHPText.text = SelectedUnit.HP.ToString();
            if (UnitMaxHPText != null)
                UnitMaxHPText.text = SelectedUnit.GetAttribute(UnitAttribute.MaxHP).ToString();

            if (UnitCurrentSPText != null)
                UnitCurrentSPText.text = SelectedUnit.SP.ToString();
            if (UnitMaxSPText != null)
                UnitMaxSPText.text = SelectedUnit.GetAttribute(UnitAttribute.MaxSP).ToString();

            if (UnitStrengthText != null)
                UnitStrengthText.text = SelectedUnit.GetAttribute(UnitAttribute.Strength).ToString();
            if (UnitMagicText != null)
                UnitMagicText.text = SelectedUnit.GetAttribute(UnitAttribute.Magic).ToString();
            if (UnitDefenseText != null)
                UnitDefenseText.text = SelectedUnit.GetAttribute(UnitAttribute.Defense).ToString();
            if (UnitResistanceText != null)
                UnitResistanceText.text = SelectedUnit.GetAttribute(UnitAttribute.Resistance).ToString();
            if (UnitSpeedText != null)
                UnitSpeedText.text = SelectedUnit.GetAttribute(UnitAttribute.Speed).ToString();
            if (UnitMoveText != null)
                UnitMoveText.text = SelectedUnit.GetAttribute(UnitAttribute.Movement).ToString();

            if (UnitSizeText != null)
                UnitSizeText.text = SelectedUnit.SizeCategory.ToString();

            if (UnitHeightText != null)
                UnitHeightText.text = SelectedUnit.ShowHeight ? SelectedUnit.Height.ToString() : "N/A";
            if (UnitWeightText != null)
                UnitWeightText.text = SelectedUnit.ShowWeight ? SelectedUnit.Weight.ToString() : "N/A";
        }
    }
}