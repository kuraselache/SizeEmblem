using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.GameMap
{
    public class GameMapCursor : MonoBehaviour
    {
        public Sprite cursorSprite;

        private bool _cursorEnabled;
        public bool CursorEnabled
        {
            get { return _cursorEnabled; }
            set
            {
                if (value == _cursorEnabled) return;
                _cursorEnabled = value;
                UpdateVisibility(_cursorEnabled);
            }
        }

        private SpriteRenderer _spriteRenderer;

        public void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateVisibility(_cursorEnabled);
        }

        
        public void UpdateVisibility(bool isVisible)
        {
            _spriteRenderer.enabled = isVisible;
        }
    }
}
