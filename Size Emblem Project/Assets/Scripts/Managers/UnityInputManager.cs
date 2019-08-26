using SizeEmblem.Scripts.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.Managers
{
    public class UnityInputManager : IInputManager
    {

        private Camera _mainCamera;

        public UnityInputManager()
        {
            _mainCamera = Camera.main;
        }


        public Vector3 GetMousePosition()
        {
            if (_mainCamera == null) return Vector3.negativeInfinity;

            return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
