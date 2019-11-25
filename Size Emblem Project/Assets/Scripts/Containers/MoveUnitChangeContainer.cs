using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Containers
{
    public class MoveUnitChangeContainer
    {
        public readonly IGameUnit Unit;

        public readonly List<TileStateChangeContainer> TileChanges = new List<TileStateChangeContainer>();

        public readonly Dictionary<UnitStatistic, ulong> UnitStatisticChanges = new Dictionary<UnitStatistic, ulong>();


        public MoveUnitChangeContainer(IGameUnit unit)
        {
            Unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }


        public void ApplyStatisticOffset(UnitStatistic statistic, ulong offset)
        {
            if (!UnitStatisticChanges.ContainsKey(statistic))
            {
                UnitStatisticChanges.Add(statistic, offset);
                return;
            }

            UnitStatisticChanges[statistic] += offset;
        }


        public void Undo()
        {
            // Undo our tracked statistic changes
            foreach(var pair in UnitStatisticChanges)
            {
                Unit.DecrementStatistic(pair.Key, pair.Value);
            }

            // Undo our tile changes too
            TileChanges.ForEach(x => x.Undo());
        }
    }
}
