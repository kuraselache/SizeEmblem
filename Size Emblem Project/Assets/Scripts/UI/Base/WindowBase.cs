using SizeEmblem.Assets.Scripts.Interfaces.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI.Base
{
    public abstract class WindowBase : MonoBehaviour, IWindowBase
    {
        public Canvas UICanvas;

        protected bool _isDirty;


        protected bool _isVisible;
        public virtual bool IsVisible
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



        protected virtual void Start()
        {
            ChangeCanvasVisibility(IsVisible);
        }

        protected virtual void Update()
        {
            if(_isDirty)
            {
                RefreshUI();
            }
        }
    }
}
