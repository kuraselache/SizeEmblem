using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates.Factory
{
    public class InputStateFactory : IInputStateFactory
    {
        #region Constructor & Dependencies

        private IGameBattle _gameBattle;
        private IGameMap _gameMap;

        // Windows, this should be cleaned up into a manager or something
        private IUnitSummaryWindow _unitSummaryWindow;
        private IEndPhaseWindow _endPhaseWindow;

        public InputStateFactory(
            IGameBattle gameBattle, 
            IGameMap gameMap, 
            IUnitSummaryWindow unitSummaryWindow,
            IEndPhaseWindow endPhaseWindow)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;

            _unitSummaryWindow = unitSummaryWindow;
            _endPhaseWindow = endPhaseWindow;
        }

        #endregion



        public IInputState ResolveEndTurnWindowState()
        {
            return new EndTurnWindowState(_gameBattle, _endPhaseWindow);
        }

        public IInputState ResolveSelectUnitState()
        {
            return new SelectUnitState(_gameBattle, _gameMap, _unitSummaryWindow, this);
        }

        public IInputState ResolvePreviewMoveUnitState(IGameUnit unit)
        {
            return new PreviewMoveUnitState(_gameBattle, _gameMap, unit, _unitSummaryWindow);
        }

        public IInputState ResolveMoveUnitState(IGameUnit unit)
        {
            return new MoveUnitState(_gameBattle, _gameMap, unit, _unitSummaryWindow, this);
        }

        public IInputState ResolveMovingUnitState(IGameUnit unit, IGameMapMovementRoute route)
        {
            return new MovingUnitState(_gameBattle, _gameMap, unit, route, this);
        }

        public IInputState ResolveUnitSelectActionState(IGameUnit unit)
        {
            return new UnitSelectActionState();
        }



        public void DisposeState(IInputState state)
        {
            (state as IDisposable)?.Dispose();
        }
    }
}
