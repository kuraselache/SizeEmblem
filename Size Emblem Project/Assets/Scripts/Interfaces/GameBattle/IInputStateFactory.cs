using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.GameBattle
{
    public interface IInputStateFactory
    {

        IInputState ResolveEndTurnWindowState();
        IInputState ResolveSelectUnitState();
        IInputState ResolvePreviewMoveUnitState(IGameUnit unit);
        IInputState ResolveMoveUnitState(IGameUnit unit);
        IInputState ResolveMovingUnitState(IGameUnit unit, IGameMapMovementRoute route);
        IInputState ResolveUnitSelectActionState(IGameUnit unit);



        void DisposeState(IInputState state);
    }
}
