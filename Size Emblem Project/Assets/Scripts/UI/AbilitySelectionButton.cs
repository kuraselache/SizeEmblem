using SizeEmblem.Assets.Scripts.Interfaces.Managers;
using SizeEmblem.Assets.Scripts.Managers;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class AbilitySelectionButton : MonoBehaviour
    {
        #region UI Components

        public Canvas ButtonCanvas;

        public Image abilityIconImage;

        public TextMeshProUGUI abilityNameText;
        public TextMeshProUGUI abilityCostText;

        #endregion

        #region Dependencies

        private IGameSystemManager _gameSystem;

        private void GetDependencies()
        {
            _gameSystem = GameSystemManager.Instance;
        }


        #endregion


        public void UpdateAbilityData(IGameUnit user, IAbility ability)
        {
            User = user;
            Ability = ability;
            RefreshUI();
        }

        public IGameUnit User { get; protected set; }
        public IAbility Ability { get; protected set; }


        public void RefreshUI()
        {

            if(Ability == null || User == null)
            {
                IsVisible = false;
                return;
            }

            // Ability icons aren't supported yet but don't forget they exist!

            if(abilityNameText != null)
            {
                abilityNameText.text = Ability.FriendlyName;
            }
            if(abilityCostText != null)
            {
                abilityCostText.text = String.Format("SP: {0}", Ability.SPCost);
            }

            IsEnabled = User.CanUseAbility(Ability);
        }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            private set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                // A disabled button can't be hovered
                if (!_isEnabled) IsHovering = false;
            }
        }

        private bool _isHovering;
        public bool IsHovering
        {
            get { return _isHovering; }
            set
            {
                if (value == _isHovering) return;
                _isHovering = value;
            }
        }

        public void OnMouseOver()
        {
            if (!_gameSystem.IsMouseInputEnabled || !IsEnabled) return;
            IsHovering = true;
        }

        public void OnMouseExit()
        {
            if (!_gameSystem.IsMouseInputEnabled || !IsEnabled) return;
            IsHovering = false;
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

        public void ChangeCanvasEnabled(bool isEnabled)
        {
            ButtonCanvas.enabled = isEnabled;
        }


        public void Start()
        {
            GetDependencies();

            _isVisible = ButtonCanvas.enabled;
            RefreshUI();
        }
    }
}
