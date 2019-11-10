using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI.Base
{
    public abstract class WindowBase : MonoBehaviour
    {
        public Canvas UICanvas;

        protected bool _isDirty;


        protected bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                _isDirty = true;
            }
        }

        public virtual void RefreshUI()
        {
            _isDirty = false;

            ChangeCanvasVisibility(IsVisible);
        }

        protected void ChangeCanvasVisibility(bool isVisible)
        {
            UICanvas.enabled = isVisible;
        }



        protected void Start()
        {
            ChangeCanvasVisibility(IsVisible);
        }

        protected void Update()
        {
            if(_isDirty)
            {
                RefreshUI();
                _isDirty = false;
            }
        }
    }
}
