using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class UnitActionWindow : MonoBehaviour
    {
        #region Unitiy Dependencies

        public Canvas UICanvas;

        #endregion

        public Action AttackSelected;
        public Action SpecialSelected;
        public Action DefendSelected;

        public Action ItemSelected;

        public void OnAttackSelected()
        {
            AttackSelected?.Invoke();
        }

        public void OnSpecialSelected()
        {
            SpecialSelected?.Invoke();
        }

        public void OnDefendSelected()
        {
            DefendSelected?.Invoke();
        }

        public void OnItemSelected()
        {
            ItemSelected?.Invoke();
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
            UICanvas.enabled = isEnabled;
        }


        public void Start()
        {
            ChangeCanvasEnabled(IsVisible);
        }
    }
}
