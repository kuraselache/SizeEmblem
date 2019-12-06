using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.GameMap;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.GameUnits
{
    public interface IAbilityEffect
    {
        AbilityResultContainer PreviewResults(AbilityExecuteParameters parameters);

        AbilityResultContainer ExecuteEffect(AbilityExecuteParameters parameters);
    }
}
