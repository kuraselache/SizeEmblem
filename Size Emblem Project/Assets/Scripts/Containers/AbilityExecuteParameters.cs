using SizeEmblem.Scripts.Containers;
using SizeEmblem.Scripts.Interfaces.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Containers
{
    public class AbilityExecuteParameters
    {
        public IGameUnit UnitExecuting; // The unit executing an ability
        public IAbility AbilityExecuting; // The ability that is being executed

        public IGameMapObject Target; // The target of this ability
        public IEnumerable<IGameMapObject> AllTargets; // The fellow targets of this ability. If this target only has one target it'll be a collection containing only Target

        public MapPoint TargetPoint; // The point of the map that was targeted
        public IGameMap GameMap; // The game map reference


        public AbilityExecuteParameters(IGameUnit unit, IAbility ability, IGameMapObject target, IEnumerable<IGameMapObject> targets, MapPoint targetPoint, IGameMap gameMap)
        {
            UnitExecuting = unit;
            AbilityExecuting = ability;
            Target = target;
            AllTargets = targets;
            TargetPoint = targetPoint;
            GameMap = gameMap;
        }
    }
}
