using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class EndTurnWindowState : IInputState
    {
        private IGameBattle _gameBattle;
        private IEndPhaseWindow _endPhaseWindow;

        public EndTurnWindowState(IGameBattle gameBattle, IEndPhaseWindow endPhaseWindow)
        {
            _gameBattle = gameBattle;
            _endPhaseWindow = endPhaseWindow;
        }


        public bool IsActive { get; set; }

        public void Activate()
        {
            IsActive = true;

            _endPhaseWindow.IsVisible = true;
            _endPhaseWindow.OKAction     += EndPhaseWindow_OKAction;
            _endPhaseWindow.CancelAction += EndPhaseWindow_CancelAction;
        }

        public void UpdateState()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _gameBattle.ClearTopInputState();
                return;
            }
        }



        public void Deactivate()
        {
            IsActive = false;

            _endPhaseWindow.IsVisible = false;
            _endPhaseWindow.OKAction     -= EndPhaseWindow_OKAction;
            _endPhaseWindow.CancelAction -= EndPhaseWindow_CancelAction;
        }



        private void EndPhaseWindow_OKAction()
        {
            if (!IsActive) return;
            _gameBattle.EndPhase();
        }

        private void EndPhaseWindow_CancelAction()
        {
            if (!IsActive) return;
            _gameBattle.ClearTopInputState();
        }

        
    }
}
