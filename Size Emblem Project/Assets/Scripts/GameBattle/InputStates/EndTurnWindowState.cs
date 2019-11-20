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

        public void BeginState()
        {
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

        public void AdvanceState(IInputState nextState)
        {
            // This is a leaf-state, it shouldn't advance
        }


        public void DisposeState(IInputState nextState)
        {
            _endPhaseWindow.IsVisible = false;
            _endPhaseWindow.OKAction     -= EndPhaseWindow_OKAction;
            _endPhaseWindow.CancelAction -= EndPhaseWindow_CancelAction;
        }



        private void EndPhaseWindow_OKAction()
        {
            _gameBattle.EndPhase();
        }

        private void EndPhaseWindow_CancelAction()
        {
            _gameBattle.ClearTopInputState();
        }

        
    }
}
