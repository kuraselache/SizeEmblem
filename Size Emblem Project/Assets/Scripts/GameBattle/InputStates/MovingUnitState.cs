using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.Interfaces.GameBattle;
using SizeEmblem.Scripts.Events.GameMap;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.GameBattle.InputStates
{
    public class MovingUnitState : IInputState
    {
        private readonly IGameBattle _gameBattle;
        private readonly IGameMap _gameMap;
        private readonly IGameUnit _unit;
        private readonly IGameMapMovementRoute _route;

        private readonly IInputStateFactory _inputStateFactory;

        public MovingUnitState(IGameBattle gameBattle, IGameMap gameMap, IGameUnit unit, IGameMapMovementRoute route, IInputStateFactory inputStateFactory)
        {
            _gameBattle = gameBattle;
            _gameMap = gameMap;
            _unit = unit;
            _route = route;

            _inputStateFactory = inputStateFactory;
        }

        public bool IsActive { get; private set; }

        private bool _hasStarted;
        private bool _movingComplete;
        private MoveUnitChangeContainer _moveUnitChangesContainer;

        public void Activate()
        {
            IsActive = true;

            if(!_hasStarted)
            {
                _hasStarted = true;

                _gameMap.MoveUnit(_unit, _route);
                _gameMap.UnitMoveCompleted += GameMap_UnitMoveCompleted;
            }
            else
            {
                // Set our unit's location back to their original location
                _gameMap.SetUnitLocation(_unit, _route.StartX, _route.StartY);
                // Undo changes from their move
                _moveUnitChangesContainer.Undo();
                // We already ran once so we should quit this state immediately
                _gameBattle.ClearTopInputState();
            }
        }

        

        public void UpdateState()
        {

        }

        public void Deactivate()
        {
            IsActive = false;
        }


        private void GameMap_UnitMoveCompleted(object sender, UnitMoveCompletedEventArgs e)
        {
            _movingComplete = true;
            _moveUnitChangesContainer = e.MoveUnitChangeContainer;
            _gameMap.UnitMoveCompleted -= GameMap_UnitMoveCompleted;

            _gameBattle.AddInputState(_inputStateFactory.ResolveUnitSelectActionState(_unit));
        }
    }
}
